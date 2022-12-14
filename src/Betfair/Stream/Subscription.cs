using System.Runtime.Serialization;
using Betfair.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair.Stream;

public sealed class Subscription
{
    private readonly StreamClient _client;
    private readonly Credentials _credentials;
    private readonly Dictionary<int, SubscriptionMessage> _subscriptionMessages = new Dictionary<int, SubscriptionMessage>();
    private int _requestId;

    public Subscription(StreamClient client, Credentials credentials)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
    }

    public bool Connected { get; private set; }

    public string ConnectionId { get; private set; } = string.Empty;

    private Dictionary<string, Action<ChangeMessage>> ProcessMessageMap =>
        new ()
        {
            { "connection", ProcessConnectionMessage },
            { "status", ProcessStatusMessage },
        };

    public async Task<UnitResult<ErrorResult>> Authenticate(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return ErrorResult.Empty(nameof(token));

        _requestId++;
        var authMessage = new Authentication
        {
            Id = _requestId,
            AppKey = _credentials.AppKey,
            Session = token,
        };

        await _client.SendLine(authMessage);
        var result = await _client.ReadLine<StatusMessage>();
        if (result.IsFailure) return result.Error;
        if (result.Value.Value.StatusCode.Equals("FAILURE", StringComparison.OrdinalIgnoreCase))
            return ErrorResult.Create(result.Value.Value.ErrorCode!);

        return UnitResult.Success<ErrorResult>();
    }

    public async Task Subscribe(MarketFilter marketFilter, MarketDataFilter dataFilter)
    {
        _requestId++;
        var subscriptionMessage = new SubscriptionMessage("marketSubscription", _requestId)
            .WithMarketFilter(marketFilter)
            .WithMarketDataFilter(dataFilter);

        await _client.SendLine(subscriptionMessage);
        _subscriptionMessages.Add(_requestId, subscriptionMessage);
    }

    public async Task SubscribeToOrders()
    {
        _requestId++;
        var subscriptionMessage = new SubscriptionMessage("orderSubscription", _requestId);
        await _client.SendLine(subscriptionMessage);
        _subscriptionMessages.Add(_requestId, subscriptionMessage);
    }

    public async Task Resubscribe()
    {
        foreach (var m in _subscriptionMessages)
            await _client.SendLine(m.Value);
    }

    public async IAsyncEnumerable<ChangeMessage> GetChanges()
    {
        while (await _client.ReadLine<ChangeMessage>() is { } line)
        {
            if (line.Value.HasNoValue) break;
            ProcessMessage(line.Value.Value);
            yield return line.Value.Value;
        }
    }

    public void Disconnect()
    {
        Connected = false;
        _client.Disconnect();
    }

    private static string GetAuthenticationMessage(string appKey, string token, int requestId)
    {
        return $"{{\"op\":\"authentication\",\"id\":{requestId},\"session\":\"{token}\",\"appKey\":\"{appKey}\"}}";
    }

    private void ProcessMessage(ChangeMessage message)
    {
        message.SetArrivalTime(DateTime.UtcNow);
        if (ProcessMessageMap.ContainsKey(message.Operation))
            ProcessMessageMap[message.Operation](message);
        SetClocks(message);
    }

    private void ProcessConnectionMessage(ChangeMessage message)
    {
        ConnectionId = message.ConnectionId;
        Connected = true;
    }

    private void ProcessStatusMessage(ChangeMessage message)
    {
        if (message.ConnectionClosed != null)
            Connected = message.ConnectionClosed == false;
    }

    private void SetClocks(ChangeMessage message)
    {
        if (message.Id == null) return;
        if (!_subscriptionMessages.ContainsKey((int)message.Id)) return;
        _subscriptionMessages[(int)message.Id]
            .WithInitialClock(message.InitialClock)
            .WithClock(message.Clock);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance",
        "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "Used to deserialize Json response.")]
    [DataContract]
    private sealed class SubscriptionMessage
    {
        internal SubscriptionMessage(string operation, int id)
        {
            Operation = operation;
            Id = id;
        }

        [DataMember(Name = "op", EmitDefaultValue = false)]
        internal string Operation { get; private set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        internal int Id { get; private set; }

        [DataMember(Name = "marketFilter", EmitDefaultValue = false)]
        internal MarketFilter MarketFilter { get; private set; }

        [DataMember(Name = "marketDataFilter", EmitDefaultValue = false)]
        internal MarketDataFilter MarketDataFilter { get; private set; }

        [DataMember(Name = "initialClk", EmitDefaultValue = false)]
        internal string InitialClock { get; private set; }

        [DataMember(Name = "clk", EmitDefaultValue = false)]
        internal string Clock { get; private set; }

        internal SubscriptionMessage WithMarketFilter(MarketFilter marketFilter)
        {
            if (marketFilter != null)
                MarketFilter = marketFilter;
            return this;
        }

        internal SubscriptionMessage WithMarketDataFilter(MarketDataFilter marketDataFilter)
        {
            if (marketDataFilter != null)
                MarketDataFilter = marketDataFilter;
            return this;
        }

        internal SubscriptionMessage WithInitialClock(string initialClock)
        {
            if (initialClock == null) return this;
            InitialClock = initialClock;
            return this;
        }

        internal void WithClock(string clock)
        {
            Clock = clock;
        }
    }
}