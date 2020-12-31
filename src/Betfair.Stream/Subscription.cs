using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Betfair.Identity;
using Betfair.Stream.Responses;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Stream
{
    public sealed class Subscription : ISubscription
    {
        private const string _hostName = "stream-api.betfair.com";
        private readonly ISession _session;
        private readonly Dictionary<int, SubscriptionMessage> _subscriptionMessages = new Dictionary<int, SubscriptionMessage>();
        private ITcpClient _tcpClient = new ExchangeStreamClient();
        private int _requestId;

        public Subscription(ISession session)
        {
            _session = session;
        }

        public StreamReader Reader { get; set; }

        public StreamWriter Writer { get; set; }

        public bool Connected { get; private set; }

        public string ConnectionId { get; private set; }

        private Dictionary<string, Action<ChangeMessage>> ProcessMessageMap =>
            new Dictionary<string, Action<ChangeMessage>>
            {
                { "connection", ProcessConnectionMessage },
                { "status", ProcessStatusMessage },
            };

        public void WithTcpClient(ITcpClient client)
        {
            _tcpClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void Connect()
        {
            var stream = GetSslStream(_tcpClient);
            Reader = new StreamReader(stream, Encoding.UTF8, false, _tcpClient.ReceiveBufferSize);
            Writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        }

        public async Task Authenticate()
        {
            var token = await _session.GetTokenAsync();
            _requestId++;
            var authMessage = GetAuthenticationMessage(_session.AppKey, token, _requestId);
            await Writer.WriteLineAsync(authMessage);
        }

        public async Task Subscribe(MarketFilter marketFilter, MarketDataFilter dataFilter)
        {
            _requestId++;
            var subscriptionMessage = new SubscriptionMessage("marketSubscription", _requestId)
                .WithMarketFilter(marketFilter)
                .WithMarketDataFilter(dataFilter);
            await Writer.WriteLineAsync(subscriptionMessage.ToJson());
            _subscriptionMessages.Add(_requestId, subscriptionMessage);
        }

        public async Task SubscribeToOrders()
        {
            _requestId++;
            var subscriptionMessage = new SubscriptionMessage("orderSubscription", _requestId);
            await Writer.WriteLineAsync(subscriptionMessage.ToJson());
            _subscriptionMessages.Add(_requestId, subscriptionMessage);
        }

        public async Task Resubscribe()
        {
            foreach (var m in _subscriptionMessages)
                await Writer.WriteLineAsync(m.Value.ToJson());
        }

        public async IAsyncEnumerable<ChangeMessage> GetChanges()
        {
            string line;
            while ((line = await Reader.ReadLineAsync()) != null)
            {
                var message = JsonSerializer.Deserialize<ChangeMessage>(line);
                ProcessMessage(message);
                yield return message;
            }
        }

        public void Disconnect()
        {
            Connected = false;
            _tcpClient.Close();
        }

        private static System.IO.Stream GetSslStream(ITcpClient client)
        {
            client.ReceiveBufferSize = 1024 * 1000 * 2;
            client.SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            client.ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            client.Connect(_hostName, 443);

            return client.GetSslStream(_hostName);
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

            internal string ToJson()
            {
                return JsonSerializer.ToJsonString(this, StandardResolver.AllowPrivateExcludeNull);
            }
        }

        private sealed class ExchangeStreamClient : TcpClient, ITcpClient
        {
            public System.IO.Stream GetSslStream(string host)
            {
                var stream = GetStream();
                var sslStream = new SslStream(stream, false);
                sslStream.AuthenticateAsClient(host);
                return sslStream;
            }
        }
    }
}
