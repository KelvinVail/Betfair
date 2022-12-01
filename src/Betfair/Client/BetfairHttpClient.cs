using System.Net.Http.Headers;
using Betfair.Login;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Client;

public class BetfairHttpClient : HttpClient
{
    private static readonly BetfairClientHandler _handler = new ();
    private readonly Credentials _credentials;

    public BetfairHttpClient(Credentials credentials)
        : base(_handler)
    {
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        Configure();
    }

    public BetfairHttpClient(Credentials credentials, HttpMessageHandler handler)
        : base(handler)
    {
        _credentials = credentials;
        Configure();
    }

#pragma warning disable CS8618 // For mocking
    protected BetfairHttpClient()
#pragma warning restore CS8618
    {
    }

    public virtual async Task<Result<T, ErrorResult>> Post<T>(
        Uri uri,
        string sessionToken,
        object? body = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (uri is null) return ErrorResult.Empty(nameof(uri));
        if (string.IsNullOrWhiteSpace(sessionToken)) return ErrorResult.Empty(nameof(sessionToken));

        var ms = new MemoryStream();
        if (body is not null)
            await JsonSerializer.SerializeAsync(ms, body, StandardResolver.CamelCase);

        ms.Seek(0, SeekOrigin.Begin);
        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        using var requestContent = new StreamContent(ms);
        request.Content = requestContent;
        _credentials.AddHeaders(requestContent, sessionToken);

        using var response = await SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return ErrorResult.Create(response.StatusCode.ToString());

        var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<T>(
            content,
            StandardResolver.CamelCase);
        return result;
    }

    private void Configure()
    {
        DefaultRequestHeaders.Clear();
        Timeout = TimeSpan.FromSeconds(30);
        DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders
            .Add("Connection", "keep-alive");
        DefaultRequestHeaders.AcceptEncoding
            .Add(new StringWithQualityHeaderValue("gzip"));

        if (_credentials.Certificate is not null)
            _handler.ClientCertificates.Add(_credentials.Certificate);
    }
}
