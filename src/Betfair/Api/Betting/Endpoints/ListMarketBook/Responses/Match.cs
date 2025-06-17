namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;

/// <summary>
/// An individual bet Match, or rollup by price or avg price. Rollup depends on the requested MatchProjection.
/// </summary>
public class Match
{
    /// <summary>
    /// Gets the bet ID. Only present if no rollup.
    /// </summary>
    [JsonPropertyName("betId")]
    public string? BetId { get; init; }

    /// <summary>
    /// Gets the match ID. Only present if no rollup.
    /// </summary>
    [JsonPropertyName("matchId")]
    public string? MatchId { get; init; }

    /// <summary>
    /// Gets the side of the bet.
    /// </summary>
    [JsonPropertyName("side")]
    public string Side { get; init; } = string.Empty;

    /// <summary>
    /// Gets the price. Either actual match price or avg match price depending on rollup.
    /// This value is not meaningful for activity on LINE markets and is not guaranteed to be returned or maintained for these markets.
    /// </summary>
    [JsonPropertyName("price")]
    public double Price { get; init; }

    /// <summary>
    /// Gets the size matched at in this fragment, or at this price or avg price depending on rollup.
    /// </summary>
    [JsonPropertyName("size")]
    public double Size { get; init; }

    /// <summary>
    /// Gets the match date. Only present if no rollup.
    /// </summary>
    [JsonPropertyName("matchDate")]
    public DateTimeOffset? MatchDate { get; init; }
}

