using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Core.Client;
using Betfair.Core.Login;

namespace Betfair.Api;

public class BetfairApiClient : IDisposable
{
    private const string _betting = "https://api.betfair.com/exchange/betting/rest/v1.0";
    private readonly BetfairHttpClient _client;
    private readonly bool _disposeClient = true;
    private bool _disposedValue;

    public BetfairApiClient(Credentials credentials) =>
        _client = new BetfairHttpClient(credentials);

    internal BetfairApiClient(BetfairHttpClient client)
    {
        _client = client;
        _disposeClient = false;
    }

    public async Task<IReadOnlyList<MarketCatalogue>> MarketCatalogue(
        ApiMarketFilter? filter = null,
        MarketCatalogueQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiMarketFilter();
        query ??= new MarketCatalogueQuery();
        return await _client.Post<IReadOnlyList<MarketCatalogue>>(
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
        var response = await _client.Post<List<MarketStatus>>(
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
}
