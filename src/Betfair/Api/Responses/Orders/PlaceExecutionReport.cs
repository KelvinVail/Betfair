﻿#pragma warning disable CA2227
namespace Betfair.Api.Responses.Orders;

/// <summary>
/// Represents the execution report of a place order operation on Betfair.
/// </summary>
public class PlaceExecutionReport
{
    /// <summary>
    /// Gets or sets the ID of the market.
    /// Echo of marketId passed
    /// </summary>
    public string MarketId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the execution report.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error code, if any, of the execution report.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the customer reference.
    /// Echo of the customerRef if passed.
    /// </summary>
    public string? CustomerRef { get; set; }

    /// <summary>
    /// Gets or sets the list of instruction reports associated with the execution report.
    /// </summary>
    public List<PlaceInstructionReport>? InstructionReports { get; set; }
}
