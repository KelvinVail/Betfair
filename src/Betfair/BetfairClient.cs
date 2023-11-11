using Betfair.Core.Client;
using Betfair.Requests;
using Betfair.Responses;

namespace Betfair;

public class BetfairClient
{
    private const string _base = $"https://api.betfair.com/exchange/betting/rest/v1.0";
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
}
