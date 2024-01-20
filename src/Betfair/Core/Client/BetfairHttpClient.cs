namespace Betfair.Core.Client;

internal class BetfairHttpClient : HttpClient
{
    private static readonly BetfairClientHandler _handler = new ();
    private readonly X509Certificate2? _cert;

    internal BetfairHttpClient(X509Certificate2? cert)
        : base(_handler)
    {
        _cert = cert;
        Configure(_handler);
    }

    internal BetfairHttpClient(HttpClientHandler handler)
        : base(handler) => Configure(handler);

    private void Configure(HttpClientHandler handler)
    {
        Timeout = TimeSpan.FromSeconds(30);
        DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders.Add("Connection", "keep-alive");
        DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        if (_cert is not null) handler.ClientCertificates.Add(_cert);
    }
}
