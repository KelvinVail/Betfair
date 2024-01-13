namespace Betfair.Core;

/// <summary>
/// The betting type of the market (i.e. Odds, Asian Handicap Singles, or Asian Handicap Doubles)
/// </summary>
public sealed class BettingType
{
    private BettingType(string id) => Id = id;

    /// <summary>
    /// Gets Odds Market - Any market that doesn't fit any of the other betting types.
    /// </summary>
    public static BettingType Odds => new ("ODDS");

    /// <summary>
    /// Gets Line Market - LINE markets operate at even-money odds of 2.0.
    /// However, price for these markets refers to the line positions available as defined by the markets min-max range and interval steps.
    /// Customers either Buy a line (LAY bet, winning if outcome is greater than the taken line (price))
    /// or Sell a line (BACK bet, winning if outcome is less than the taken line (price)).
    /// If settled outcome equals the taken line, stake is returned.
    /// </summary>
    public static BettingType Line => new ("LINE");

    /// <summary>
    /// Gets Asian Handicap Market - A traditional Asian handicap market. Can be identified by marketType ASIAN_HANDICAP.
    /// </summary>
    public static BettingType AsianHandicapDoubles => new ("ASIAN_HANDICAP_DOUBLE_LINE");

    /// <summary>
    /// Gets Asian Single Line Market - A market in which there can be 0 or multiple winners. e.g. marketType TOTAL_GOALS.
    /// </summary>
    public static BettingType AsianHandicapSingles => new ("ASIAN_HANDICAP_SINGLE_LINE");

    public string Id { get; }
}
