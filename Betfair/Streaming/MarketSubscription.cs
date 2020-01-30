namespace Betfair.Streaming
{
    using System;
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

        public StreamWriter Writer { get; private set; }

        public string ConnectionId { get; private set; }

        public bool Connected { get; private set; }

        public void WithTcpClient(ITcpClient client)
        {
            this.tcpClient = client;
            if (client is null) throw new ArgumentNullException(nameof(client));
        }

        public void Connect()
        {
            var stream = GetSslStream(this.tcpClient);
            this.Reader = new StreamReader(stream, Encoding.UTF8, false, this.tcpClient.ReceiveBufferSize);
            this.Writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        }

        public async Task Start()
        {
            while (true)
            {
                var line = await this.Reader.ReadLineAsync();
                if (line == null) return;
                this.ProcessLine(line);
            }
        }

        private static SslStream GetSslStream(ITcpClient client)
        {
            client.ReceiveBufferSize = 1024 * 1000 * 2;
            client.SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            client.ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            client.Connect(HostName, 443);

            Stream stream = client.GetStream();
            var sslStream = new SslStream(stream, false);
            sslStream.AuthenticateAsClient(HostName);
            return sslStream;
        }

        private static MessageOperation GetOperation(string line)
        {
            return JsonConvert.DeserializeObject<MessageOperation>(line);
        }

        private void ProcessLine(string line)
        {
            var operation = GetOperation(line);
            if (operation.IsConnectionMessage)
                this.ProcessConnectionMessage(line);
            if (operation.IsStatusMessage)
                this.ProcessStatusMessage(line);
        }

        private void ProcessConnectionMessage(string line)
        {
            var connectionMessage = JsonConvert.DeserializeObject<ConnectionMessage>(line);
            this.ConnectionId = connectionMessage.ConnectionId;
            this.Connected = true;
        }

        private void ProcessStatusMessage(string line)
        {
            var message = JsonConvert.DeserializeObject<StatusMessage>(line);
            if (message.ConnectionClosed)
            {
                this.ConnectionId = null;
                this.Connected = false;
            }
        }

        private sealed class ExchangeStreamClient : TcpClient, ITcpClient
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize Json.")]
        private sealed class MessageOperation
        {
            [JsonProperty(PropertyName = "op")]
            internal string Operation { get; set; }

            internal bool IsConnectionMessage => this.Operation == "connection";

            internal bool IsStatusMessage => this.Operation == "status";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize Json.")]
        private sealed class ConnectionMessage
        {
            [JsonProperty(PropertyName = "connectionId")]
            internal string ConnectionId { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize Json.")]
        private sealed class StatusMessage
        {
            [JsonProperty(PropertyName = "connectionClosed")]
            internal bool ConnectionClosed { get; set; }
        }
    }
}
