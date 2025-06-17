namespace Betfair.Api.Betting.Endpoints.ListClearedOrders;

/// <summary>
/// Cleared order summary report.
/// </summary>
public class ClearedOrderSummaryReport
{
    /// <summary>
    /// Gets the cleared orders.
    /// </summary>
    public List<ClearedOrder>? ClearedOrders { get; init; }

    /// <summary>
    /// Gets a value indicating whether more orders are available.
    /// </summary>
    public bool MoreAvailable { get; init; }
}
