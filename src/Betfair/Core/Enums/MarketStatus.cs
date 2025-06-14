namespace Betfair.Core.Enums;

/// <summary>
/// Market status values.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<MarketStatus>))]
public enum MarketStatus
{
    /// <summary>
    /// Inactive Market.
    /// </summary>
    Inactive,

    /// <summary>
    /// Open Market.
    /// </summary>
    Open,

    /// <summary>
    /// Suspended Market.
    /// </summary>
    Suspended,

    /// <summary>
    /// Closed Market.
    /// </summary>
    Closed,
}
