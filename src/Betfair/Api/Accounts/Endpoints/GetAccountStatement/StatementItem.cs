﻿namespace Betfair.Api.Accounts.Endpoints.GetAccountStatement;

/// <summary>
/// Statement item.
/// </summary>
public class StatementItem
{
    /// <summary>
    /// Gets the reference ID.
    /// </summary>
    public string? RefId { get; init; }

    /// <summary>
    /// Gets the item date.
    /// </summary>
    public DateTime ItemDate { get; init; }

    /// <summary>
    /// Gets the amount.
    /// </summary>
    public double Amount { get; init; }

    /// <summary>
    /// Gets the balance.
    /// </summary>
    public double Balance { get; init; }

    /// <summary>
    /// Gets the item class.
    /// </summary>
    public string? ItemClass { get; init; }

    /// <summary>
    /// Gets the item class data.
    /// </summary>
    public Dictionary<string, string>? ItemClassData { get; init; }

    /// <summary>
    /// Gets the legacy data.
    /// </summary>
    public StatementLegacyData? LegacyData { get; init; }
}
