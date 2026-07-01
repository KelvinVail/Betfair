using System.Collections.Concurrent;
using Betfair.Core.Authentication;
using Betfair.Core.Client;
using Betfair.Stream.MarketCache;
using Betfair.Stream.Messages;
using Betfair.Stream.OrderCache;
using Betfair.Stream.Responses;

namespace Betfair.Stream;

/// <summary>
/// Used to subscribe to Betfair market and order streams.
/// </summary>
public class Subscription : IDisposable
{
    private readonly TokenProvider _tokenProvider;
    private readonly string _appKey;
    private readonly BetfairHttpClient? _httpClient;
    private readonly bool _ownsTransport;
    private readonly ConcurrentDictionary<int, TaskCompletionSource<ChangeMessage>> _statusAwaiters = new ();

    private BetfairTcpClient _tcpClient = new ();

    private System.IO.Stream? _stream;
    private IPipeline _pipe;

    private int _requestId;
    private bool _isAuthenticated;
    private bool _disposedValue;
    private int _readerActive;

    // Resume token tracking
    private string? _lastInitialClk;
    private string? _lastClk;

    // Last subscriptions to auto-resubscribe
    private LastMarketSubscription? _lastMarket;
    private LastOrderSubscription? _lastOrder;

    /// <summary>
    /// Initializes a new instance of the <see cref="Subscription"/> class.
    /// Used to subscribe to Betfair market and order streams.
    /// </summary>
    /// <param name="credentials">The <see cref="Credentials"/> object need to authenticate with Betfair.</param>
    [ExcludeFromCodeCoverage]
    public Subscription(Credentials credentials)
    {
        ArgumentNullException.ThrowIfNull(credentials);

        _appKey = credentials.AppKey;
        _httpClient = new BetfairHttpClient(credentials.Certificate);
        _tokenProvider = new TokenProvider(_httpClient, credentials);
        _stream = _tcpClient.GetAuthenticatedSslStream();
        _pipe = new Pipeline(_stream);
        _ownsTransport = true;
    }

    internal Subscription(TokenProvider tokenProvider, string appKey, IPipeline pipe)
    {
        _tokenProvider = tokenProvider;
        _appKey = appKey;
        _pipe = pipe;
        _stream = null;
        _ownsTransport = false;
    }

    /// <summary>
    /// Gets the zero-allocation market cache processor that maintains live market state.
    /// Access this property at any time to read the latest market/runner data.
    /// Populated when <see cref="RunMarketCache"/> is active.
    /// </summary>
    public MarketCacheProcessor MarketProcessor { get; } = new ();

    /// <summary>
    /// Gets the zero-allocation order cache processor that maintains live order state.
    /// Access this property at any time to read the latest order data.
    /// Populated when <see cref="RunOrderCache"/> is active.
    /// </summary>
    public OrderCacheProcessor OrderProcessor { get; } = new ();

    /// <summary>
    /// Subscribe to a market stream.
    /// </summary>
    /// <param name="marketFilter">Used to define which markets to subscribe to.</param>
    /// <param name="dataFilter">Optional: Used to define what data to include in the market stream.
    /// If null Best Available Prices are returned.</param>
    /// <param name="conflate">Optional: Data will be rolled up and sent on each increment of this time interval.</param>
    /// <param name="initialClk">Optional: Resume token initialClk provided by Betfair to resume a previous stream.</param>
    /// <param name="clk">Optional: Resume token clk provided by Betfair to resume a previous stream.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the subscribe operation.</param>
    /// <returns>A task that completes when the subscription request has been sent.</returns>
    public async Task Subscribe(
        StreamMarketFilter marketFilter,
        DataFilter? dataFilter = null,
        TimeSpan? conflate = null,
        string? initialClk = null,
        string? clk = null,
        CancellationToken cancellationToken = default)
    {
        await Authenticate(cancellationToken).ConfigureAwait(false);

        _requestId++;
        var marketSubscription = new MarketSubscription(_requestId, marketFilter, dataFilter, conflate, initialClk, clk);
        _requestId = await EnsureSuccess(marketSubscription, cancellationToken).ConfigureAwait(false);

        _lastMarket = new LastMarketSubscription(marketFilter, dataFilter, conflate);
    }

