using Betfair.Core.Login;

namespace Betfair.Core.Client;

internal class BetfairClient
{
    private readonly HttpClient _httpClient;
    private readonly TokenProvider _provider;
    private readonly string _appKey;
    private string _token = string.Empty;

    internal BetfairClient(TokenProvider provider, string appKey, BetfairHttpClient httpClient)
    {
        _httpClient = httpClient;
        _provider = provider;
        _appKey = appKey;
    }

    internal virtual async Task<T> Post<T>(
        Uri uri,
        object? body = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        await SetToken(cancellationToken);
        using var request = ComposeRequest(uri, _appKey, _token, body);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(null, null, statusCode: response.StatusCode);

        var result = JsonSerializer.Deserialize<T>(
            await response.Content.ReadAsStringAsync(cancellationToken),
            StandardResolver.AllowPrivateCamelCase);

        return result;
    }

    private static HttpRequestMessage ComposeRequest(Uri uri, string appKey, string token, object? body)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = ComposeContent(body);
        AddHeaders(request.Content, appKey, token);
        return request;
    }

    private static StringContent ComposeContent(object? body)
    {
        var json = JsonSerializer.ToJsonString(body, StandardResolver.AllowPrivateExcludeNullCamelCase);
        var requestContent = new StringContent(json);
        return requestContent;
    }

    private static void AddHeaders(HttpContent requestContent, string appKey, string token)
    {
        requestContent.Headers.Add("X-Application", appKey);
        requestContent.Headers.Add("X-Authentication", token);
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    }

    private async Task SetToken(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_token))
            _token = await _provider.GetToken(cancellationToken);
    }
}
