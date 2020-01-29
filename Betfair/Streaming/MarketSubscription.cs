namespace Betfair.Streaming
{
    using System;
    using System.IO;
    using System.Net.Security;
    using System.Text;

    public sealed class MarketSubscription
    {
        private const string HostName = "stream-api.betfair.com";
        private readonly ISession session;

        public MarketSubscription(ISession session)
        {
            this.session = session;
        }

        public StreamReader Reader { get; private set; }

        public StreamWriter Writer { get; private set; }

        public void WithTcpClient(ITcpClient client)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            var stream = GetSslStream(client);
            this.Reader = new StreamReader(stream, Encoding.UTF8, false, client.ReceiveBufferSize);
            this.Writer = new StreamWriter(stream, Encoding.UTF8);
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
    }
}
