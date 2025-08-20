using Betfair.Core.Authentication;
using Betfair.Core.Client;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair.Stream;

/// <summary>
/// Used to subscribe to Betfair market and order streams.
/// </summary>
public class Subscription : IDisposable
{
    private readonly BetfairTcpClient _tcpClient = new ();
    private readonly TokenProvider _tokenProvider;
    private readonly string _appKey;
    private readonly BetfairHttpClient? _httpClient;

    private readonly bool _ownsTransport;

    private System.IO.Stream? _stream;
    private IPipeline _pipe;

    private int _requestId;
    private bool _isAuthenticated;
    private bool _disposedValue;

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
        await Authenticate(cancellationToken);

        _requestId++;
        var marketSubscription = new MarketSubscription(_requestId, marketFilter, dataFilter, conflate, initialClk, clk);
        await _pipe.WriteLine(marketSubscription);

        // Track last subscription to support automatic reconnection
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
        await Authenticate(cancellationToken);

        _requestId++;
        await _pipe.WriteLine(new OrderSubscription(_requestId, orderFilter, conflate, initialClk, clk));

        // Track last subscription for automatic reconnection
        _lastOrder = new LastOrderSubscription(orderFilter, conflate);
    }

    /// <summary>
    /// Asynchronously iterate ChangeMessages as they become available on the stream.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>An Async Enumerable of <see cref="ChangeMessage"/>.</returns>
    public async IAsyncEnumerable<ChangeMessage> ReadLines([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var change in RunReadLoop(cancellationToken).ConfigureAwait(false))
            yield return change;
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

            yield return changeMessage;
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

            _stream = _tcpClient.GetAuthenticatedSslStream();
            _pipe = new Pipeline(_stream);
        }

        await Authenticate(cancellationToken).ConfigureAwait(false);

        if (_lastMarket is { } m)
        {
            _requestId++;
            await _pipe.WriteLine(new MarketSubscription(_requestId, m.Filter, m.DataFilter, m.Conflate, _lastInitialClk, _lastClk)).ConfigureAwait(false);
        }

        if (_lastOrder is { } o)
        {
            _requestId++;
            await _pipe.WriteLine(new OrderSubscription(_requestId, o.Filter, o.Conflate, _lastInitialClk, _lastClk)).ConfigureAwait(false);
        }
    }

    private async Task Authenticate(CancellationToken cancellationToken = default)
    {
        if (_isAuthenticated) return;

        _requestId++;
        var token = await _tokenProvider.GetToken(cancellationToken);
        var authMessage = new Authentication(_requestId, token, _appKey);

        await _pipe.WriteLine(authMessage);

        _isAuthenticated = true;
    }

    private sealed class LastMarketSubscription
    {
        public LastMarketSubscription(StreamMarketFilter filter, DataFilter? dataFilter, TimeSpan? conflate)
        {
            Filter = filter;
            DataFilter = dataFilter;
            Conflate = conflate;
        }

        public StreamMarketFilter Filter { get; }

        public DataFilter? DataFilter { get; }

        public TimeSpan? Conflate { get; }
    }

    private sealed class LastOrderSubscription
    {
        public LastOrderSubscription(StreamOrderFilter? filter, TimeSpan? conflate)
        {
            Filter = filter;
            Conflate = conflate;
        }

        public StreamOrderFilter? Filter { get; }

        public TimeSpan? Conflate { get; }
    }
}
