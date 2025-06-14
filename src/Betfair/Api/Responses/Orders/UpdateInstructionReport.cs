using Betfair.Api.Requests.Orders;

namespace Betfair.Api.Responses.Orders;

public class UpdateInstructionReport
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
    /// Gets or sets the update instruction associated with the report.
    /// The instruction that was requested.
    /// </summary>
    public UpdateInstruction? Instruction { get; init; }
}
