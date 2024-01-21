using Betfair.Stream;

namespace Betfair.Tests.Stream.TestDoubles;

internal class BetfairTcpClientStub : BetfairTcpClient
{
    public System.IO.Stream StreamToUse { get; set; } = new MemoryStream();

    internal override System.IO.Stream GetAuthenticatedSslStream() =>
        StreamToUse;
}
