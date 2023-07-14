using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Stream;

public class StreamClient : IDisposable
{
    private readonly Pipeline _client;
    private int _requestId;
    private bool _disposedValue;

    public StreamClient() =>
        _client = new Pipeline();

    public StreamClient(System.IO.Stream stream) =>
        _client = new Pipeline(stream);

    public Task Authenticate(string appKey, string sessionToken)
    {
        _requestId++;
        var authMessage = new Authentication(_requestId, sessionToken, appKey);

        return _client.Write(authMessage);
    }

    public Task Subscribe(MarketFilter marketFilter, DataFilter dataFilter)
    {
        _requestId++;
        var marketSubscription = new MarketSubscription(
            _requestId,
            marketFilter,
            dataFilter);

        return _client.Write(marketSubscription);
    }

    public Task SubscribeToOrders()
    {
        _requestId++;

        return _client.Write(new OrderSubscription(_requestId));
    }

    public async IAsyncEnumerable<ChangeMessage> GetChanges()
    {
        await foreach (var line in _client.Read())
        {
            var received = DateTimeOffset.UtcNow.Ticks;
            var changeMessage = JsonSerializer.Deserialize<ChangeMessage>(line, StandardResolver.ExcludeNullCamelCase);
            changeMessage.ReceivedTick = received;
            changeMessage.DeserializedTick = DateTimeOffset.UtcNow.Ticks;
            yield return changeMessage;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _client.Dispose();

        _disposedValue = true;
    }
}
