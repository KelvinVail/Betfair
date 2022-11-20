using Betfair.Client;

namespace Betfair.Betting;

public class BettingClient
{
    private const string _area = "betting";
    private const string _baseUri = $"https://api.betfair.com/exchange/{_area}/rest/v1.0";
    private readonly BetfairHttpClient _client;

    public BettingClient(BetfairHttpClient client) =>
        _client = client;

    public async Task<Result<IReadOnlyList<EventTypesResponse>, ErrorResult>> EventTypes(
        string sessionToken,
        MarketFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        var body = new RequestBody();
        if (filter is not null) body.Filter = filter;
        return await _client.Post<IReadOnlyList<EventTypesResponse>>(
            new Uri($"{_baseUri}/listEventTypes/"),
            sessionToken,
            body,
            cancellationToken);
    }
}
