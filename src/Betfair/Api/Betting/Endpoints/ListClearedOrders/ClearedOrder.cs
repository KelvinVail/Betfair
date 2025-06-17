namespace Betfair.Api.Betting.Endpoints.ListClearedOrders;

/// <summary>
/// Cleared order.
/// </summary>
public class ClearedOrder
{
    /// <summary>
    /// Gets the event type ID.
    /// </summary>
    public string? EventTypeId { get; init; }

    /// <summary>
    /// Gets the event ID.
    /// </summary>
    public string? EventId { get; init; }

    /// <summary>
    /// Gets the market ID.
    /// </summary>
    public string? MarketId { get; init; }

    /// <summary>
    /// Gets the selection ID.
    /// </summary>
    public long SelectionId { get; init; }

    /// <summary>
    /// Gets the handicap.
    /// </summary>
    public double Handicap { get; init; }

    /// <summary>
    /// Gets the bet ID.
    /// </summary>
    public string? BetId { get; init; }

    /// <summary>
    /// Gets the placed date.
    /// </summary>
    public DateTime PlacedDate { get; init; }

    /// <summary>
    /// Gets the persistence type.
    /// </summary>
    public string? PersistenceType { get; init; }

    /// <summary>
    /// Gets the order type.
    /// </summary>
    public string? OrderType { get; init; }

    /// <summary>
    /// Gets the side.
    /// </summary>
    public string? Side { get; init; }

    /// <summary>
    /// Gets the item description.
    /// </summary>
    public ItemDescription? ItemDescription { get; init; }

    /// <summary>
    /// Gets the bet outcome.
    /// </summary>
    public string? BetOutcome { get; init; }

    /// <summary>
    /// Gets the price requested.
    /// </summary>
    public double PriceRequested { get; init; }

    /// <summary>
    /// Gets the settled date.
    /// </summary>
    public DateTime? SettledDate { get; init; }

    /// <summary>
    /// Gets the last matched date.
    /// </summary>
    public DateTime? LastMatchedDate { get; init; }

    /// <summary>
    /// Gets the bet count.
    /// </summary>
    public int BetCount { get; init; }

    /// <summary>
    /// Gets the commission.
    /// </summary>
    public double Commission { get; init; }

    /// <summary>
    /// Gets the price matched.
    /// </summary>
    public double PriceMatched { get; init; }

    /// <summary>
    /// Gets a value indicating whether the price was reduced.
    /// </summary>
    public bool PriceReduced { get; init; }

    /// <summary>
    /// Gets the size settled.
    /// </summary>
    public double SizeSettled { get; init; }

    /// <summary>
    /// Gets the profit.
    /// </summary>
    public double Profit { get; init; }

    /// <summary>
    /// Gets the size cancelled.
    /// </summary>
    public double SizeCancelled { get; init; }

    /// <summary>
    /// Gets the customer order reference.
    /// </summary>
    public string? CustomerOrderRef { get; init; }

    /// <summary>
    /// Gets the customer strategy reference.
    /// </summary>
    public string? CustomerStrategyRef { get; init; }
}
