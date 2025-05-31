﻿namespace Betfair.Api.Responses.Account;

/// <summary>
/// Account statement report.
/// </summary>
public class AccountStatementReport
{
    /// <summary>
    /// Gets the account statement items.
    /// </summary>
    public List<StatementItem>? AccountStatement { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether more items are available.
    /// </summary>
    public bool MoreAvailable { get; internal set; }
}
