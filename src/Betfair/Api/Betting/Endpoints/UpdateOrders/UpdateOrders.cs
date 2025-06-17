#pragma warning disable CA2227
namespace Betfair.Api.Betting.Endpoints.UpdateOrders;

/// <summary>
/// Represents a request to update orders on Betfair.
/// Update non-exposure changing fields.
/// </summary>
public class UpdateOrders
{
    public UpdateOrders(string marketId) =>
        MarketId = marketId ?? throw new ArgumentNullException(nameof(marketId));

    /// <summary>
    /// Gets the ID of the market.
    /// The market id these orders are to be placed on.
    /// </summary>
    public string MarketId { get; }

    /// <summary>
    /// Gets or sets the list of instructions for updating orders.
    /// The number of update instructions.  The limit of update instructions per request is 60
    /// </summary>
    public List<UpdateInstruction>? Instructions { get; set; }

    /// <summary>
    /// Gets or sets the customer reference.
    /// Optional parameter allowing the client to pass a unique string (up to 32 chars) that is used to de-dupe mistaken re-submissions.
    /// </summary>
    public string? CustomerRef { get; set; }
}
