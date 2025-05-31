﻿namespace Betfair.Api.Responses.Account;

/// <summary>
/// Statement item.
/// </summary>
public class StatementItem
{
    /// <summary>
    /// Gets the reference ID.
    /// </summary>
    public string? RefId { get; internal set; }

    /// <summary>
    /// Gets the item date.
    /// </summary>
    public DateTime ItemDate { get; internal set; }

    /// <summary>
    /// Gets the amount.
    /// </summary>
    public double Amount { get; internal set; }

    /// <summary>
    /// Gets the balance.
    /// </summary>
    public double Balance { get; internal set; }

    /// <summary>
    /// Gets the item class.
    /// </summary>
    public string? ItemClass { get; internal set; }

    /// <summary>
    /// Gets the item class data.
    /// </summary>
    public Dictionary<string, object>? ItemClassData { get; internal set; }

    /// <summary>
    /// Gets the legacy data.
    /// </summary>
    public StatementLegacyData? LegacyData { get; internal set; }
}
