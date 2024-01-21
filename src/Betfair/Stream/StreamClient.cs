using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair.Stream;

/// <summary>
/// Used to subscribe to Betfair market and order streams.
/// </summary>
public class StreamClient : IDisposable
{
    private readonly Pipeline _pipe;
    private readonly System.IO.Stream _stream = new BetfairTcpClient().GetAuthenticatedSslStream();
    private readonly HttpClient _httpClient;
    private readonly TokenProvider _provider;
    private string _appKey;
    private int _requestId;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamClient"/> class.
    /// Used to subscribe to Betfair market and order streams.
    /// </summary>
    /// <param name="credentials">The <see cref="Credentials"/> object need to authenticate with Betfair.</param>
    public StreamClient(Credentials credentials)
    {
        _httpClient = new BetfairHttpClient(credentials.Certificate);
        _provider = new TokenProvider(_httpClient, credentials);
        _appKey = credentials.AppKey;
        _pipe = new Pipeline(_stream);
    }

    //internal StreamClient(System.IO.Stream stream, HttpClient client)
    //{
    //    _pipe = new Pipeline(stream);
    //    _httpClient = client;
    //}

    /// <summary>
    /// Authenticates the StreamClient with Betfair. Only needs to be performed once on StreamClient creation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>An awaitable task.</returns>
    public async Task Authenticate(CancellationToken cancellationToken = default)
    {
        // TODO Hide this method call.
        _requestId++;
        var token = await _provider.GetToken(cancellationToken);
        var authMessage = new Authentication(_requestId, token, _appKey);

        await _pipe.Write(authMessage);
    }

    /// <summary>
    /// Define and subscribe to a market stream.
    /// </summary>
    /// <param name="marketFilter">Used to define which markets to subscribe to.</param>
    /// <param name="dataFilter">Used to define what data to include in the market stream.</param>
    /// <returns>An awaitable task.</returns>
    public async Task Subscribe(StreamMarketFilter marketFilter, DataFilter dataFilter)
    {
        // TODO Add cancellation support.
        _requestId++;
        var marketSubscription = new MarketSubscription(
            _requestId,
            marketFilter,
            dataFilter);

        await _pipe.Write(marketSubscription);
    }

    /// <summary>
    /// Subscribe to new and open orders. To retrieve historical orders use the <see cref="Api.BetfairApiClient"/> class.
    /// </summary>
    /// <returns>An awaitable task.</returns>
    public Task SubscribeToOrders()
    {
        // TODO Add cancellation support.
        _requestId++;
        return _pipe.Write(new OrderSubscription(_requestId));
    }

    /// <summary>
    /// Asynchronously iterate ChangeMessages as they become available on the stream.
    /// </summary>
    /// <returns>An Async Enumerable of <see cref="ChangeMessage"/>.</returns>
    public async IAsyncEnumerable<ChangeMessage> GetChanges()
    {
        // TODO Rename GetChanges and ReadLines
        await foreach (var line in _pipe.Read())
        {
            var received = DateTimeOffset.UtcNow.Ticks;
            var changeMessage = JsonSerializer.Deserialize<ChangeMessage>(line, StandardResolver.ExcludeNullCamelCase);
            changeMessage.ReceivedTick = received;
            changeMessage.DeserializedTick = DateTimeOffset.UtcNow.Ticks;
            yield return changeMessage;
        }
    }

    /// <summary>
    /// Asynchronously iterate the raw byte arrays as they become available on the stream.
    /// </summary>
    /// <returns>An Async Enumerable of <see cref="byte[]"/>.</returns>
    public IAsyncEnumerable<byte[]> ReadLines() =>
        _pipe.Read();

    // TODO Remove this method
    public async Task CopyToStream([NotNull] System.IO.Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var line in _pipe.Read().WithCancellation(cancellationToken))
            {
                await stream.WriteAsync(line, cancellationToken);
                stream.WriteByte((byte)'\n');
            }
        }
        finally
        {
            await stream.FlushAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Close the stream to stop receiving change messages.
    /// </summary>
    public void Close() =>
        _stream.Close();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            _stream.Dispose();
            _httpClient.Dispose();
        }

        _disposedValue = true;
    }
}
