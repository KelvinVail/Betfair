namespace Betfair.Streaming
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public sealed class StreamSubscription
    {
        private const string HostName = "stream-api.betfair.com";
        private readonly ISession session;
        private readonly List<string> subscriptionMessages = new List<string>();
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

        private Dictionary<string, Action<string>> ProcessMessageMap =>
            new Dictionary<string, Action<string>>
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
            var subscriptionMessage = GetMarketSubscriptionMessage(marketFilter, dataFilter, this.requestId);
            await this.Writer.WriteLineAsync(subscriptionMessage);
            this.subscriptionMessages.Add(subscriptionMessage);
        }

        public async Task SubscribeToOrders()
        {
            this.requestId++;
            var subscriptionMessage = $"{{\"op\":\"orderSubscription\",\"id\":{this.requestId}}}";
            await this.Writer.WriteLineAsync(subscriptionMessage);
            this.subscriptionMessages.Add(subscriptionMessage);
        }

        public async Task Resubscribe()
        {
            foreach (var subscriptionMessage in this.subscriptionMessages)
            {
                await this.Writer.WriteLineAsync(subscriptionMessage);
            }
        }

        public async IAsyncEnumerable<string> GetChanges()
        {
            string line;
            while ((line = await this.Reader.ReadLineAsync()) != null)
            {
                this.ProcessLine(line);
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

        private static string GetMarketSubscriptionMessage(MarketFilter marketFilter, MarketDataFilter dataFilter, int requestId)
        {
            var operation = $"{{\"op\":\"marketSubscription\",\"id\":{requestId}";
            if (marketFilter != null) operation += ",\"marketFilter\":" + JsonConvert.SerializeObject(marketFilter);
            if (dataFilter != null) operation += ",\"marketDataFilter\":" + JsonConvert.SerializeObject(dataFilter);
            return operation += "}";
        }

        private static string GetOperation(string line)
        {
            return line.Split(",")[0].Split(":")[1].Replace("\"", string.Empty, StringComparison.CurrentCulture);
        }

        private void ProcessLine(string line)
        {
            var operation = GetOperation(line);
            if (this.ProcessMessageMap.ContainsKey(operation))
                this.ProcessMessageMap[operation](line);
        }

        private void ProcessConnectionMessage(string line)
        {
            var split = line.Split(",");
            foreach (var p in split)
            {
                if (!p.Contains("connectionId", StringComparison.CurrentCulture)) continue;
                this.ConnectionId = p.Split(":")[1]
                    .Replace("\"", string.Empty, StringComparison.CurrentCulture)
                    .Replace("}", string.Empty, StringComparison.CurrentCulture);
                break;
            }

            this.Connected = true;
        }

        private void ProcessStatusMessage(string line)
        {
            var split = line.Split(",");
            foreach (var p in split)
            {
                if (!p.Contains("connectionClosed", StringComparison.CurrentCulture)) continue;
                this.Connected = !p.Contains("true", StringComparison.CurrentCulture);
                break;
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
