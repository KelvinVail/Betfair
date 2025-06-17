namespace Betfair.Api.Betting.Enums;

[JsonConverter(typeof(SnakeCaseEnumJsonConverter<OrderType>))]
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