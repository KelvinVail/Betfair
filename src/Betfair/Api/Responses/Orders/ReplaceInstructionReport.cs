namespace Betfair.Api.Responses.Orders;

public class ReplaceInstructionReport
{
    /// <summary>
    /// Gets or sets the status of the instruction report.
    /// Whether the command succeeded or failed.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the error code, if any, of the instruction report.
    /// Cause of failure, or null if command succeeds
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the cancel instruction report.
    /// Cancellation report for the original order.
    /// </summary>
    public CancelInstructionReport? CancelInstructionReport { get; set; }

    /// <summary>
    /// Gets or sets the place instruction report.
    /// Placement report for the new order.
    /// </summary>
    public PlaceInstructionReport? PlaceInstructionReport { get; set; }
}
