namespace Betfair.Core.Enums;

/// <summary>
/// Group by options for cleared orders.
/// </summary>
public enum GroupBy
{
    /// <summary>
    /// Group by event type.
    /// </summary>
    EventType,

    /// <summary>
    /// Group by event.
    /// </summary>
    Event,

    /// <summary>
    /// Group by market.
    /// </summary>
    Market,

    /// <summary>
    /// Group by side.
    /// </summary>
    Side,

    /// <summary>
    /// Group by bet.
    /// </summary>
    Bet
}
