using Betfair.Betting.Models;
using Betfair.Client;

namespace Betfair.Betting;

public class BettingClient
{
    private const string _base = $"https://api.betfair.com/exchange/betting/rest/v1.0";
    private readonly BetfairHttpClient _client;

    public BettingClient(BetfairHttpClient client) =>
        _client = client;

    public async Task<Result<IReadOnlyList<EventTypesResponse>, ErrorResult>> EventTypes(
        string sessionToken,
        MarketFilter? marketFilter = null,
        CancellationToken cancellationToken = default)
    {
        var body = new RequestBody();
        if (marketFilter is not null) body.Filter = marketFilter.Filter;
        return await _client.Post<IReadOnlyList<EventTypesResponse>>(
            new Uri($"{_base}/listEventTypes/"),
            sessionToken,
            body,
            cancellationToken);
    }

    public async Task<Result<IReadOnlyList<MarketCatalogue>, ErrorResult>> MarketCatalogue(
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
            sessionToken,
            body,
            cancellationToken);
    }
}
