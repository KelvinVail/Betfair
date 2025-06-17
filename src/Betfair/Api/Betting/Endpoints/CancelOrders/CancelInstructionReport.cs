namespace Betfair.Api.Betting.Endpoints.CancelOrders;

public class CancelInstructionReport
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
    /// Gets or sets the cancel instruction associated with the report.
    /// The instruction that was requested.
    /// </summary>
    public CancelInstruction? Instruction { get; init; }

    /// <summary>
    /// Gets or set the Size Cancelled.
    /// </summary>
    public double SizeCancelled { get; init; }

    /// <summary>
    /// Gets or set the Cancelled Date.
    /// </summary>
    public DateTimeOffset CancelledDate { get; init; }
}
