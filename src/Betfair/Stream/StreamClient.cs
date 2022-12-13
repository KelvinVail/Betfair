using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace Betfair.Stream;

public class StreamClient
{
    private const string _hostName = "stream-api.betfair.com";
    private readonly ITcpClient _tcpClient;

#pragma warning disable CS8618
    public StreamClient()
    {
        _tcpClient = new ExchangeStreamClient();
        Connect();
    }

    public StreamClient(ITcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        Connect();
    }

    public virtual StreamWriter Writer { get; private set; }

    public virtual StreamReader Reader { get; protected set; }

    private static System.IO.Stream GetStream(ITcpClient client)
    {
        client.ReceiveBufferSize = 1024 * 1000 * 2;
        client.SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        client.ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        client.Connect(_hostName, 443);

        var sslStream = client.GetSslStream();
        sslStream.AuthenticateAsClient(_hostName);

        return sslStream;
    }

    private void Connect()
    {
        var stream = GetStream(_tcpClient);
        Reader = new StreamReader(
            stream,
            Encoding.UTF8,
            false,
            _tcpClient.ReceiveBufferSize);

        Writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
    }

    private sealed class ExchangeStreamClient : TcpClient, ITcpClient
    {
        public SslStream GetSslStream() =>
            new (GetStream(), false);
    }
}
