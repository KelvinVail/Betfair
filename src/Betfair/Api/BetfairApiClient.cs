using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Core.Client;
using Betfair.Core.Login;

namespace Betfair.Api;

public class BetfairApiClient : IDisposable
{
    private const string _betting = "https://api.betfair.com/exchange/betting/rest/v1.0";
    private readonly HttpClient _client;
    private readonly TokenProvider _provider;
    private string _appKey;
    private string _token = string.Empty;
    private readonly bool _disposeClient = true;
    private bool _disposedValue;

    public BetfairApiClient(Credentials credentials)
    {
        _client = new BetfairHttpClient(credentials.Certificate);
        _provider = new TokenProvider(_client, credentials);
        _appKey = credentials.AppKey;
    }

    internal BetfairApiClient(HttpClient client, TokenProvider provider)
    {
        _client = client;
        _provider = provider;
        _disposeClient = false;
    }

    public async Task<IReadOnlyList<MarketCatalogue>> MarketCatalogue(
        ApiMarketFilter? filter = null,
        MarketCatalogueQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiMarketFilter();
        query ??= new MarketCatalogueQuery();
        return await Post<IReadOnlyList<MarketCatalogue>>(
            new Uri($"{_betting}/listMarketCatalogue/"),
            new
            {
                filter,
                query.MarketProjection,
                query.Sort,
                query.MaxResults,
            },
            cancellationToken);
    }

    public async Task<string> MarketStatus(
        string marketId,
        CancellationToken cancellationToken)
    {
        var response = await Post<List<MarketStatus>>(
            new Uri($"{_betting}/listMarketBook/"),
            new { MarketIds = new List<string> { marketId } },
            cancellationToken);

        return response?.FirstOrDefault()?.Status ?? "NONE";
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;

        if (disposing && _disposeClient)
            _client.Dispose();

        _disposedValue = true;
    }

    public virtual async Task<T> Post<T>(
        Uri uri,
        object? body = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (uri is null) throw new ArgumentNullException(nameof(uri));
        await SetToken(cancellationToken);

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        var json = JsonSerializer.ToJsonString(body, StandardResolver.AllowPrivateExcludeNullCamelCase);
        using var requestContent = new StringContent(json);
        requestContent.Headers.Add("X-Authentication", _token);
        requestContent.Headers.Add("X-Application", _appKey);
        request.Content = requestContent;
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(null, null, statusCode: response.StatusCode);

        var result = JsonSerializer.Deserialize<T>(
            await response.Content.ReadAsStringAsync(cancellationToken),
            StandardResolver.AllowPrivateCamelCase);

        return result;
    }

    private async Task SetToken(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_token))
            _token = await _provider.GetToken(cancellationToken);
    }
}
