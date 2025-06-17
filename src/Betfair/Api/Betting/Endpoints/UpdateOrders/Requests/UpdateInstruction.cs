using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.UpdateOrders.Requests;

/// <summary>
/// Represents an instruction to update an order on Betfair.
/// Update non-exposure changing fields
/// </summary>
public class UpdateInstruction
{
    /// <summary>
    /// Gets or sets the ID of the bet to update.
    /// Unique identifier for the bet.
    /// </summary>
    public string BetId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new persistence type for the bet.
    /// The new persistence type to update this bet to. Defaults to Lapse if not specified.
    /// </summary>
    public PersistenceType NewPersistenceType { get; set; } = PersistenceType.Lapse;
}
