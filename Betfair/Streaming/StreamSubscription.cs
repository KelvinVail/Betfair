namespace Betfair.Streaming
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using Utf8Json;
    using Utf8Json.Resolvers;

    public sealed class StreamSubscription
    {
        private const string HostName = "stream-api.betfair.com";
        private readonly ISession session;
        private readonly Dictionary<int, SubscriptionMessage> subscriptionMessages = new Dictionary<int, SubscriptionMessage>();
        private ITcpClient tcpClient = new ExchangeStreamClient();
        private int requestId;

        public StreamSubscription(ISession session)
        {
            this.session = session;
        }

        public StreamReader Reader { get; set; }

        public StreamWriter Writer { get; set; }

        public bool Connected { get; private set; }

        public string ConnectionId { get; private set; }

        private Dictionary<string, Action<ResponseMessage>> ProcessMessageMap =>
            new Dictionary<string, Action<ResponseMessage>>
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

        public async Task AuthenticateAsync()
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
            {
                await this.Writer.WriteLineAsync(m.Value.ToJson());
            }
        }

        public async IAsyncEnumerable<string> GetChanges()
        {
            string line;
            while ((line = await this.Reader.ReadLineAsync()) != null)
            {
                var message = Utf8Json.JsonSerializer.Deserialize<ResponseMessage>(line);
                this.ProcessLine(message);
                this.SetInitialClock(message);
                yield return line;
            }
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

        private void ProcessLine(ResponseMessage message)
        {
            if (this.ProcessMessageMap.ContainsKey(message.Operation))
                this.ProcessMessageMap[message.Operation](message);
        }

        private void ProcessConnectionMessage(ResponseMessage message)
        {
            this.ConnectionId = message.ConnectionId;
            this.Connected = true;
        }

        private void ProcessStatusMessage(ResponseMessage message)
        {
            this.Connected = !message.ConnectionClosed;
        }

        private void SetInitialClock(ResponseMessage message)
        {
            if (!this.subscriptionMessages.ContainsKey(message.Id)) return;
            var m = this.subscriptionMessages[message.Id];
            this.subscriptionMessages[message.Id] = m.WithInitialClock(message.InitialClock);
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
                this.InitialClock = initialClock;
                return this;
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
