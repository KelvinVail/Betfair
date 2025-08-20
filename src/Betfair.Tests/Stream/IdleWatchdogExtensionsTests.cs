using Betfair.Stream;
using Betfair.Stream.Responses;

namespace Betfair.Tests.Stream;

public class IdleWatchdogExtensionsTests
{
    [Fact]
    public async Task DoesNotTriggerOnStallWhenMessagesArriveWithinThreshold()
    {
        var triggered = false;
        var hbMs = 200; // heartbeat provided by stream
        var items = new (ChangeMessage, int)[]
        {
            (new ChangeMessage { HeartbeatMs = hbMs }, 0),
            (new ChangeMessage(), 100),
            (new ChangeMessage(), 100),
            (new ChangeMessage(), 100),
        };

        var list = new List<ChangeMessage>();
        await foreach (var m in Sequence(items).WithIdleWatchdog(
            onStall: async () =>
            {
                triggered = true;
                await Task.CompletedTask;
            },
            defaultHeartbeat: TimeSpan.FromMilliseconds(500),
            thresholdMultiplier: 2.5,
            pollInterval: TimeSpan.FromMilliseconds(25),
            token: default))
        {
            list.Add(m);
        }

        triggered.Should().BeFalse();
        list.Should().HaveCount(4);
    }

    [Fact]
    public async Task TriggersOnStallWhenNoMessagesArriveWithinThreshold()
    {
        var tcs = new TaskCompletionSource();
        var hbMs = 100; // 100ms heartbeat
        var items = new (ChangeMessage, int)[]
        {
            (new ChangeMessage { HeartbeatMs = hbMs }, 0),
            (new ChangeMessage(), hbMs),

            // next gap intentionally long to trigger stall
        };

        var list = new List<ChangeMessage>();
        await foreach (var m in Sequence(items).WithIdleWatchdog(
            onStall: async () =>
            {
                tcs.TrySetResult();
                await Task.CompletedTask;
            },
            defaultHeartbeat: TimeSpan.FromMilliseconds(100),
            thresholdMultiplier: 2.0, // 2x hb => 200ms
            pollInterval: TimeSpan.FromMilliseconds(25),
            token: default))
        {
            list.Add(m);
            if (list.Count == 2)
            {
                // wait long enough to pass threshold so watchdog cancels
                await Task.Delay(300);
            }
        }

        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        list.Should().HaveCount(2);
    }

    [Fact]
    public async Task UsesDefaultHeartbeatWhenStreamDoesNotSupplyOne()
    {
        var triggered = false;
        var defaultHb = TimeSpan.FromMilliseconds(100);

        var list = new List<ChangeMessage>();
        await foreach (var m in Sequence((new ChangeMessage(), 0)).WithIdleWatchdog(
            onStall: async () =>
            {
                triggered = true;
                await Task.CompletedTask;
            },
            defaultHeartbeat: defaultHb,
            thresholdMultiplier: 1.5, // 150ms
            pollInterval: TimeSpan.FromMilliseconds(25),
            token: default))
        {
            list.Add(m);
            await Task.Delay(200); // exceed threshold to cause stall
        }

        triggered.Should().BeTrue();
        list.Should().HaveCount(1);
    }

    [Fact]
    public async Task WatchdogCallbackIsInvokedOnce()
    {
        var count = 0;
        var hbMs = 50;

        await foreach (var msg in Sequence((new ChangeMessage { HeartbeatMs = hbMs }, 0)).WithIdleWatchdog(
            onStall: async () =>
            {
                Interlocked.Increment(ref count);
                await Task.CompletedTask;
            },
            defaultHeartbeat: TimeSpan.FromMilliseconds(50),
            thresholdMultiplier: 1.0,
            pollInterval: TimeSpan.FromMilliseconds(10),
            token: default))
        {
            await Task.Delay(100); // exceed 1x hb
            _ = msg; // ensure msg is used
        }

        count.Should().Be(1);
    }

    [Fact]
    public async Task CancellationEndsSequenceWithoutTriggeringStall()
    {
        var triggered = false;
        using var cts = new CancellationTokenSource();

        var enumerator = Sequence((new ChangeMessage { HeartbeatMs = 1000 }, 0)).WithIdleWatchdog(
            onStall: async () =>
            {
                triggered = true;
                await Task.CompletedTask;
            },
            defaultHeartbeat: TimeSpan.FromMilliseconds(1000),
            thresholdMultiplier: 2.0,
            pollInterval: TimeSpan.FromMilliseconds(25),
            token: cts.Token)
            .GetAsyncEnumerator();

        // Move once then cancel
        await enumerator.MoveNextAsync();
        await cts.CancelAsync();

        // Drain
        try
        {
            while (await enumerator.MoveNextAsync())
            {
                // no-op
            }
        }
        catch (OperationCanceledException)
        {
            // ignore cancellation
        }

        triggered.Should().BeFalse();
    }

    // Helper sequence used by tests (public wrapper then private core) must be declared before tests for StyleCop ordering
    private static IAsyncEnumerable<ChangeMessage> Sequence(params (ChangeMessage msg, int delayMs)[] items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return SequenceCore(items);
    }

    // Helper sequence used by tests (Sequence is public, core is private)
    private static async IAsyncEnumerable<ChangeMessage> SequenceCore((ChangeMessage msg, int delayMs)[] items)
    {
        foreach (var (msg, delayMs) in items)
        {
            if (delayMs > 0)
            {
                await Task.Delay(delayMs);
            }

            yield return msg;
        }
    }
}
