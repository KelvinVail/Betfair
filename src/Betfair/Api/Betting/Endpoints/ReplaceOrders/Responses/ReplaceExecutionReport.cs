namespace Betfair.Api.Betting.Endpoints.ReplaceOrders.Responses;

/// <summary>
/// Represents the execution report of a replace order operation on Betfair.
/// </summary>
public class ReplaceExecutionReport
{
    /// <summary>
    /// Gets or sets the ID of the market.
    /// Echo of marketId passed
    /// </summary>
    public string MarketId { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the execution report.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the error code, if any, of the execution report.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets or sets the customer reference.
    /// Echo of the customerRef if passed.
    /// </summary>
    public string? CustomerRef { get; init; }

    /// <summary>
    /// Gets or sets the list of instruction reports associated with the execution report.
    /// </summary>
    public List<ReplaceInstructionReport>? InstructionReports { get; init; }
}
