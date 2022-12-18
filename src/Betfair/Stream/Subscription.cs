using System.Runtime.Serialization;
using Betfair.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair.Stream;

public sealed class Subscription
{
    private readonly Pipeline _client;
    private readonly Credentials _credentials;
    private readonly Dictionary<int, SubscriptionMessage> _subscriptionMessages = new Dictionary<int, SubscriptionMessage>();
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

    public async Task Subscribe(StreamMarketFilter streamMarketFilter, MarketDataFilter dataFilter)
    {
        _requestId++;
        var subscriptionMessage = new SubscriptionMessage("marketSubscription", _requestId)
            .WithMarketFilter(streamMarketFilter)
            .WithMarketDataFilter(dataFilter);

        await _client.Write(subscriptionMessage);
        _subscriptionMessages.Add(_requestId, subscriptionMessage);
    }

    public async Task SubscribeToOrders()
    {
        _requestId++;
        var subscriptionMessage = new SubscriptionMessage("orderSubscription", _requestId);
        await _client.Write(subscriptionMessage);
        _subscriptionMessages.Add(_requestId, subscriptionMessage);
    }

    public async Task Resubscribe()
    {
        foreach (var m in _subscriptionMessages)
            await _client.Write(m.Value);
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
        SetClocks(message);
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

    private void SetClocks(ChangeMessage message)
    {
        if (message.Id == null) return;
        if (!_subscriptionMessages.ContainsKey((int)message.Id)) return;
        _subscriptionMessages[(int)message.Id]
            .WithInitialClock(message.InitialClock)
            .WithClock(message.Clock);
    }

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
        internal StreamMarketFilter StreamMarketFilter { get; private set; }

        [DataMember(Name = "marketDataFilter", EmitDefaultValue = false)]
        internal MarketDataFilter MarketDataFilter { get; private set; }

        [DataMember(Name = "initialClk", EmitDefaultValue = false)]
        internal string InitialClock { get; private set; }

        [DataMember(Name = "clk", EmitDefaultValue = false)]
        internal string Clock { get; private set; }

        internal SubscriptionMessage WithMarketFilter(StreamMarketFilter streamMarketFilter)
        {
            if (streamMarketFilter != null)
                StreamMarketFilter = streamMarketFilter;
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