namespace Betfair.Api.Betting.Endpoints.ListCompetitions.Responses;

/// <summary>
/// Competition.
/// </summary>
public sealed class Competition
{
    /// <summary>
    /// Gets the unique identifier for the competition.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the competition.
    /// </summary>
    public string Name { get; init; } = string.Empty;
}

