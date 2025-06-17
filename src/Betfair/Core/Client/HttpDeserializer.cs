namespace Betfair.Core.Client;

internal class HttpDeserializer : IHttpClient
{
    private readonly BetfairHttpClient _httpClient;

    internal HttpDeserializer(BetfairHttpClient httpClient) =>
        _httpClient = httpClient;

    public async Task<T> PostAsync<T>(Uri uri, HttpContent content, CancellationToken ct = default)
        where T : class
    {
        var response = await Post(uri, content, ct);

        return await Deserialize<T>(response, ct);
    }

    public async Task PostAsync(Uri uri, HttpContent content, CancellationToken ct = default) =>
        await Post(uri, content, ct);

    private static async Task Throw(HttpResponseMessage response, CancellationToken ct) =>
        throw new HttpRequestException(
            await response.Content.ReadAsStringAsync(ct),
            null,
            statusCode: response.StatusCode);

    private static async Task<T> Deserialize<T>(HttpResponseMessage response, CancellationToken ct)
        where T : class
    {
        var stream = await response.Content.ReadAsStreamAsync(ct);
        var str = await response.Content.ReadAsStringAsync(ct);
        return (await JsonSerializer.DeserializeAsync(stream, SerializerContextExtensions.GetInternalTypeInfo<T>(), ct)) !;
    }

    private async Task<HttpResponseMessage> Post(Uri uri, HttpContent content, CancellationToken ct)
    {
        var response = await _httpClient.PostAsync(uri, content, ct);
        if (!response.IsSuccessStatusCode) await Throw(response, ct);

        return response;
    }
}
