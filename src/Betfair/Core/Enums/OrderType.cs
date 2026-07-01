using Betfair.Api.Betting.Enums;

namespace Betfair.Core.Enums;

[JsonConverter(typeof(SnakeCaseEnumJsonConverter<OrderType>))]
public enum OrderType
{
    /// <summary>Unknown or not yet set.</summary>
    Unknown = 0,

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
