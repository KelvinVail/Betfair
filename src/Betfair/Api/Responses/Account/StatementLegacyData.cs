﻿namespace Betfair.Api.Responses.Account;

/// <summary>
/// Statement legacy data.
/// </summary>
public class StatementLegacyData
{
    /// <summary>
    /// Gets the average price.
    /// </summary>
    public double? AvgPrice { get; internal set; }

    /// <summary>
    /// Gets the bet size.
    /// </summary>
    public double? BetSize { get; internal set; }

    /// <summary>
    /// Gets the bet type.
    /// </summary>
    public string? BetType { get; internal set; }

    /// <summary>
    /// Gets the event ID.
    /// </summary>
    public string? EventId { get; internal set; }

    /// <summary>
    /// Gets the event type ID.
    /// </summary>
    public string? EventTypeId { get; internal set; }

    /// <summary>
    /// Gets the full market name.
    /// </summary>
    public string? FullMarketName { get; internal set; }

    /// <summary>
    /// Gets the gross bet amount.
    /// </summary>
    public double? GrossBetAmount { get; internal set; }

    /// <summary>
    /// Gets the market name.
    /// </summary>
    public string? MarketName { get; internal set; }

    /// <summary>
    /// Gets the market type.
    /// </summary>
    public string? MarketType { get; internal set; }

    /// <summary>
    /// Gets the placed date.
    /// </summary>
    public DateTime? PlacedDate { get; internal set; }

    /// <summary>
    /// Gets the selection ID.
    /// </summary>
    public long? SelectionId { get; internal set; }

    /// <summary>
    /// Gets the selection name.
    /// </summary>
    public string? SelectionName { get; internal set; }

    /// <summary>
    /// Gets the start date.
    /// </summary>
    public DateTime? StartDate { get; internal set; }

    /// <summary>
    /// Gets the transaction ID.
    /// </summary>
    public string? TransactionId { get; internal set; }

    /// <summary>
    /// Gets the transaction type.
    /// </summary>
    public string? TransactionType { get; internal set; }

    /// <summary>
    /// Gets the win/lose status.
    /// </summary>
    public string? WinLose { get; internal set; }
}
