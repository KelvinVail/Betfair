namespace Betfair.Tests.TestDoubles
{
    using System.Net.Sockets;
    using Betfair.Streaming;

    public class TcpClientSpy : ITcpClient
    {
        private NetworkStream networkStream;

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

        public NetworkStream GetStream()
        {
            return this.networkStream;
        }

        public TcpClientSpy WithStream(NetworkStream stream)
        {
            this.networkStream = stream;
            return this;
        }

        public void Dispose()
        {
        }
    }
}
