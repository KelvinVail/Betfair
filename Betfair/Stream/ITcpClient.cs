namespace Betfair.Stream
{
    public interface ITcpClient
    {
        int ReceiveBufferSize { get; set; }

        int SendTimeout { get; set; }

        int ReceiveTimeout { get; set; }

        void Connect(string host, int port);

        void Close();

        System.IO.Stream GetSslStream(string host);
    }
}