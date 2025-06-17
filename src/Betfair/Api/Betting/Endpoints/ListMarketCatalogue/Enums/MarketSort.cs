using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Enums;

/// <summary>
/// Market sort options for listMarketCatalogue.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<MarketSort>))]
public enum MarketSort
{
    /// <summary>
    /// Minimum traded volume.
    /// </summary>
    MinimumTraded,

    /// <summary>
    /// Maximum traded volume.
    /// </summary>
    MaximumTraded,

    /// <summary>
    /// Minimum available to match.
    /// </summary>
    MinimumAvailable,

    /// <summary>
    /// Maximum available to match.
    /// </summary>
    MaximumAvailable,

    /// <summary>
    /// The closest markets based on their expected start time.
    /// </summary>
    FirstToStart,

    /// <summary>
    /// The most distant markets based on their expected start time.
    /// </summary>
    LastToStart,
}
