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

    private static async Task Throw(HttpResponseMessage response) =>
        throw new HttpRequestException(
            await ErrorCode(response),
            null,
            statusCode: response.StatusCode);

    private static async Task<dynamic> ErrorCode(HttpResponseMessage response)
    {
        var error = await Deserialize<dynamic>(response, default);
        try
        {
            var errorCode = error["detail"]["apiNgException"]["errorCode"];
            return errorCode;
        }
        catch (KeyNotFoundException)
        {
            return "An HttpRequestException Occurred.";
        }
    }

    private static async Task<T> Deserialize<T>(HttpResponseMessage response, CancellationToken ct)
        where T : class =>
        JsonSerializer.Deserialize<T>(
            await response.Content.ReadAsStreamAsync(ct),
            StandardResolver.AllowPrivateExcludeNullCamelCase);
}
