namespace Betfair.Core;

public sealed class OrderType
{
    private OrderType(string value) => Value = value;

    /// <summary>
    /// Gets the Limit order type.
    /// A normal exchange limit order for immediate execution.
    /// </summary>
    /// <value>The Limit order type.</value>
    public static OrderType Limit { get; } = new ("LIMIT");

    /// <summary>
    /// Gets the Limit On Close order type.
    /// Limit order for the auction (SP).
    /// </summary>
    /// <value>The Limit On CLose order type.</value>
    public static OrderType LimitOnClose { get; } = new ("LIMIT_ON_CLOSE");

    /// <summary>
    /// Gets the Market On Close order type.
    /// Market order for the auction (SP).
    /// </summary>
    /// <value>The Market On Close order type.</value>
    public static OrderType MarketOnClose { get; } = new ("MARKET_ON_CLOSE");

    public string Value { get; }

    public override string ToString() => Value;
}
