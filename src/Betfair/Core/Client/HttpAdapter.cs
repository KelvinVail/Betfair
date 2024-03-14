namespace Betfair.Core.Client;

internal class HttpAdapter
{
    private readonly IHttpClient _client;

    internal HttpAdapter(IHttpClient client) =>
        _client = client;

    internal HttpAdapter() =>
        _client = null!;

    public virtual async Task<T> PostAsync<T>(Uri uri, object body, CancellationToken ct = default)
        where T : class
    {
        using var content = ToContent(body);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return await _client.PostAsync<T>(uri, content, ct);
    }

    //TODO: Remove duplicate code
    public virtual async Task PostAsync(Uri uri, object body, CancellationToken ct = default)
    {
        using var content = ToContent(body);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        await _client.PostAsync(uri, content, ct);
    }

    private static ByteArrayContent ToContent(object body) =>
        new (JsonSerializer.SerializeToUtf8Bytes(body, body.GetContext()));
}
