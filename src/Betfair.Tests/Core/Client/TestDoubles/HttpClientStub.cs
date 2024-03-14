using Betfair.Core.Client;

namespace Betfair.Tests.Core.Client.TestDoubles;

public class HttpClientStub : IHttpClient
{
    public HttpContent? HttpContentSent { get; private set; }

    public object? ContentSent { get; private set; }

    public string? ThrowsError { get; set; }

    public int TimesToThrowError { get; set; } = 1;

    public async Task<T> PostAsync<T>(Uri uri, HttpContent content, CancellationToken ct = default)
        where T : class
    {
        if (ThrowsError is not null && TimesToThrowError > 0)
        {
            TimesToThrowError--;
            throw new HttpRequestException(ThrowsError);
        }

        HttpContentSent = content;
        if (content is not null)
            ContentSent = await content.ReadAsByteArrayAsync(ct);
        return await Task.FromResult<T>(default!);
    }

    public async Task PostAsync(Uri uri, HttpContent content, CancellationToken ct = default) =>
        await PostAsync<object>(uri, content, ct);
}
