using Betfair.Api.Betting.Endpoints.CancelOrders;
using Betfair.Api.Betting.Endpoints.PlaceOrders;

namespace Betfair.Api.Betting.Endpoints.ReplaceOrders;

public class ReplaceInstructionReport
{
    /// <summary>
    /// Gets or sets the status of the instruction report.
    /// Whether the command succeeded or failed.
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Gets or sets the error code, if any, of the instruction report.
    /// Cause of failure, or null if command succeeds
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets or sets the cancel instruction report.
    /// Cancellation report for the original order.
    /// </summary>
    public CancelInstructionReport? CancelInstructionReport { get; init; }

    /// <summary>
    /// Gets or sets the place instruction report.
    /// Placement report for the new order.
    /// </summary>
    public PlaceInstructionReport? PlaceInstructionReport { get; init; }
}
