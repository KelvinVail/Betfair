namespace Betfair.Streaming
{
    using System.Net.Sockets;

    public interface ITcpClient
    {
        int ReceiveBufferSize { get; set; }

        int SendTimeout { get; set; }

        int ReceiveTimeout { get; set; }

        void Connect(string host, int port);

        NetworkStream GetStream();
    }
}