﻿namespace Betfair.Api.Responses;

/// <summary>
/// Competition result.
/// </summary>
public class CompetitionResult
{
    /// <summary>
    /// Gets the competition.
    /// </summary>
    public Competition? Competition { get; internal set; }

    /// <summary>
    /// Gets the count of markets associated with this competition.
    /// </summary>
    public int MarketCount { get; internal set; }

    /// <summary>
    /// Gets the region associated with this competition.
    /// </summary>
    public string? CompetitionRegion { get; internal set; }
}
