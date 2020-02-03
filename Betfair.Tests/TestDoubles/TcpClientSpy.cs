namespace Betfair.Tests.TestDoubles
{
    using System.IO;
    using Betfair.Streaming;

    public sealed class TcpClientSpy : ITcpClient
    {
        public TcpClientSpy(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public int ReceiveBufferSize { get; set; }

        public int SendTimeout { get; set; }

        public int ReceiveTimeout { get; set; }

        public void Connect(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }

        public Stream GetSslStream(string host)
        {
            var ms = new MemoryStream();
            return ms;
        }
    }
}
