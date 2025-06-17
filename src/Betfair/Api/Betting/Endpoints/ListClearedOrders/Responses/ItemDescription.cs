namespace Betfair.Api.Betting.Endpoints.ListClearedOrders.Responses;

/// <summary>
/// Item description for cleared orders.
/// </summary>
public class ItemDescription
{
    /// <summary>
    /// Gets the event type description.
    /// </summary>
    public string? EventTypeDesc { get; init; }

    /// <summary>
    /// Gets the event description.
    /// </summary>
    public string? EventDesc { get; init; }

    /// <summary>
    /// Gets the market description.
    /// </summary>
    public string? MarketDesc { get; init; }

    /// <summary>
    /// Gets the market type.
    /// </summary>
    public string? MarketType { get; init; }

    /// <summary>
    /// Gets the market start time.
    /// </summary>
    public DateTime? MarketStartTime { get; init; }

    /// <summary>
    /// Gets the runner description.
    /// </summary>
    public string? RunnerDesc { get; init; }

    /// <summary>
    /// Gets the number of winners.
    /// </summary>
    public int? NumberOfWinners { get; init; }

    /// <summary>
    /// Gets the each way divisor.
    /// </summary>
    public double? EachWayDivisor { get; init; }
}
