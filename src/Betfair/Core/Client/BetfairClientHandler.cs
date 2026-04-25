namespace Betfair.Core.Client;

/// <summary>
/// The default <see cref="HttpClientHandler"/> for Betfair API requests.
/// Enables certificate revocation checking, GZip decompression, and disables proxy usage.
/// Pass an instance to <see cref="IHttpClientBuilder.ConfigurePrimaryHttpMessageHandler"/> when
/// registering a named or typed client with <see cref="IHttpClientFactory"/> to ensure the same
/// transport settings are applied when using the factory-based constructor of
/// <see cref="Betfair.Api.BetfairApiClient"/>.
/// </summary>
public sealed class BetfairClientHandler : HttpClientHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BetfairClientHandler"/> class.
    /// </summary>
    /// <param name="cert">An optional client certificate for certificate-based authentication.</param>
    public BetfairClientHandler(X509Certificate2? cert = null)
    {
        CheckCertificateRevocationList = true;
        AutomaticDecompression = DecompressionMethods.GZip;
        UseProxy = false;
        if (cert is not null) ClientCertificates.Add(cert);
    }
}