    /// <summary>
    /// Subscribe to new and open orders. To retrieve historical orders use the <see cref="Api.BetfairApiClient"/> class.
    /// </summary>
    /// <param name="orderFilter">Optional: Used to shape and filter the order data returned on the stream.</param>
    /// <param name="conflate">Optional: Data will be rolled up and sent on each increment of this time interval.</param>
    /// <param name="initialClk">Optional: Resume token initialClk provided by Betfair to resume a previous stream.</param>
    /// <param name="clk">Optional: Resume token clk provided by Betfair to resume a previous stream.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>An awaitable task.</returns>
    public async Task SubscribeToOrders(
        StreamOrderFilter? orderFilter = null,
        TimeSpan? conflate = null,
        string? initialClk = null,
        string? clk = null,
        CancellationToken cancellationToken = default)
    {
        await Authenticate(cancellationToken).ConfigureAwait(false);

        _requestId++;
        var orderSubscription = new OrderSubscription(_requestId, orderFilter, conflate, initialClk, clk);
        _requestId = await EnsureSuccess(orderSubscription, cancellationToken).ConfigureAwait(false);

        _lastOrder = new LastOrderSubscription(orderFilter, conflate);
    }

    /// <summary>
    /// Asynchronously iterate raw byte sequences as they become available on the stream,
    /// in the exact order they arrived. Each element represents one newline-delimited message
    /// before JSON deserialization. Useful for logging, recording, or custom parsing.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>An Async Enumerable of <see cref="ReadOnlyMemory{Byte}"/> representing each raw message.</returns>
    public async IAsyncEnumerable<ReadOnlyMemory<byte>> ReadRawLines([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Authenticate(cancellationToken).ConfigureAwait(false);

        await foreach (var line in _pipe.ReadLines(cancellationToken).ConfigureAwait(false))
            yield return line;
    }

    /// <summary>
    /// Asynchronously iterate ChangeMessages as they become available on the stream.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>An Async Enumerable of <see cref="ChangeMessage"/>.</returns>
    public async IAsyncEnumerable<ChangeMessage> ReadLines([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Interlocked.Exchange(ref _readerActive, 1);
        try
        {
            await foreach (var change in RunReadLoop(cancellationToken).ConfigureAwait(false))
                yield return change;
        }
        finally
        {
            Interlocked.Exchange(ref _readerActive, 0);
        }
    }

    /// <summary>
    /// Subscribes to a market stream and processes all messages directly into <see cref="MarketProcessor"/>
    /// using the zero-copy pipeline. No intermediate ChangeMessage objects are allocated.
    /// This method blocks until the stream ends or the cancellation token is triggered.
    /// </summary>
    /// <param name="marketFilter">Used to define which markets to subscribe to.</param>
    /// <param name="dataFilter">Optional: Used to define what data to include in the market stream.</param>
    /// <param name="conflate">Optional: Data will be rolled up and sent on each increment of this time interval.</param>
    /// <param name="onUpdate">Optional: Callback invoked after each message is processed into the cache.
    /// Use for latency-sensitive consumers that need to react immediately to price changes.</param>
    /// <param name="cancellationToken">A cancellation token to stop the stream.</param>
    /// <returns>A task that completes when the stream ends or is cancelled.</returns>
    public async Task RunMarketCache(
        StreamMarketFilter marketFilter,
        DataFilter? dataFilter = null,
        TimeSpan? conflate = null,
        Action<MarketCacheProcessor>? onUpdate = null,
        CancellationToken cancellationToken = default)
    {
        await Subscribe(marketFilter, dataFilter, conflate, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (_pipe is Pipeline pipeline)
        {
            if (onUpdate is null)
            {
                await pipeline.ProcessLines(MarketProcessor.Process, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                var callback = onUpdate;
                var processor = MarketProcessor;
                await pipeline.ProcessLines(
                    span =>
                    {
                        processor.Process(span);
                        callback(processor);
                    },
                    cancellationToken).ConfigureAwait(false);
            }
        }
        else
        {
            // Fallback for test doubles using IPipeline
            await foreach (var line in _pipe.ReadLines(cancellationToken).ConfigureAwait(false))
            {
                MarketProcessor.Process(line);
                onUpdate?.Invoke(MarketProcessor);
            }
        }
    }

    /// <summary>
    /// Subscribes to the order stream and processes all messages directly into <see cref="OrderProcessor"/>
    /// using the zero-copy pipeline. No intermediate ChangeMessage objects are allocated.
    /// This method blocks until the stream ends or the cancellation token is triggered.
    /// </summary>
    /// <param name="orderFilter">Optional: Used to shape and filter the order data returned on the stream.</param>
    /// <param name="conflate">Optional: Data will be rolled up and sent on each increment of this time interval.</param>
    /// <param name="onUpdate">Optional: Callback invoked after each message is processed into the cache.
    /// Use for latency-sensitive consumers that need to react immediately to order changes.</param>
    /// <param name="cancellationToken">A cancellation token to stop the stream.</param>
    /// <returns>A task that completes when the stream ends or is cancelled.</returns>
    public async Task RunOrderCache(
        StreamOrderFilter? orderFilter = null,
        TimeSpan? conflate = null,
        Action<OrderCacheProcessor>? onUpdate = null,
        CancellationToken cancellationToken = default)
    {
        await SubscribeToOrders(orderFilter, conflate, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (_pipe is Pipeline pipeline)
        {
            if (onUpdate is null)
            {
                await pipeline.ProcessLines(OrderProcessor.Process, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                var callback = onUpdate;
                var processor = OrderProcessor;
                await pipeline.ProcessLines(
                    span =>
                    {
                        processor.Process(span);
                        callback(processor);
                    },
                    cancellationToken).ConfigureAwait(false);
            }
        }
        else
        {
            // Fallback for test doubles using IPipeline
            await foreach (var line in _pipe.ReadLines(cancellationToken).ConfigureAwait(false))
            {
                OrderProcessor.Process(line);
                onUpdate?.Invoke(OrderProcessor);
            }
        }
    }

    /// <summary>
    /// Subscribes to both market and order streams and processes all messages directly into
    /// <see cref="MarketProcessor"/> and <see cref="OrderProcessor"/> using the zero-copy pipeline.
    /// Both processors receive every line; each filters on its own operation type.
    /// This uses a single TCP connection for the lowest possible latency.
    /// </summary>
    /// <param name="marketFilter">Used to define which markets to subscribe to.</param>
    /// <param name="dataFilter">Optional: Used to define what data to include in the market stream.</param>
    /// <param name="orderFilter">Optional: Used to shape and filter the order data returned on the stream.</param>
    /// <param name="conflate">Optional: Data will be rolled up and sent on each increment of this time interval.</param>
    /// <param name="onUpdate">Optional: Callback invoked after each message is processed into both caches.
    /// Use for latency-sensitive consumers that need correlated market and order state.</param>
    /// <param name="cancellationToken">A cancellation token to stop the stream.</param>
    /// <returns>A task that completes when the stream ends or is cancelled.</returns>
    public async Task RunMarketAndOrderCaches(
        StreamMarketFilter marketFilter,
        DataFilter? dataFilter = null,
        StreamOrderFilter? orderFilter = null,
        TimeSpan? conflate = null,
        Action<MarketCacheProcessor, OrderCacheProcessor>? onUpdate = null,
        CancellationToken cancellationToken = default)
    {
        await Subscribe(marketFilter, dataFilter, conflate, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        await SubscribeToOrders(orderFilter, conflate, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (_pipe is Pipeline pipeline)
        {
            var mp = MarketProcessor;
            var op = OrderProcessor;

            if (onUpdate is null)
            {
                await pipeline.ProcessLines(
                    span =>
                    {
                        mp.Process(span);
                        op.Process(span);
                    },
                    cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var callback = onUpdate;
                await pipeline.ProcessLines(
                    span =>
                    {
                        mp.Process(span);
                        op.Process(span);
                        callback(mp, op);
                    },
                    cancellationToken).ConfigureAwait(false);
            }
        }
        else
        {
            // Fallback for test doubles using IPipeline
            await foreach (var line in _pipe.ReadLines(cancellationToken).ConfigureAwait(false))
            {
                MarketProcessor.Process(line);
                OrderProcessor.Process(line);
                onUpdate?.Invoke(MarketProcessor, OrderProcessor);
            }
        }
    }

    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [ExcludeFromCodeCoverage]
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            _tcpClient.Dispose();
            _stream?.Dispose();
            _httpClient?.Dispose();
        }

        _disposedValue = true;
    }

    private static bool ShouldEndWithoutReconnect(bool ownsTransport, int stalled)
        => !ownsTransport && Volatile.Read(ref stalled) == 0;

    private static bool ConformationReceived(ChangeMessage? status, int id) =>
        status is not null && status.Id == id && string.Equals(status.StatusCode, "SUCCESS", StringComparison.OrdinalIgnoreCase);

    private async Task Authenticate(CancellationToken cancellationToken = default)
    {
        if (_isAuthenticated) return;

        _requestId++;
        var token = await _tokenProvider.GetToken(cancellationToken);
        var authMessage = new Authentication(_requestId, token, _appKey);
        _requestId = await EnsureSuccess(authMessage, cancellationToken).ConfigureAwait(false);

        _isAuthenticated = true;
    }

    private bool UseInBandWaiter()
    {
        // Only when the main reader is active should we await status in-band.
        return Volatile.Read(ref _readerActive) == 1;
    }

    private async Task<int> EnsureSuccess(MessageBase request, CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromSeconds(1);

        if (UseInBandWaiter())
        {
            await WriteAndAwaitStatus(request, timeout, cancellationToken).ConfigureAwait(false);
            return request.Id;
        }

        // No reader active and we own the transport: write without consuming the raw stream.
        await _pipe.WriteLine(request).ConfigureAwait(false);
        return request.Id;
    }

    private async Task WriteAndAwaitStatus(MessageBase request, TimeSpan timeout, CancellationToken cancellationToken)
    {
        // Register waiter by request id
        var tcs = new TaskCompletionSource<ChangeMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        _statusAwaiters[request.Id] = tcs;
        try
        {
            await _pipe.WriteLine(request).ConfigureAwait(false);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);
            using var reg = cts.Token.Register(static state => ((TaskCompletionSource<ChangeMessage>)state!).TrySetCanceled(), tcs);

            var status = await tcs.Task.ConfigureAwait(false);
            if (!ConformationReceived(status, request.Id))
            {
                // Retry once by incrementing id and re-sending
                _statusAwaiters.TryRemove(request.Id, out _);
                request.Id++;
                var retryTcs = new TaskCompletionSource<ChangeMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
                _statusAwaiters[request.Id] = retryTcs;

                await _pipe.WriteLine(request).ConfigureAwait(false);

                using var cts2 = new CancellationTokenSource(timeout);
                using var reg2 = cts2.Token.Register(static state => ((TaskCompletionSource<ChangeMessage>)state!).TrySetCanceled(), retryTcs);

                var retryStatus = await retryTcs.Task.ConfigureAwait(false);
                if (!ConformationReceived(retryStatus, request.Id))
                    throw new TimeoutException("No status response received after subscription.");
            }
        }
        finally
        {
            _statusAwaiters.TryRemove(request.Id, out _);
        }
    }

    private async IAsyncEnumerable<ChangeMessage> RunReadLoop([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (true)
        {
            using var stallCts = new CancellationTokenSource();
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, stallCts.Token);

            var stalled = 0;
            async Task OnStall()
            {
                Interlocked.Exchange(ref stalled, 1);
                await stallCts.CancelAsync().ConfigureAwait(false);
            }

            var guarded = ReadOnce(linked.Token).WithIdleWatchdog(
                onStall: OnStall,
                defaultHeartbeat: null,
                thresholdMultiplier: 2.5,
                pollInterval: TimeSpan.FromSeconds(1),
                token: linked.Token);

            await foreach (var change in guarded.ConfigureAwait(false))
                yield return change;

            if (cancellationToken.IsCancellationRequested)
                yield break;

            if (ShouldEndWithoutReconnect(_ownsTransport, stalled))
                yield break;

            await ReconnectAndResubscribe(cancellationToken).ConfigureAwait(false);
        }
    }

    private async IAsyncEnumerable<ChangeMessage> ReadOnce([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var line in _pipe.ReadLines(cancellationToken).ConfigureAwait(false))
        {
            var received = DateTimeOffset.UtcNow.Ticks;
            var changeMessage = JsonSerializer.Deserialize(line, SerializerContext.Default.ChangeMessage) !;
            changeMessage.ReceivedTick = received;
            changeMessage.DeserializedTick = DateTimeOffset.UtcNow.Ticks;

            if (!string.IsNullOrEmpty(changeMessage.InitialClock)) _lastInitialClk = changeMessage.InitialClock;
            if (!string.IsNullOrEmpty(changeMessage.Clock)) _lastClk = changeMessage.Clock;

            MaybeResolveStatusWaiter(changeMessage);

            yield return changeMessage;
        }
    }

    private void MaybeResolveStatusWaiter(ChangeMessage changeMessage)
    {
        if (string.Equals(changeMessage.Operation, "status", StringComparison.OrdinalIgnoreCase)
            && changeMessage.Id != 0
            && _statusAwaiters.TryRemove(changeMessage.Id, out var waiter))
        {
            waiter.TrySetResult(changeMessage);
        }
    }

    private async Task ReconnectAndResubscribe(CancellationToken cancellationToken)
    {
        _isAuthenticated = false;

        if (_stream is not null)
        {
            try
            {
                await _stream.DisposeAsync().ConfigureAwait(false);
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }

            // Dispose the old TcpClient and create a new one for reconnection
            _tcpClient.Dispose();
            _tcpClient = new BetfairTcpClient();

            _stream = _tcpClient.GetAuthenticatedSslStream();
            _pipe = new Pipeline(_stream);
        }

        await Authenticate(cancellationToken).ConfigureAwait(false);

        if (_lastMarket is { } m)
            await WriteMarketWithStatusAsync(m, cancellationToken).ConfigureAwait(false);

        if (_lastOrder is { } o)
            await WriteOrderWithStatusAsync(o, cancellationToken).ConfigureAwait(false);
    }

    private async Task WriteMarketWithStatusAsync(LastMarketSubscription m, CancellationToken cancellationToken)
    {
        _requestId++;
        var marketSubscription = new MarketSubscription(_requestId, m.Filter, m.DataFilter, m.Conflate, _lastInitialClk, _lastClk);
        _requestId = await EnsureSuccess(marketSubscription, cancellationToken).ConfigureAwait(false);
    }

    private async Task WriteOrderWithStatusAsync(LastOrderSubscription o, CancellationToken cancellationToken)
    {
        _requestId++;
        var orderSubscription = new OrderSubscription(_requestId, o.Filter, o.Conflate, _lastInitialClk, _lastClk);
        _requestId = await EnsureSuccess(orderSubscription, cancellationToken).ConfigureAwait(false);
    }

    private sealed class LastMarketSubscription(StreamMarketFilter filter, DataFilter? dataFilter, TimeSpan? conflate)
    {
        public StreamMarketFilter Filter { get; } = filter;

        public DataFilter? DataFilter { get; } = dataFilter;

        public TimeSpan? Conflate { get; } = conflate;
    }

    private sealed class LastOrderSubscription(StreamOrderFilter? filter, TimeSpan? conflate)
    {
        public StreamOrderFilter? Filter { get; } = filter;

        public TimeSpan? Conflate { get; } = conflate;
    }
}
