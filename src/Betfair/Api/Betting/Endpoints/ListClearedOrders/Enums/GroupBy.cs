using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListClearedOrders.Enums;

/// <summary>
/// Group by options for cleared orders.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<GroupBy>))]
public enum GroupBy
{
    /// <summary>
    /// A roll up of settled profit and loss, commission paid and number of bet orders, on a specified event type.
    /// </summary>
    EventType,

    /// <summary>
    /// A roll up of settled profit and loss, commission paid and number of bet orders, on a specified event.
    /// </summary>
    Event,

    /// <summary>
    /// A roll up of settled profit and loss, commission paid and number of bet orders, on a specified market.
    /// </summary>
    Market,

    /// <summary>
    /// An averaged roll up of settled profit and loss, and number of bets, on the specified side of a specified selection within a specified market, that are either settled or voided.
    /// </summary>
    Side,

    /// <summary>
    /// The profit and loss, side and regulatory information etc., about each individual bet order.
    /// </summary>
    Bet,
}
