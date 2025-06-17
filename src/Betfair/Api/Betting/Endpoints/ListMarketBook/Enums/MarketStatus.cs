using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;

/// <summary>
/// Market status values.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<MarketStatus>))]
public enum MarketStatus
{
    /// <summary>
    /// The market has been created but isn't yet available.
    /// </summary>
    Inactive,

    /// <summary>
    /// The market is open for betting.
    /// </summary>
    Open,

    /// <summary>
    /// The market is suspended and not available for betting.
    /// </summary>
    Suspended,

    /// <summary>
    /// The market has been settled and is no longer available for betting.
    /// </summary>
    Closed,
}
