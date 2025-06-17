#pragma warning disable CA2227
namespace Betfair.Api.Betting.Endpoints.CancelOrders.Requests;

/// <summary>
/// Cancel all bets OR cancel all bets on a market
/// OR fully or partially cancel particular orders on a market.
/// Only LIMIT orders can be cancelled or partially cancelled once placed.
/// </summary>
public class CancelOrders
{
    /// <summary>
    /// Gets or sets the market ID.
    /// If marketId and betId aren't supplied all bets are cancelled.
    /// Please note: Concurrent requests to cancel all bets will be
    /// rejected until the initial request to cancel all bets is complete.
    /// </summary>
    public string? MarketId { get; set; }

    /// <summary>
    /// Gets or sets the Instructions.
    /// All instructions need to be on the same market.
    /// If not supplied all unmatched bets on the market (if market id is passed)
    /// are fully cancelled.
    /// The limit of cancel instructions per request is 60.
    /// </summary>
    public List<CancelInstruction>? Instructions { get; set; }

    /// <summary>
    /// Gets or sets the customer reference.
    /// Optional parameter allowing the client to pass a unique string (up to 32 chars) that is used to de-dupe mistaken re-submissions.
    /// CustomerRef can contain: upper/lower chars, digits, chars : - . _ + * : ; ~ only.
    /// Please note: There is a time window associated with the de-duplication of duplicate submissions which is 60 seconds.
    /// NB: This field does not persist into the placeOrders response/Order Stream API and should not be confused with customerOrderRef, which is separate field that can be sent in the PlaceInstruction.
    /// </summary>
    public string? CustomerRef { get; set; }
}
