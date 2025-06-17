#pragma warning disable CA2227
namespace Betfair.Api.Betting.Endpoints.CancelOrders.Responses;

public class CancelExecutionReport
{
    /// <summary>
    /// Gets the ID of the market.
    /// Echo of marketId passed.
    /// </summary>
    public string MarketId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the status of the execution report.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets the error code, if any, of the execution report.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets the customer reference.
    /// Echo of the customerRef if passed.
    /// </summary>
    public string? CustomerRef { get; init; }

    /// <summary>
    /// Gets the list of instruction reports associated with the execution report.
    /// </summary>
    public List<CancelInstructionReport>? InstructionReports { get; init; }
}
