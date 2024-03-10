#pragma warning disable CA2000

namespace Betfair.Core.Client;

internal class BetfairHttpClient : HttpClient
{
    private static readonly MediaTypeWithQualityHeaderValue _jsonMediaType = new ("application/json");
    private static readonly StringWithQualityHeaderValue _gzipEncoding = new ("gzip");

    internal BetfairHttpClient(X509Certificate2? cert)
        : this(new BetfairClientHandler(cert))
    {
    }

    internal BetfairHttpClient(HttpMessageHandler handler)
        : base(handler, true) => Configure();

    private void Configure()
    {
        Timeout = TimeSpan.FromSeconds(30);
        DefaultRequestHeaders.Accept.Add(_jsonMediaType);
        DefaultRequestHeaders.Add("Connection", "keep-alive");
        DefaultRequestHeaders.AcceptEncoding.Add(_gzipEncoding);
    }
}
