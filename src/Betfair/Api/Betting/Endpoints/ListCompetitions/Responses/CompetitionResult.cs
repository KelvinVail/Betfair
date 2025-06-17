namespace Betfair.Api.Betting.Endpoints.ListCompetitions.Responses;

/// <summary>
/// Competition result.
/// </summary>
public class CompetitionResult
{
    /// <summary>
    /// Gets the competition.
    /// </summary>
    public Competition? Competition { get; init; }

    /// <summary>
    /// Gets the count of markets associated with this competition.
    /// </summary>
    public int MarketCount { get; init; }

    /// <summary>
    /// Gets the region associated with this competition.
    /// </summary>
    public string? CompetitionRegion { get; init; }
}

