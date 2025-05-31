﻿namespace Betfair.Api.Responses;

/// <summary>
/// Market type result.
/// </summary>
public class MarketTypeResult
{
    /// <summary>
    /// Gets the market type.
    /// </summary>
    public string? MarketType { get; internal set; }

    /// <summary>
    /// Gets the count of markets associated with this market type.
    /// </summary>
    public int MarketCount { get; internal set; }
}
