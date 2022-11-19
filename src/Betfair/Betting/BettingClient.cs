using Betfair.Client;

namespace Betfair.Betting;

public class BettingClient
{
    private const string _area = "betting";
    private const string _baseUri = $"https://api.betfair.com/exchange/{_area}/rest/v1.0/";
    private readonly BetfairHttpClient _client;

    public BettingClient(BetfairHttpClient client) =>
        _client = client;

    public async Task<Result<IReadOnlyList<EventType>, ErrorResult>> EventTypes(
        string sessionToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
            return ErrorResult.Empty(nameof(sessionToken));

        await _client.Post<object>(
            new Uri($"{_baseUri}listEventTypes/"),
            Maybe<object>.None,
            string.Empty,
            cancellationToken);
        return default;
    }
}
