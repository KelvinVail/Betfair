using System.IO;

namespace Betfair.Stream.Tests.TestDoubles
{
    public sealed class TcpClientSpy : ITcpClient
    {
        public TcpClientSpy(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public int ReceiveBufferSize { get; set; }

        public int SendTimeout { get; set; }

        public int ReceiveTimeout { get; set; }

        public int TimeGetSslStreamCalled { get; set; }

        public bool Connected { get; set; }

        public void Connect(string host, int port)
        {
            Host = host;
            Port = port;
            Connected = true;
        }

        public void Close()
        {
            Connected = false;
        }

        public System.IO.Stream GetSslStream(string host)
        {
            TimeGetSslStreamCalled++;
            var ms = new MemoryStream();
            return ms;
        }
    }
}
