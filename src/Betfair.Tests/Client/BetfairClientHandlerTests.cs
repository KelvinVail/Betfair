using System.Net;
using Betfair.Client;

namespace Betfair.Tests.Client;

public sealed class BetfairClientHandlerTests
{
    [Fact]
    public void CheckCertificateRevocationListIsTrue()
    {
        using var handler = new BetfairClientHandler();

        Assert.True(handler.CheckCertificateRevocationList);
    }

    [Fact]
    public void AutoUnzipGzip()
    {
        using var handler = new BetfairClientHandler();

        Assert.Equal(DecompressionMethods.GZip, handler.AutomaticDecompression);
    }

    [Fact]
    public void UseProxyIsSetToFalse()
    {
        using var handler = new BetfairClientHandler();

        Assert.False(handler.UseProxy);
    }
}
