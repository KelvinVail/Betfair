using Betfair.Stream.Responses;

namespace Betfair.Stream;

/// <summary>
/// Idle watchdog for stream change messages. Wraps an async sequence and monitors for stalls using
/// heartbeat intervals from ChangeMessage.HeartbeatMs or a provided default.
/// When the idle threshold is exceeded, the supplied onStall callback is invoked and the sequence ends.
/// </summary>
internal static class IdleWatchdogExtensions
{
    /// <summary>
    /// Wraps a stream of ChangeMessage with an idle watchdog.
    /// The watchdog triggers onStall when no messages are received for thresholdMultiplier × heartbeat.
    /// Heartbeat defaults to 5 seconds if not provided by the stream and not overridden.
    /// </summary>
    /// <param name="source">The source ChangeMessage sequence.</param>
    /// <param name="onStall">Callback invoked exactly once when idle threshold is exceeded.</param>
    /// <param name="defaultHeartbeat">Optional default heartbeat when stream does not provide HeartbeatMs. Defaults to 5 seconds.</param>
    /// <param name="thresholdMultiplier">Multiplier applied to heartbeat to compute idle threshold. Defaults to 2.5.</param>
    /// <param name="pollInterval">How often to check for idleness. Defaults to 1 second.</param>
    /// <param name="token">Cancellation token to end the sequence.</param>
    /// <returns>A wrapped sequence that yields the same messages until a stall is detected.</returns>
    public static IAsyncEnumerable<ChangeMessage> WithIdleWatchdog(
        this IAsyncEnumerable<ChangeMessage> source,
        Func<Task> onStall,
        TimeSpan? defaultHeartbeat = null,
        double thresholdMultiplier = 2.5,
        TimeSpan? pollInterval = null,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(onStall);

        return WithIdleWatchdogImpl(source, onStall, defaultHeartbeat, thresholdMultiplier, pollInterval, token);
    }

    private static async IAsyncEnumerable<ChangeMessage> WithIdleWatchdogImpl(
        IAsyncEnumerable<ChangeMessage> source,
        Func<Task> onStall,
        TimeSpan? defaultHeartbeat,
        double thresholdMultiplier,
        TimeSpan? pollInterval,
        [EnumeratorCancellation] CancellationToken token)
    {
        var hb = defaultHeartbeat ?? TimeSpan.FromSeconds(5);
        var poll = pollInterval ?? TimeSpan.FromSeconds(1);

        using var stallCts = new CancellationTokenSource();
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, stallCts.Token);

        var lastSeen = DateTimeOffset.UtcNow;
        var expectedHeartbeat = hb;
        var stalled = 0; // 0 = not stalled, 1 = stalled

        TimeSpan GetThreshold()
            => TimeSpan.FromMilliseconds(expectedHeartbeat.TotalMilliseconds * thresholdMultiplier);

        bool IsStalled()
            => DateTimeOffset.UtcNow - lastSeen > GetThreshold();

        bool TryTriggerStall()
            => Interlocked.Exchange(ref stalled, 1) == 0;

        var watchdog = RunWatchdogAsync(IsStalled, TryTriggerStall, onStall, stallCts, poll, linked.Token);

        void UpdateHeartbeat(ChangeMessage msg)
        {
            var hbMs = msg.HeartbeatMs.GetValueOrDefault();
            if (hbMs > 0)
            {
                expectedHeartbeat = TimeSpan.FromMilliseconds(hbMs);
            }
        }

        try
        {
            await foreach (var msg in source.WithCancellation(linked.Token).ConfigureAwait(false))
            {
                UpdateHeartbeat(msg);
                lastSeen = DateTimeOffset.UtcNow;
                yield return msg;
            }
        }
        finally
        {
            // Ensure background task ends (no exceptions should be thrown here)
            await stallCts.CancelAsync().ConfigureAwait(false);
            await Task.WhenAny(watchdog, Task.Delay(TimeSpan.FromMilliseconds(1), CancellationToken.None)).ConfigureAwait(false);
        }
    }

    private static Task RunWatchdogAsync(
        Func<bool> isStalled,
        Func<bool> tryTriggerStall,
        Func<Task> onStall,
        CancellationTokenSource stallCts,
        TimeSpan poll,
        CancellationToken token)
    {
        return Task.Run(
            async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (isStalled())
                        {
                            if (tryTriggerStall())
                            {
                                try
                                {
                                    await onStall().ConfigureAwait(false);
                                }
                                catch (OperationCanceledException)
                                {
                                    // ignore cancellation in user callback
                                }

                                await stallCts.CancelAsync().ConfigureAwait(false);
                            }

                            break;
                        }

                        try
                        {
                            await Task.Delay(poll, token).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // normal shutdown
                }
            },
            token);
    }
}
