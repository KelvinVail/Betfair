namespace Betfair.Stream;

internal class BetfairTcpClient : TcpClient
{
    private const string _hostName = "stream-api.betfair.com";

    public void Configure()
    {
        ReceiveBufferSize = 1024 * 1000 * 2;
        SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        ConnectToHost(_hostName, 443);
    }

    public virtual void ConnectToHost(string hostName, int port) =>
        Connect(hostName, port);

    public virtual System.IO.Stream GetAuthenticatedSslStream()
    {
        var sslStream = new SslStream(GetStream(), false);
        sslStream.AuthenticateAsClient(_hostName);

        return sslStream;
    }
}
