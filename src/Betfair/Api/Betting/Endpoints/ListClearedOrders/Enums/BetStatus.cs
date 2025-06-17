using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListClearedOrders.Enums;

/// <summary>
/// Bet status for cleared orders.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<BetStatus>))]
public enum BetStatus
{
    /// <summary>
    /// A matched bet that was settled normally.
    /// </summary>
    Settled,

    /// <summary>
    /// A matched bet that was subsequently voided by Betfair, before, during or after settlement.
    /// </summary>
    Voided,

    /// <summary>
    /// Unmatched bet that was cancelled by Betfair (for example at turn in play).
    /// </summary>
    Lapsed,

    /// <summary>
    /// Unmatched bet that was cancelled by an explicit customer action.
    /// </summary>
    Cancelled,
}
