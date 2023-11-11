using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Stream;

public class StreamClient : IDisposable
{
    private readonly Pipeline _pipe;
    private int _requestId;
    private bool _disposedValue;

    public StreamClient() =>
        _pipe = new Pipeline();

    public StreamClient(System.IO.Stream stream) =>
        _pipe = new Pipeline(stream);

    public Task Authenticate(string appKey, string sessionToken)
    {
        _requestId++;
        var authMessage = new Authentication(_requestId, sessionToken, appKey);

        return _pipe.Write(authMessage);
    }

    public Task Subscribe(MarketFilter marketFilter, DataFilter dataFilter)
    {
        _requestId++;
        var marketSubscription = new MarketSubscription(
            _requestId,
            marketFilter,
            dataFilter);

        return _pipe.Write(marketSubscription);
    }

    public Task SubscribeToOrders()
    {
        _requestId++;

        return _pipe.Write(new OrderSubscription(_requestId));
    }

    public async IAsyncEnumerable<ChangeMessage> GetChanges()
    {
        await foreach (var line in _pipe.Read())
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
        if (disposing) _pipe.Dispose();

        _disposedValue = true;
    }
}
