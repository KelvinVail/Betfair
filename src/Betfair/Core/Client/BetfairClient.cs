using Betfair.Core.Login;

namespace Betfair.Core.Client;

public class BetfairClient
{
    private readonly BetfairHttpClient _httpClient;
    private readonly TokenProvider _provider;
    private readonly string _appKey;
    private string _token = string.Empty;

    internal BetfairClient(BetfairHttpClient httpClient, TokenProvider provider, string appKey)
    {
        _httpClient = httpClient;
        _provider = provider;
        _appKey = appKey;
    }

    // For mock testing
    internal BetfairClient()
    {
        _httpClient = null!;
        _provider = null!;
        _appKey = null!;
    }

    internal virtual async Task<T> Post<T>(
        Uri uri,
        object? body = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        await SetToken(cancellationToken);
        using var request = ComposePost(uri, _appKey, _token, body);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(null, null, statusCode: response.StatusCode);

        var result = JsonSerializer.Deserialize<T>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            StandardResolver.AllowPrivateCamelCase);

        return result;
    }

    private static HttpRequestMessage ComposePost(Uri uri, string appKey, string token, object? body)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = ComposeContent(body) };
        AddHeaders(request.Content, appKey, token);
        return request;
    }

    private static ByteArrayContent ComposeContent(object? body)
    {
        var bytes = JsonSerializer.Serialize(body, StandardResolver.AllowPrivateExcludeNullCamelCase);
        return new ByteArrayContent(bytes);
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
