﻿namespace Betfair.Api.Responses.Orders;

/// <summary>
/// Current order.
/// </summary>
public class CurrentOrder
{
    /// <summary>
    /// Gets the bet ID.
    /// </summary>
    public string? BetId { get; internal set; }

    /// <summary>
    /// Gets the market ID.
    /// </summary>
    public string? MarketId { get; internal set; }

    /// <summary>
    /// Gets the selection ID.
    /// </summary>
    public long SelectionId { get; internal set; }

    /// <summary>
    /// Gets the handicap.
    /// </summary>
    public double Handicap { get; internal set; }

    /// <summary>
    /// Gets the price and size.
    /// </summary>
    public PriceSize? PriceSize { get; internal set; }

    /// <summary>
    /// Gets the BSP liability.
    /// </summary>
    public double BspLiability { get; internal set; }

    /// <summary>
    /// Gets the side.
    /// </summary>
    public string? Side { get; internal set; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    public string? Status { get; internal set; }

    /// <summary>
    /// Gets the persistence type.
    /// </summary>
    public string? PersistenceType { get; internal set; }

    /// <summary>
    /// Gets the order type.
    /// </summary>
    public string? OrderType { get; internal set; }

    /// <summary>
    /// Gets the placed date.
    /// </summary>
    public DateTime PlacedDate { get; internal set; }

    /// <summary>
    /// Gets the matched date.
    /// </summary>
    public DateTime? MatchedDate { get; internal set; }

    /// <summary>
    /// Gets the average price matched.
    /// </summary>
    public double AveragePriceMatched { get; internal set; }

    /// <summary>
    /// Gets the size matched.
    /// </summary>
    public double SizeMatched { get; internal set; }

    /// <summary>
    /// Gets the size remaining.
    /// </summary>
    public double SizeRemaining { get; internal set; }

    /// <summary>
    /// Gets the size lapsed.
    /// </summary>
    public double SizeLapsed { get; internal set; }

    /// <summary>
    /// Gets the size cancelled.
    /// </summary>
    public double SizeCancelled { get; internal set; }

    /// <summary>
    /// Gets the size voided.
    /// </summary>
    public double SizeVoided { get; internal set; }

    /// <summary>
    /// Gets the customer order reference.
    /// </summary>
    public string? CustomerOrderRef { get; internal set; }

    /// <summary>
    /// Gets the customer strategy reference.
    /// </summary>
    public string? CustomerStrategyRef { get; internal set; }
}
