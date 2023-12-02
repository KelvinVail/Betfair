using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Betfair.Core.Client;

internal sealed class BetfairClientHandler : HttpClientHandler
{
    public BetfairClientHandler(X509Certificate2? cert = null)
    {
        CheckCertificateRevocationList = true;
        AutomaticDecompression = DecompressionMethods.GZip;
        UseProxy = false;
        if (cert is not null) ClientCertificates.Add(cert);
    }
}
