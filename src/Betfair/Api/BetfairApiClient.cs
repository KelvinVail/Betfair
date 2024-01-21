using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Core.Client;
using Betfair.Core.Login;

namespace Betfair.Api;

public class BetfairApiClient
{
    private const string _betting = "https://api.betfair.com/exchange/betting/rest/v1.0";
    private readonly BetfairClient _client;

    public BetfairApiClient(Credentials credentials)
    {
        ArgumentNullException.ThrowIfNull(credentials);
        var httpClient = new BetfairHttpClient(credentials.Certificate);
        var provider = new TokenProvider(httpClient, credentials);
        _client = new BetfairClient(httpClient, provider, credentials.AppKey);
    }

    internal BetfairApiClient(BetfairClient client) =>
        _client = client;

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
}
