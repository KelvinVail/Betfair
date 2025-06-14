﻿namespace Betfair.Api.Responses;

/// <summary>
/// Venue result.
/// </summary>
public class VenueResult
{
    /// <summary>
    /// Gets the venue.
    /// </summary>
    public string? Venue { get; init; }

    /// <summary>
    /// Gets the count of markets associated with this venue.
    /// </summary>
    public int MarketCount { get; init; }
}
