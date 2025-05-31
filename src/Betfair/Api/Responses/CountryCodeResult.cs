﻿namespace Betfair.Api.Responses;

/// <summary>
/// Country code result.
/// </summary>
public class CountryCodeResult
{
    /// <summary>
    /// Gets the country code.
    /// </summary>
    public string? CountryCode { get; internal set; }

    /// <summary>
    /// Gets the count of markets associated with this country.
    /// </summary>
    public int MarketCount { get; internal set; }
}
