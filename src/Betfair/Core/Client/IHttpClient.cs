namespace Betfair.Core.Client;

internal interface IHttpClient
{
    Task<T> PostAsync<T>(Uri uri, HttpContent content, CancellationToken ct = default)
        where T : class;

    Task PostAsync(Uri uri, HttpContent content, CancellationToken ct = default);
}