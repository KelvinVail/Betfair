using System.Diagnostics.CodeAnalysis;
using Betfair.Core.Login;

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

    // For testing
    protected BetfairHttpClient()
    {
        _credentials = null!;
        _tokenProvider = null!;
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

        var ms = new MemoryStream();
        if (body is not null)
            await JsonSerializer.SerializeAsync(ms, body, StandardResolver.AllowPrivateExcludeNullCamelCase);

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        using var requestContent = new StreamContent(ms);
        request.Content = requestContent;
        requestContent.Headers.Add("Content-Type", "application/json");
        requestContent.Headers.Add("X-Authentication", _token);
        requestContent.Headers.Add("X-Application", _credentials.AppKey);

        using var response = await SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new BetfairRequestException(response.StatusCode);

        var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<T>(
            content,
            StandardResolver.CamelCase);
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
