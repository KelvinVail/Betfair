namespace Betfair.Streaming
{
    using System.IO;

    public interface ITcpClient
    {
        int ReceiveBufferSize { get; set; }

        int SendTimeout { get; set; }

        int ReceiveTimeout { get; set; }

        void Connect(string host, int port);

        void Close();

        Stream GetSslStream(string host);
    }
}