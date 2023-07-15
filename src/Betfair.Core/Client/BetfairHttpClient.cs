namespace Betfair.Core.Client;

public class BetfairHttpClient : HttpClient
{
    private static readonly BetfairClientHandler _handler = new ();
    private readonly string _appKey;

    public BetfairHttpClient(string appKey)
        : base(_handler)
    {
        if (string.IsNullOrWhiteSpace(appKey))
            throw new ArgumentNullException(nameof(appKey));
        _appKey = appKey;
        Configure();
    }

    public BetfairHttpClient(HttpMessageHandler handler, string appKey)
        : base(handler)
    {
        if (string.IsNullOrWhiteSpace(appKey))
            throw new ArgumentNullException(nameof(appKey));
        _appKey = appKey;
        Configure();
    }

    public virtual async Task<T> Post<T>(
        Uri uri,
        string sessionToken,
        object? body = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (uri is null) throw new ArgumentNullException(nameof(uri));
        if (string.IsNullOrWhiteSpace(sessionToken)) throw new ArgumentNullException(nameof(sessionToken));

        var ms = new MemoryStream();
        if (body is not null)
            await JsonSerializer.SerializeAsync(ms, body, StandardResolver.AllowPrivateExcludeNullCamelCase);

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        using var requestContent = new StreamContent(ms);
        request.Content = requestContent;
        requestContent.Headers.Add("Content-Type", "application/json");
        requestContent.Headers.Add("X-Authentication", sessionToken);
        requestContent.Headers.Add("X-Application", _appKey);

        using var response = await SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new BetfairRequestException(response.StatusCode);

        var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<T>(
            content,
            StandardResolver.CamelCase);
        return result;
    }

    private void Configure()
    {
        Timeout = TimeSpan.FromSeconds(30);
        DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders
            .Add("Connection", "keep-alive");
        DefaultRequestHeaders.AcceptEncoding
            .Add(new StringWithQualityHeaderValue("gzip"));
    }
}
