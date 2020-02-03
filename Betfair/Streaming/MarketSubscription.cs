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

    public sealed class MarketSubscription
    {
        private const string HostName = "stream-api.betfair.com";
        private readonly ISession session;
        private ITcpClient tcpClient = new ExchangeStreamClient();

        public MarketSubscription(ISession session)
        {
            this.session = session;
        }

        public StreamReader Reader { get; set; }

        public StreamWriter Writer { get; set; }

        public bool Connected { get; private set; }

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
            var authMessage = GetAuthenticationMessage(this.session.AppKey, token);
            await this.Writer.WriteLineAsync(authMessage);
        }

        public async Task Start()
        {
            var line = "not null";
            while (line != null)
            {
                line = await this.Reader.ReadLineAsync();
                this.ProcessLine(line);
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

        private static string GetAuthenticationMessage(string appKey, string token)
        {
            return $"{{\"op\":\"authentication\",\"id\":1,\"session\":\"{token}\",\"appKey\":\"{appKey}\"}}";
        }

        private void ProcessLine(string line)
        {
            if (line is null) return;
            var message = JsonConvert.DeserializeObject<ResponseMessage>(line);
            this.ProcessMessageMap[message.Operation](message);
        }

        private void ProcessConnectionMessage(ResponseMessage message)
        {
            this.Connected = true;
        }

        private void ProcessStatusMessage(ResponseMessage message)
        {
            this.Connected = !message.ConnectionClosed;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize Json.")]
        private sealed class ResponseMessage
        {
            [JsonProperty(PropertyName = "op")]
            internal string Operation { get; private set; }

            [JsonProperty(PropertyName = "connectionClosed")]
            internal bool ConnectionClosed { get; private set; }
        }
    }
}
