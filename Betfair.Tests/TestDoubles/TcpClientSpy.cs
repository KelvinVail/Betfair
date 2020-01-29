namespace Betfair.Tests.TestDoubles
{
    using System;
    using System.Net.Sockets;
    using Betfair.Streaming;

    public sealed class TcpClientSpy : ITcpClient, IDisposable
    {
        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private readonly NetworkStream networkStream;

        public TcpClientSpy(string host)
        {
            this.socket.Connect(host, 443);
            this.networkStream = new NetworkStream(this.socket);
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

        public NetworkStream GetStream()
        {
            this.networkStream.WriteTimeout = this.SendTimeout;
            this.networkStream.ReadTimeout = this.ReceiveTimeout;
            return this.networkStream;
        }

        public void Dispose()
        {
            this.socket.Dispose();
            this.networkStream.Dispose();
        }
    }
}
