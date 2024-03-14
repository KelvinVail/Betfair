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
        if (!response.IsSuccessStatusCode) await Throw(response);

        return await Deserialize<T>(response, ct);
    }

    //TODO: Remove duplicate code
    public async Task PostAsync(Uri uri, HttpContent content, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsync(uri, content, ct);
        if (!response.IsSuccessStatusCode) await Throw(response);
    }

    private static async Task Throw(HttpResponseMessage response) =>
        throw new HttpRequestException(
            await ErrorCode(response),
            null,
            statusCode: response.StatusCode);

    private static async Task<string> ErrorCode(HttpResponseMessage response)
    {
        var error = await Deserialize<BadRequestResponse>(response, default);
        try
        {
            return error.Detail.ApiNgException.ErrorCode ?? "An HttpRequestException Occurred.";
        }
        catch (KeyNotFoundException)
        {
            return "An HttpRequestException Occurred.";
        }
    }

    private static async Task<T> Deserialize<T>(HttpResponseMessage response, CancellationToken ct)
        where T : class
    {
        var stream = await response.Content.ReadAsStreamAsync(ct);
        return (await JsonSerializer.DeserializeAsync(stream, SerializerContextExtensions.GeTypeInfo<T>(), ct)) !;
    }
}
