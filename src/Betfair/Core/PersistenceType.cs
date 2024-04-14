namespace Betfair.Core;

public sealed class PersistenceType
{
    private PersistenceType(string value) => Value = value;

    /// <summary>
    /// Lapse the order when the market is turned in-play.
    /// </summary>
    public static PersistenceType Lapse = new ("LAPSE");

    /// <summary>
    /// Persist the order to in-play.
    /// The bet will be place automatically into the in-play market at the start of the event.
    /// </summary>
    public static PersistenceType Persist = new ("PERSIST");

    /// <summary>
    /// Put the order into the auction (SP) at turn-in-play.
    /// </summary>
    public static PersistenceType MarketOnClose = new("MARKET_ON_CLOSE");

    public string Value { get; }

    public override string ToString() => Value;
}
