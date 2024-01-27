using Betfair.Core.Login;

namespace Betfair.Core.Client;

internal class HttpTokenInjector : IHttpClient
{
    private readonly IHttpClient _httpClient;
    private readonly TokenProvider _tokenProvider;
    private readonly string _appKey;
    private string _token = string.Empty;

    internal HttpTokenInjector(IHttpClient httpClient, TokenProvider tokenProvider, string appKey)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _appKey = appKey;
    }

    public async Task<T> PostAsync<T>(Uri uri, HttpContent content, CancellationToken ct = default)
        where T : class
    {
        var token = await GetSessionToken(ct);
        content.Headers.Add("X-Authentication", token);
        content.Headers.Add("X-Application", _appKey);
        return await _httpClient.PostAsync<T>(uri, content, ct);
    }

    private async Task<string> GetSessionToken(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_token))
            _token = await _tokenProvider.GetToken(ct);

        return _token;
    }
}
