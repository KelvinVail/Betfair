using System.Runtime.CompilerServices;
using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair.Stream;

/// <summary>
/// Used to subscribe to Betfair market and order streams.
/// </summary>
public class Subscription : IDisposable
{
    private readonly BetfairTcpClient _tcpClient = new ();
    private readonly System.IO.Stream? _stream;
    private readonly IPipeline _pipe;
    private readonly TokenProvider _tokenProvider;
    private readonly string _appKey;
    private readonly BetfairHttpClient? _httpClient;
    private int _requestId;
    private bool _isAuthenticated;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Subscription"/> class.
    /// Used to subscribe to Betfair market and order streams.
    /// </summary>
    /// <param name="credentials">The <see cref="Credentials"/> object need to authenticate with Betfair.</param>
    public Subscription(Credentials credentials)
    {
        ArgumentNullException.ThrowIfNull(credentials);

        _appKey = credentials.AppKey;
        _httpClient = new BetfairHttpClient(credentials.Certificate);
        _tokenProvider = new TokenProvider(_httpClient, credentials);
        _stream = _tcpClient.GetAuthenticatedSslStream();
        _pipe = new Pipeline(_stream);
    }

    internal Subscription(TokenProvider tokenProvider, string appKey, IPipeline pipe)
    {
        _tokenProvider = tokenProvider;
        _appKey = appKey;
        _pipe = pipe;
    }

    /// <summary>
    /// Subscribe to a market stream.
    /// </summary>
    /// <param name="marketFilter">Used to define which markets to subscribe to.</param>
    /// <param name="dataFilter">Optional: Used to define what data to include in the market stream.
    /// If null Best Available Prices are returned.</param>
    /// <param name="conflate">Optional: Data will be rolled up and sent on each increment of this time interval.</param>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>An awaitable task.</returns>
    public async Task Subscribe(
        StreamMarketFilter marketFilter,
        DataFilter? dataFilter = null,
        TimeSpan? conflate = null,
        CancellationToken cancellationToken = default)
    {
        await Authenticate(cancellationToken);

        _requestId++;
        var marketSubscription = new MarketSubscription(_requestId, marketFilter, dataFilter, conflate);
        await _pipe.WriteLine(marketSubscription);
    }

    /// <summary>
    /// Subscribe to new and open orders. To retrieve historical orders use the <see cref="Api.BetfairApiClient"/> class.
    /// </summary>
    /// <param name="orderFilter">Optional: Used to shape and filter the order data returned on the stream.</param>
    /// <param name="conflate">Optional: Data will be rolled up and sent on each increment of this time interval.</param>
    /// <returns>An awaitable task.</returns>
    public async Task SubscribeToOrders(OrderFilter? orderFilter = null, TimeSpan? conflate = null, CancellationToken cancellationToken = default)
    {
        await Authenticate(cancellationToken);

        _requestId++;
        await _pipe.WriteLine(new OrderSubscription(_requestId, orderFilter, conflate));
    }

    /// <summary>
    /// Asynchronously iterate ChangeMessages as they become available on the stream.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken.</param>
    /// <returns>An Async Enumerable of <see cref="ChangeMessage"/>.</returns>
    public async IAsyncEnumerable<ChangeMessage> ReadLines([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var line in _pipe.ReadLines(cancellationToken))
        {
            var received = DateTimeOffset.UtcNow.Ticks;
            var changeMessage = JsonSerializer.Deserialize<ChangeMessage>(line, StandardResolver.ExcludeNullCamelCase);
            changeMessage.ReceivedTick = received;
            changeMessage.DeserializedTick = DateTimeOffset.UtcNow.Ticks;
            yield return changeMessage;
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

    private async Task Authenticate(CancellationToken cancellationToken = default)
    {
        if (_isAuthenticated) return;

        _requestId++;
        var token = await _tokenProvider.GetToken(cancellationToken);
        var authMessage = new Authentication(_requestId, token, _appKey);

        await _pipe.WriteLine(authMessage);

        _isAuthenticated = true;
    }
}
