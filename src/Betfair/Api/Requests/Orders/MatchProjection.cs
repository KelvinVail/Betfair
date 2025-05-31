﻿namespace Betfair.Api.Requests.Orders;

/// <summary>
/// Match projection for orders.
/// </summary>
public enum MatchProjection
{
    /// <summary>
    /// No matches.
    /// </summary>
    NoRollup,

    /// <summary>
    /// Rolled up matches by price.
    /// </summary>
    RolledUpByPrice,

    /// <summary>
    /// Rolled up matches by average price.
    /// </summary>
    RolledUpByAvgPrice
}
