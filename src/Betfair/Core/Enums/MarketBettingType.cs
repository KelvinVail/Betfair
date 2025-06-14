namespace Betfair.Core.Enums;

/// <summary>
/// Market betting type values.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<MarketBettingType>))]
public enum MarketBettingType
{
    /// <summary>
    /// Odds Market.
    /// </summary>
    Odds,

    /// <summary>
    /// Line Market.
    /// </summary>
    Line,

    /// <summary>
    /// Range Market.
    /// </summary>
    Range,

    /// <summary>
    /// Asian Handicap Market.
    /// </summary>
    AsianHandicapDoubleLines,

    /// <summary>
    /// Asian Single Line Market.
    /// </summary>
    AsianHandicapSingleLines,

    /// <summary>
    /// Fixed Odds Market.
    /// </summary>
    FixedOdds,
}
