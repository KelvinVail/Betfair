﻿namespace Betfair.Api.Responses.Orders;

/// <summary>
/// Item description for cleared orders.
/// </summary>
public class ItemDescription
{
    /// <summary>
    /// Gets the event type description.
    /// </summary>
    public string? EventTypeDesc { get; internal set; }

    /// <summary>
    /// Gets the event description.
    /// </summary>
    public string? EventDesc { get; internal set; }

    /// <summary>
    /// Gets the market description.
    /// </summary>
    public string? MarketDesc { get; internal set; }

    /// <summary>
    /// Gets the market type.
    /// </summary>
    public string? MarketType { get; internal set; }

    /// <summary>
    /// Gets the market start time.
    /// </summary>
    public DateTime? MarketStartTime { get; internal set; }

    /// <summary>
    /// Gets the runner description.
    /// </summary>
    public string? RunnerDesc { get; internal set; }

    /// <summary>
    /// Gets the number of winners.
    /// </summary>
    public int? NumberOfWinners { get; internal set; }

    /// <summary>
    /// Gets the each way divisor.
    /// </summary>
    public double? EachWayDivisor { get; internal set; }
}
