﻿namespace Betfair.Api.Accounts.Endpoints.GetAccountStatement.Responses;

/// <summary>
/// Statement legacy data.
/// </summary>
public class StatementLegacyData
{
    /// <summary>
    /// Gets the average price.
    /// </summary>
    public double? AvgPrice { get; init; }

    /// <summary>
    /// Gets the bet size.
    /// </summary>
    public double? BetSize { get; init; }

    /// <summary>
    /// Gets the bet type.
    /// </summary>
    public string? BetType { get; init; }

    /// <summary>
    /// Gets the event ID.
    /// </summary>
    public string? EventId { get; init; }

    /// <summary>
    /// Gets the event type ID.
    /// </summary>
    public string? EventTypeId { get; init; }

    /// <summary>
    /// Gets the full market name.
    /// </summary>
    public string? FullMarketName { get; init; }

    /// <summary>
    /// Gets the gross bet amount.
    /// </summary>
    public double? GrossBetAmount { get; init; }

    /// <summary>
    /// Gets the market name.
    /// </summary>
    public string? MarketName { get; init; }

    /// <summary>
    /// Gets the market type.
    /// </summary>
    public string? MarketType { get; init; }

    /// <summary>
    /// Gets the placed date.
    /// </summary>
    public DateTime? PlacedDate { get; init; }

    /// <summary>
    /// Gets the selection ID.
    /// </summary>
    public long? SelectionId { get; init; }

    /// <summary>
    /// Gets the selection name.
    /// </summary>
    public string? SelectionName { get; init; }

    /// <summary>
    /// Gets the start date.
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Gets the transaction ID.
    /// </summary>
    public string? TransactionId { get; init; }

    /// <summary>
    /// Gets the transaction type.
    /// </summary>
    public string? TransactionType { get; init; }

    /// <summary>
    /// Gets the win/lose status.
    /// </summary>
    public string? WinLose { get; init; }
}
