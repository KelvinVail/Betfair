namespace Betfair.Core.Client;

public sealed class BetfairClientHandler : HttpClientHandler
{
    public BetfairClientHandler(X509Certificate2? cert = null)
    {
        CheckCertificateRevocationList = true;
        AutomaticDecompression = DecompressionMethods.GZip;
        UseProxy = false;
        if (cert is not null) ClientCertificates.Add(cert);
    }
}
