﻿namespace Betfair.Api.Responses.Orders;

/// <summary>
/// Cleared order.
/// </summary>
public class ClearedOrder
{
    /// <summary>
    /// Gets the event type ID.
    /// </summary>
    public string? EventTypeId { get; internal set; }

    /// <summary>
    /// Gets the event ID.
    /// </summary>
    public string? EventId { get; internal set; }

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
    /// Gets the bet ID.
    /// </summary>
    public string? BetId { get; internal set; }

    /// <summary>
    /// Gets the placed date.
    /// </summary>
    public DateTime PlacedDate { get; internal set; }

    /// <summary>
    /// Gets the persistence type.
    /// </summary>
    public string? PersistenceType { get; internal set; }

    /// <summary>
    /// Gets the order type.
    /// </summary>
    public string? OrderType { get; internal set; }

    /// <summary>
    /// Gets the side.
    /// </summary>
    public string? Side { get; internal set; }

    /// <summary>
    /// Gets the item description.
    /// </summary>
    public ItemDescription? ItemDescription { get; internal set; }

    /// <summary>
    /// Gets the bet outcome.
    /// </summary>
    public string? BetOutcome { get; internal set; }

    /// <summary>
    /// Gets the price requested.
    /// </summary>
    public double PriceRequested { get; internal set; }

    /// <summary>
    /// Gets the settled date.
    /// </summary>
    public DateTime? SettledDate { get; internal set; }

    /// <summary>
    /// Gets the last matched date.
    /// </summary>
    public DateTime? LastMatchedDate { get; internal set; }

    /// <summary>
    /// Gets the bet count.
    /// </summary>
    public int BetCount { get; internal set; }

    /// <summary>
    /// Gets the commission.
    /// </summary>
    public double Commission { get; internal set; }

    /// <summary>
    /// Gets the price matched.
    /// </summary>
    public double PriceMatched { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the price was reduced.
    /// </summary>
    public bool PriceReduced { get; internal set; }

    /// <summary>
    /// Gets the size settled.
    /// </summary>
    public double SizeSettled { get; internal set; }

    /// <summary>
    /// Gets the profit.
    /// </summary>
    public double Profit { get; internal set; }

    /// <summary>
    /// Gets the size cancelled.
    /// </summary>
    public double SizeCancelled { get; internal set; }

    /// <summary>
    /// Gets the customer order reference.
    /// </summary>
    public string? CustomerOrderRef { get; internal set; }

    /// <summary>
    /// Gets the customer strategy reference.
    /// </summary>
    public string? CustomerStrategyRef { get; internal set; }
}
