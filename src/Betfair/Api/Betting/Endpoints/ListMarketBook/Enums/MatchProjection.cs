using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;

/// <summary>
/// Match projection for orders.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<MatchProjection>))]
public enum MatchProjection
{
    /// <summary>
    /// No rollup, return raw fragments.
    /// </summary>
    NoRollup,

    /// <summary>
    /// Rollup matched amounts by distinct matched prices per side.
    /// </summary>
    RolledUpByPrice,

    /// <summary>
    /// Rollup matched amounts by average matched price per side.
    /// </summary>
    RolledUpByAvgPrice,
}
