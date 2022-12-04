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
}
