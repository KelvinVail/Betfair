namespace Betfair.Core.Client;

internal class HttpDeserializer : IHttpClient
{
    private readonly BetfairHttpClient _httpClient;

    internal HttpDeserializer(BetfairHttpClient httpClient) =>
        _httpClient = httpClient;

    public async Task<T> PostAsync<T>(Uri uri, HttpContent content, CancellationToken ct = default)
        where T : class
    {
        var response = await _httpClient.PostAsync(uri, content, ct);
        if (!response.IsSuccessStatusCode) Throw(response);

        return await Deserialize<T>(response, ct);
    }

    private static void Throw(HttpResponseMessage response) =>
        throw new HttpRequestException(null, null, statusCode: response.StatusCode);

    private static async Task<T> Deserialize<T>(HttpResponseMessage response, CancellationToken ct)
        where T : class =>
        JsonSerializer.Deserialize<T>(
            await response.Content.ReadAsStreamAsync(ct),
            StandardResolver.AllowPrivateExcludeNullCamelCase);
}
