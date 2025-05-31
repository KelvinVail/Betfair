﻿namespace Betfair.Api.Requests.Orders;

/// <summary>
/// Bet status for cleared orders.
/// </summary>
public enum BetStatus
{
    /// <summary>
    /// Settled bets.
    /// </summary>
    Settled,

    /// <summary>
    /// Voided bets.
    /// </summary>
    Voided,

    /// <summary>
    /// Lapsed bets.
    /// </summary>
    Lapsed,

    /// <summary>
    /// Cancelled bets.
    /// </summary>
    Cancelled
}
