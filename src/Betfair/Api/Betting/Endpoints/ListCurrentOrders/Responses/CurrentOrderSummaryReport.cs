namespace Betfair.Api.Betting.Endpoints.ListCurrentOrders.Responses;

/// <summary>
/// Current order summary report.
/// </summary>
public class CurrentOrderSummaryReport
{
    /// <summary>
    /// Gets the list of current orders returned by your query.
    /// This will be a valid list (i.e. empty or non-empty but never 'null').
    /// </summary>
    public List<CurrentOrder> CurrentOrders { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether there are further result items beyond this page.
    /// Note that underlying data is highly time-dependent and the subsequent search orders query might return an empty result.
    /// </summary>
    public bool MoreAvailable { get; init; }
}

