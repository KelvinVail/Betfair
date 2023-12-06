using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Core.Client;

namespace Betfair.Api;

public class BetfairClient
{
    private const string _base = "https://api.betfair.com/exchange/betting/rest/v1.0";
    private readonly BetfairHttpClient _client;

    public BetfairClient(BetfairHttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<MarketCatalogue>> MarketCatalogue(
        string sessionToken,
        MarketFilter? marketFilter = null,
        MarketProjection? marketProjection = null,
        MarketSort? marketSort = null,
        int maxResults = 1000,
        CancellationToken cancellationToken = default)
    {
        var body = new RequestBody { MaxResults = maxResults };
        if (marketFilter is not null) body.Filter = marketFilter.Filter;
        if (marketProjection is not null) body.MarketProjection = marketProjection;
        if (marketSort is not null) body.Sort = marketSort.Value;

        return await _client.Post<IReadOnlyList<MarketCatalogue>>(
            new Uri($"{_base}/listMarketCatalogue/"),
            body,
            cancellationToken);
    }

    public async Task<string> MarketStatus(string marketId, CancellationToken cancellationToken)
    {
        var response = await _client.Post<List<MarketStatus>>(
            new Uri($"{_base}/listMarketBook/"),
            new { MarketIds = new List<string> { marketId } },
            cancellationToken);

        return response?.FirstOrDefault()?.Status ?? "NONE";
    }
}
