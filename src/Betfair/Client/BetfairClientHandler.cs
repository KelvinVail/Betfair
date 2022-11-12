using System.Net;

namespace Betfair.Client;

public sealed class BetfairClientHandler : HttpClientHandler
{
    public BetfairClientHandler() =>
        Configure();

    private void Configure()
    {
        CheckCertificateRevocationList = true;
        AutomaticDecompression = DecompressionMethods.GZip;
    }
}
