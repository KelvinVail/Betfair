// ReSharper disable InconsistentNaming
namespace Betfair.Core;

[JsonConverter(typeof(UpperCaseEnumJsonConverter<OrderType>))]
[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "The underscores are required by Betfair.")]
public enum OrderType
{
    /// <summary>
    /// A normal exchange limit order for immediate execution.
    /// </summary>
    Limit,

    /// <summary>
    /// Limit order for the auction (SP).
    /// </summary>
    Limit_On_Close,

    /// <summary>
    /// Market order for the auction (SP).
    /// </summary>
    Market_On_Close,
}