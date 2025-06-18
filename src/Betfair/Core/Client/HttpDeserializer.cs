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

    private static async Task Throw(HttpResponseMessage response, Uri uri, CancellationToken ct)
    {
        var responseContent = await response.Content.ReadAsStringAsync(ct);

        var betfairException = TryParseBetfairError(responseContent, uri, response.StatusCode);
        if (betfairException != null)
        {
            throw betfairException;
        }

        // Fallback to generic HttpRequestException with the original response content
        throw new HttpRequestException(responseContent, null, statusCode: response.StatusCode);
    }

    private static Exception? TryParseBetfairError(string responseContent, Uri uri, HttpStatusCode statusCode)
    {
        var errorResponse = TryDeserializeErrorResponse(responseContent);
        if (!IsValidBetfairError(errorResponse))
            return null;

        return CreateBetfairException(errorResponse!, uri, statusCode);
    }

    private static bool IsValidBetfairError(BetfairErrorResponse? errorResponse)
    {
        return errorResponse?.Detail?.APINGException?.ErrorCode != null;
    }

    private static Exception CreateBetfairException(BetfairErrorResponse errorResponse, Uri uri, HttpStatusCode statusCode)
    {
        var isAccountApi = uri.ToString().Contains("/account/", StringComparison.OrdinalIgnoreCase);

        if (isAccountApi)
        {
            return BetfairExceptionFactory.CreateAccountException(errorResponse, statusCode);
        }

        return BetfairExceptionFactory.CreateBettingException(errorResponse, statusCode);
    }

    private static BetfairErrorResponse? TryDeserializeErrorResponse(string responseContent)
    {
        try
        {
            return JsonSerializer.Deserialize(
                responseContent,
                SerializerContextExtensions.GetInternalTypeInfo<BetfairErrorResponse>());
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static async Task<T> Deserialize<T>(HttpResponseMessage response, CancellationToken ct)
        where T : class
    {
        var stream = await response.Content.ReadAsStreamAsync(ct);
        return (await JsonSerializer.DeserializeAsync(stream, SerializerContextExtensions.GetInternalTypeInfo<T>(), ct)) !;
    }

    private async Task<HttpResponseMessage> Post(Uri uri, HttpContent content, CancellationToken ct)
    {
        var response = await _httpClient.PostAsync(uri, content, ct);
        if (!response.IsSuccessStatusCode) await Throw(response, uri, ct);

        return response;
    }
}
