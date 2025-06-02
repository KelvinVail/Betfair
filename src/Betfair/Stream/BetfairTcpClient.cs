namespace Betfair.Stream;

[ExcludeFromCodeCoverage]
internal class BetfairTcpClient : TcpClient
{
    private const string _hostName = "stream-api.betfair.com";

    public BetfairTcpClient() =>
        Configure();

    [ExcludeFromCodeCoverage]
    internal virtual System.IO.Stream GetAuthenticatedSslStream()
    {
        Connect(_hostName, 443);
        var sslStream = new SslStream(GetStream(), false);
        sslStream.AuthenticateAsClient(_hostName);

        return sslStream;
    }

    private void Configure()
    {
        ReceiveBufferSize = 1024 * 1000 * 2;
        SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
    }
}
