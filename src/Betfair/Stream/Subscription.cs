using System.Runtime.Serialization;
using Betfair.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair.Stream;

public sealed class Subscription
{
    private readonly Pipeline _client;
    private readonly Credentials _credentials;
    private bool _connected;
    private int _requestId;

    public Subscription(Pipeline client, Credentials credentials)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
    }

    public UnitResult<ErrorResult> Status { get; private set; }

    private Dictionary<string, Action<ChangeMessage>> ProcessMessageMap =>
        new ()
        {
            { "connection", ProcessConnectionMessage },
            { "status", ProcessStatusMessage },
        };

    public Task Authenticate(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            Status = ErrorResult.Empty(nameof(token));
            return Task.CompletedTask;
        }

        _requestId++;
        var authMessage = new Authentication
        {
            Id = _requestId,
            AppKey = _credentials.AppKey,
            Session = token,
        };

        Status = UnitResult.Success<ErrorResult>();
        return _client.Write(authMessage);
    }

    public Task Subscribe(StreamMarketFilter marketFilter, MarketDataFilter dataFilter)
    {
        if (marketFilter is null) Status = ErrorResult.Empty(nameof(marketFilter));
        if (dataFilter is null) Status = ErrorResult.Empty(nameof(dataFilter));
        if (Status.IsFailure) return Task.CompletedTask;

        _requestId++;
        var subscriptionMessage = new SubscriptionMessage
        {
            Op = "marketSubscription",
            Id = _requestId,
            MarketFilter = marketFilter,
            MarketDataFilter = dataFilter,
        };

        return _client.Write(subscriptionMessage);
    }

    public Task SubscribeToOrders()
    {
        _requestId++;
        var subscriptionMessage = new SubscriptionMessage
        {
            Op = "orderSubscription",
            Id = _requestId,
        };

        return _client.Write(subscriptionMessage);
    }

    public async IAsyncEnumerable<ChangeMessage> GetChanges()
    {
        await foreach (var line in _client.Read())
        {
            if (line is null) break;
            ProcessMessage(line);
            yield return line;
        }
    }

    private void ProcessMessage(ChangeMessage message)
    {
        if (ProcessMessageMap.ContainsKey(message.Operation))
            ProcessMessageMap[message.Operation](message);
    }

    private void ProcessConnectionMessage(ChangeMessage message)
    {
        if (message.StatusCode is null || message.StatusCode.Equals("FAILURE", StringComparison.OrdinalIgnoreCase))
        {
            Status = ErrorResult.Create(message.ErrorCode ?? "CONNECTION_ERROR");
            return;
        }

        _connected = true;
        Status = UnitResult.Success<ErrorResult>();
    }

    private void ProcessStatusMessage(ChangeMessage message)
    {
        if (message.ConnectionClosed != null)
            _connected = message.ConnectionClosed == false;
    }
}