namespace Betfair.Api.Betting.Endpoints.ReplaceOrders;

/// <summary>
/// Instruction to replace a LIMIT or LIMIT_ON_CLOSE order at a new price.
/// Original order will be cancelled and a new order placed at the new price
/// for the remaining stake.
/// </summary>
public class ReplaceInstruction
{
    /// <summary>
    /// Gets or set the Bet ID.
    /// Unique identifier for the bet.
    /// </summary>
    public string BetId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the New Price.
    /// The price to replace the bet at.
    /// </summary>
    public double NewPrice { get; set; }
}
