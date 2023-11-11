﻿using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Stream;

public class StreamClient : IDisposable
{
    private readonly Pipeline _pipe;
    private readonly BetfairHttpClient _httpClient;
    private int _requestId;
    private bool _disposedValue;

    public StreamClient(Credentials credentials)
    {
        _pipe = new Pipeline();
        _httpClient = new BetfairHttpClient(credentials);
    }

    public StreamClient(System.IO.Stream stream, BetfairHttpClient client)
    {
        _pipe = new Pipeline(stream);
        _httpClient = client;
    }

    public async Task Authenticate(CancellationToken cancellationToken = default)
    {
        _requestId++;
        var token = await _httpClient.GetToken(cancellationToken);
        var authMessage = new Authentication(_requestId, token, _httpClient.AppKey);

        await _pipe.Write(authMessage);
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
        if (disposing)
        {
            _pipe.Dispose();
            _httpClient.Dispose();
        }

        _disposedValue = true;
    }
}
