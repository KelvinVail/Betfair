﻿﻿namespace Betfair.Api.Accounts.Endpoints.GetAccountStatement.Responses;

/// <summary>
/// Account statement report.
/// </summary>
public class AccountStatementReport
{
    /// <summary>
    /// Gets the account statement items.
    /// </summary>
    public List<StatementItem>? AccountStatement { get; init; }

    /// <summary>
    /// Gets a value indicating whether more items are available.
    /// </summary>
    public bool MoreAvailable { get; init; }
}
