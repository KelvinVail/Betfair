#pragma warning disable CA2227
namespace Betfair.Api.Betting.Endpoints.UpdateOrders.Responses;

public class UpdateExecutionReport
{
    /// <summary>
    /// Gets or sets the customer reference.
    /// Echo of the customerRef if passed.
    /// </summary>
    public string? CustomerRef { get; init; }

    /// <summary>
    /// Gets or sets the status of the execution report.
    /// Whether the command succeeded or failed.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the error code, if any, of the execution report.
    /// Cause of failure, or null if command succeeds.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets or sets the ID of the market.
    /// Echo of marketId passed
    /// </summary>
    public string? MarketId { get; init; }

    /// <summary>
    /// Gets or sets the list of instruction reports associated with the execution report.
    /// </summary>
    public List<UpdateInstructionReport>? InstructionReports { get; init; }
}
