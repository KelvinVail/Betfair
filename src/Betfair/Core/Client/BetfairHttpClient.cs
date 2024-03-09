namespace Betfair.Core.Client;

internal class BetfairHttpClient : HttpClient
{
    internal BetfairHttpClient(X509Certificate2? cert)
#pragma warning disable CA2000, Disposed by HttpClient
        : this(new BetfairClientHandler(cert))
#pragma warning restore CA2000
    {
    }

    internal BetfairHttpClient(HttpMessageHandler handler)
        : base(handler, true) => Configure();

    private void Configure()
    {
        Timeout = TimeSpan.FromSeconds(30);
        DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders.Add("Connection", "keep-alive");
        DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
    }
}
