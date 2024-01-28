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
        content.Headers.Add("X-Application", _appKey);
        await AddSessionTokenHeader(content, ct);

        try
        {
            return await _httpClient.PostAsync<T>(uri, content, ct);
        }
        catch (HttpRequestException e)
        {
            if (SessionIsInvalid(e)) throw;

            await RefreshSessionTokenHeader(content, ct);
            return await _httpClient.PostAsync<T>(uri, content, ct);
        }
    }

    private static bool SessionIsInvalid(HttpRequestException e) =>
        e.Message != "INVALID_SESSION_INFORMATION";

    private async Task AddSessionTokenHeader(HttpContent content, CancellationToken ct)
    {
        var token = await GetSessionToken(ct);
        content.Headers.Add("X-Authentication", token);
    }

    private async Task<string> GetSessionToken(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_token))
            _token = await _tokenProvider.GetToken(ct);

        return _token;
    }

    private async Task RefreshSessionTokenHeader(HttpContent content, CancellationToken ct)
    {
        var refreshedToken = await GetRefreshedSessionToken(ct);
        content.Headers.Remove("X-Authentication");
        content.Headers.Add("X-Authentication", refreshedToken);
    }

    private async Task<string> GetRefreshedSessionToken(CancellationToken ct)
    {
        _token = await _tokenProvider.GetToken(ct);

        return _token;
    }
}
