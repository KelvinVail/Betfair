namespace Betfair.Core;

[JsonConverter(typeof(UpperCaseEnumJsonConverter<OrderType>))]
public enum OrderType
{
    /// <summary>
    /// A normal exchange limit order for immediate execution.
    /// </summary>
    Limit,

    /// <summary>
    /// Limit order for the auction (SP).
    /// </summary>
    LimitOnClose,

    /// <summary>
    /// Market order for the auction (SP).
    /// </summary>
    MarketOnClose,
}