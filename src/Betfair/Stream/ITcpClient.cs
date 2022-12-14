using System.Net.Security;

namespace Betfair.Stream;

public interface ITcpClient : IDisposable
{
    int ReceiveBufferSize { get; set; }

    int SendTimeout { get; set; }

    int ReceiveTimeout { get; set; }

    void Connect(string host, int port);

    void Close();

    SslStream GetSslStream();
}