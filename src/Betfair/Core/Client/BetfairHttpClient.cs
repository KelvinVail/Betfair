using Betfair.Core.Login;
using JsonSerializer = Utf8Json.JsonSerializer;

namespace Betfair.Core.Client;

public class BetfairHttpClient : HttpClient
{
    private static readonly BetfairClientHandler _handler = new ();
    private readonly Credentials _credentials;
    private readonly TokenProvider _tokenProvider;
    private string _token = string.Empty;

    public BetfairHttpClient(Credentials credentials)
        : base(_handler)
    {
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        _tokenProvider = new TokenProvider(this, _credentials);

        Configure(_handler);
    }

    public BetfairHttpClient([NotNull]HttpClientHandler handler, Credentials credentials)
        : base(handler)
    {
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        _tokenProvider = new TokenProvider(this, _credentials);
        Configure(handler);
    }

    public virtual string AppKey => _credentials.AppKey;

    public virtual async Task<T> Post<T>(
        Uri uri,
        object? body = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (uri is null) throw new ArgumentNullException(nameof(uri));
        await SetToken(cancellationToken);

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        var json = JsonSerializer.ToJsonString(body, StandardResolver.AllowPrivateExcludeNullCamelCase);
        using var requestContent = new StringContent(json);
        requestContent.Headers.Add("X-Authentication", _token);
        requestContent.Headers.Add("X-Application", _credentials.AppKey);
        request.Content = requestContent;
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(null, null, statusCode: response.StatusCode);

        var result = JsonSerializer.Deserialize<T>(
            await response.Content.ReadAsStringAsync(cancellationToken),
            StandardResolver.AllowPrivateCamelCase);

        return result;
    }

    public virtual async Task<string> GetToken(CancellationToken cancellationToken = default)
    {
        await SetToken(cancellationToken);
        return _token;
    }

    private async Task SetToken(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_token))
            _token = await _tokenProvider.GetToken(cancellationToken);
    }

    private void Configure(HttpClientHandler handler)
    {
        Timeout = TimeSpan.FromSeconds(30);
        DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders
            .Add("Connection", "keep-alive");
        DefaultRequestHeaders.AcceptEncoding
            .Add(new StringWithQualityHeaderValue("gzip"));

        if (_credentials.Certificate != null)
            handler.ClientCertificates.Add(_credentials.Certificate);
    }
}
