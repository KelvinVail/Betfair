﻿namespace Betfair.Api.Requests.Account;

/// <summary>
/// Include item types for account statement.
/// </summary>
public enum IncludeItem
{
    /// <summary>
    /// All items.
    /// </summary>
    All,

    /// <summary>
    /// Deposits and withdrawals.
    /// </summary>
    DepositsWithdrawals,

    /// <summary>
    /// Exchange items.
    /// </summary>
    Exchange,

    /// <summary>
    /// Poker room items.
    /// </summary>
    PokerRoom
}
