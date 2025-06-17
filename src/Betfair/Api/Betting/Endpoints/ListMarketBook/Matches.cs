namespace Betfair.Api.Betting.Endpoints.ListMarketBook;

/// <summary>
/// Match list.
/// </summary>
public class Matches
{
    /// <summary>
    /// Gets the list of matches.
    /// </summary>
    [JsonPropertyName("matches")]
    public List<Match>? MatchList { get; init; }
}
