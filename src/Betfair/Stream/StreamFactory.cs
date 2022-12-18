namespace Betfair.Stream;

public static class StreamFactory
{
    private const string _hostName = "stream-api.betfair.com";

    public static System.IO.Stream Open(ITcpClient client)
    {
        if (client is null) return Open();

        client.ReceiveBufferSize = 1024 * 1000 * 2;
        client.SendTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        client.ReceiveTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
        client.Connect(_hostName, 443);

        var sslStream = client.GetSslStream();
        sslStream.AuthenticateAsClient(_hostName);

        return sslStream;
    }

    public static System.IO.Stream Open() =>
        new MemoryStream();
}
