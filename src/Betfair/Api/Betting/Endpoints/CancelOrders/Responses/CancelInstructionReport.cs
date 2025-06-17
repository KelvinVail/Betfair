using Betfair.Api.Betting.Endpoints.CancelOrders.Requests;

namespace Betfair.Api.Betting.Endpoints.CancelOrders.Responses;

public class CancelInstructionReport
{
    /// <summary>
    /// Gets the status of the instruction report.
    /// Whether the command succeeded or failed.
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Gets the error code, if any, of the instruction report.
    /// Cause of failure, or null if command succeeds
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets the cancel instruction associated with the report.
    /// The instruction that was requested.
    /// </summary>
    public CancelInstruction? Instruction { get; init; }

    /// <summary>
    /// Gets the Size Cancelled.
    /// </summary>
    public double SizeCancelled { get; init; }

    /// <summary>
    /// Gets the Cancelled Date.
    /// </summary>
    public DateTimeOffset CancelledDate { get; init; }
}
