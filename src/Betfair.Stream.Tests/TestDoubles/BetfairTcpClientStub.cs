namespace Betfair.Stream.Tests.TestDoubles;

internal class BetfairTcpClientStub : BetfairTcpClient
{
    public string HostConnectedTo { get; private set; } = string.Empty;

    public int PortConnectTo { get; private set; }

    public System.IO.Stream StreamToUse { get; set; } = new MemoryStream();

    public override void ConnectToHost(string hostName, int port)
    {
        HostConnectedTo = hostName;
        PortConnectTo = port;
    }

    public override System.IO.Stream GetAuthenticatedSslStream() =>
        StreamToUse;
}
