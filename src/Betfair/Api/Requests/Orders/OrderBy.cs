﻿namespace Betfair.Api.Requests.Orders;

/// <summary>
/// Order by options for current orders.
/// </summary>
public enum OrderBy
{
    /// <summary>
    /// Order by bet ID.
    /// </summary>
    BetId,

    /// <summary>
    /// Order by market ID.
    /// </summary>
    MarketId,

    /// <summary>
    /// Order by placed date.
    /// </summary>
    PlacedDate,

    /// <summary>
    /// Order by matched date.
    /// </summary>
    MatchedDate
}
