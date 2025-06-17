namespace Betfair.Api.Betting.Endpoints.CancelOrders.Requests;

/// <summary>
/// Instruction to fully or partially cancel an order (only applies to LIMIT orders).
/// Please note: the CancelInstruction report won't be returned for
/// account level cancel instructions.
/// </summary>
public class CancelInstruction
{
    /// <summary>
    /// Gets or sets the Bet ID.
    /// </summary>
    public string? BetId { get; set; }

    /// <summary>
    /// Gets or sets the size reduction.
    /// If supplied then this is a partial cancel.
    /// Should be set to 'null' if no size reduction is required.
    /// </summary>
    public double? SizeReduction { get; set; }
}
