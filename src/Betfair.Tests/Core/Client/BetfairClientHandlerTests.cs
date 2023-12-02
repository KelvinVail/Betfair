using Betfair.Core.Client;
using Betfair.Tests.Core.Client.TestDoubles;

namespace Betfair.Tests.Core.Client;

public sealed class BetfairClientHandlerTests
{
    [Fact]
    public void CheckCertificateRevocationListIsTrue()
    {
        using var handler = new BetfairClientHandler();

        handler.CheckCertificateRevocationList.Should().BeTrue();
    }

    [Fact]
    public void AutoUnzipGzip()
    {
        using var handler = new BetfairClientHandler();

        handler.AutomaticDecompression.Should().Be(DecompressionMethods.GZip);
    }

    [Fact]
    public void UseProxyIsSetToFalse()
    {
        using var handler = new BetfairClientHandler();

        handler.UseProxy.Should().BeFalse();
    }

    [Fact]
    public void ACertCanBeAdded()
    {
        using var cert = new X509Certificate2Stub();
        using var handler = new BetfairClientHandler(cert);

        handler.ClientCertificates.Contains(cert).Should().BeTrue();
    }
}
