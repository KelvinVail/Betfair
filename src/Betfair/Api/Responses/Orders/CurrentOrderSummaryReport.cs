﻿namespace Betfair.Api.Responses.Orders;

/// <summary>
/// Current order summary report.
/// </summary>
public class CurrentOrderSummaryReport
{
    /// <summary>
    /// Gets the current orders.
    /// </summary>
    public List<CurrentOrder>? CurrentOrders { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether more orders are available.
    /// </summary>
    public bool MoreAvailable { get; internal set; }
}
