namespace Betfair.Stream
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using Identity;
    using Betfair.Stream.Responses;
    using Utf8Json;
    using Utf8Json.Resolvers;

    public sealed class Subscription
    {
        private const string HostName = "stream-api.betfair.com";
        private readonly ISession session;
        private readonly Dictionary<int, SubscriptionMessage> subscriptionMessages = new Dictionary<int, SubscriptionMessage>();
        private ITcpClient tcpClient = new ExchangeStreamClient();
        private int requestId;

        public Subscription(ISession session)
        {
            this.session = session;
        }

        public StreamReader Reader { get; set; }

        public StreamWriter Writer { get; set; }

        public bool Connected { get; private set; }

        public string ConnectionId { get; private set; }

        private Dictionary<string, Action<ChangeMessage>> ProcessMessageMap =>
            new Dictionary<string, Action<ChangeMessage>>
            {
                { "connection", this.ProcessConnectionMessage },
                { "status", this.ProcessStatusMessage },
            };

        public void WithTcpClient(ITcpClient client)
        {
            this.tcpClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void Connect()
        {
            var stream = GetSslStream(this.tcpClient);
            this.Reader = new StreamReader(stream, Encoding.UTF8, false, this.tcpClient.ReceiveBufferSize);
            this.Writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        }

        public async Task Authenticate()
        {
            var token = await this.session.GetTokenAsync();
            this.requestId++;
            var authMessage = GetAuthenticationMessage(this.session.AppKey, token, this.requestId);
            await this.Writer.WriteLineAsync(authMessage);
        }

        public async Task Subscribe(MarketFilter marketFilter, MarketDataFilter dataFilter)
        {
            this.requestId++;
            var subscriptionMessage = new SubscriptionMessage("marketSubscription", this.requestId)
                .WithMarketFilter(marketFilter)
                .WithMarketDateFilter(dataFilter);
            await this.Writer.WriteLineAsync(subscriptionMessage.ToJson());
            this.subscriptionMessages.Add(this.requestId, subscriptionMessage);
        }

        public async Task SubscribeToOrders()
        {
            this.requestId++;
            var subscriptionMessage = new SubscriptionMessage("orderSubscription", this.requestId);
            await this.Writer.WriteLineAsync(subscriptionMessage.ToJson());
            this.subscriptionMessages.Add(this.requestId, subscriptionMessage);
        }

        public async Task Resubscribe()
        {
            foreach (var m in this.subscriptionMessages)
                await this.Writer.WriteLineAsync(m.Value.ToJson());
        }

        public async IAsyncEnumerable<ChangeMessage> GetChanges()
        {
            string line;
            while ((line = await this.Reader.ReadLineAsync()) != null)
            {
                var message = JsonSerializer.Deserialize<ChangeMessage>(line);
                this.ProcessMessage(message);
                yield return message;
            }
        }

        public void Stop()
        {
            this.Connected = false;
            this.tcpClient.Close();
        }

        private static Stream GetSslStream(ITcpClient client)
        {
            client.ReceiveBufferSize = 1024 * 1000 * 2;
            client.SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            client.ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            client.Connect(HostName, 443);

            return client.GetSslStream(HostName);
        }

        private static string GetAuthenticationMessage(string appKey, string token, int requestId)
        {
            return $"{{\"op\":\"authentication\",\"id\":{requestId},\"session\":\"{token}\",\"appKey\":\"{appKey}\"}}";
        }

        private void ProcessMessage(ChangeMessage message)
        {
            message.SetArrivalTime(DateTime.UtcNow);
            if (this.ProcessMessageMap.ContainsKey(message.Operation))
                this.ProcessMessageMap[message.Operation](message);
            this.SetClocks(message);
        }

        private void ProcessConnectionMessage(ChangeMessage message)
        {
            this.ConnectionId = message.ConnectionId;
            this.Connected = true;
        }

        private void ProcessStatusMessage(ChangeMessage message)
        {
            if (message.ConnectionClosed != null)
                this.Connected = message.ConnectionClosed == false;
        }

        private void SetClocks(ChangeMessage message)
        {
            if (message.Id == null) return;
            if (!this.subscriptionMessages.ContainsKey((int)message.Id)) return;
            this.subscriptionMessages[(int)message.Id]
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
                this.Operation = operation;
                this.Id = id;
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
                    this.MarketFilter = marketFilter;
                return this;
            }

            internal SubscriptionMessage WithMarketDateFilter(MarketDataFilter marketDataFilter)
            {
                if (marketDataFilter != null)
                    this.MarketDataFilter = marketDataFilter;
                return this;
            }

            internal SubscriptionMessage WithInitialClock(string initialClock)
            {
                if (initialClock == null) return this;
                this.InitialClock = initialClock;
                return this;
            }

            internal void WithClock(string clock)
            {
                this.Clock = clock;
            }

            internal string ToJson()
            {
                return JsonSerializer.ToJsonString(this, StandardResolver.AllowPrivateExcludeNull);
            }
        }

        private sealed class ExchangeStreamClient : TcpClient, ITcpClient
        {
            public Stream GetSslStream(string host)
            {
                var stream = this.GetStream();
                var sslStream = new SslStream(stream, false);
                sslStream.AuthenticateAsClient(host);
                return sslStream;
            }
        }
    }
}
