namespace Betfair.Api;

public class MarketType
{
    public MarketType(string id) => Id = id;

    public static MarketType AltTotalGoals => new ("ALT_TOTAL_GOALS");

    public static MarketType AsianHandicap => new ("ASIAN_HANDICAP");

    public static MarketType BothTeamsToScore => new ("BOTH_TEAMS_TO_SCORE");

    public static MarketType CombinedTotal => new ("COMBINED_TOTAL");

    public static MarketType CorrectScore => new ("CORRECT_SCORE");

    public static MarketType DoubleChance => new ("DOUBLE_CHANCE");

    public static MarketType DrawNoBet => new ("DRAW_NO_BET");

    public static MarketType FirstHalfGoals05 => new ("FIRST_HALF_GOALS_05");

    public static MarketType FirstHalfGoals15 => new ("FIRST_HALF_GOALS_15");

    public static MarketType FirstHalfGoals25 => new ("FIRST_HALF_GOALS_25");

    public static MarketType Forecast => new ("FORECAST");

    public static MarketType HalfTime => new ("HALF_TIME");

    public static MarketType HalfTimeFullTime => new ("HALF_TIME_FULL_TIME");

    public static MarketType HalfTimeScore => new ("HALF_TIME_SCORE");

    public static MarketType Handicap => new ("HANDICAP");

    public static MarketType MatchOdds => new ("MATCH_ODDS");

    public static MarketType OverUnder05 => new ("OVER_UNDER_05");

    public static MarketType OverUnder15 => new ("OVER_UNDER_15");

    public static MarketType OverUnder25 => new ("OVER_UNDER_25");

    public static MarketType OverUnder35 => new ("OVER_UNDER_35");

    public static MarketType OverUnder45 => new ("OVER_UNDER_45");

    public static MarketType OverUnder55 => new ("OVER_UNDER_55");

    public static MarketType OverUnder65 => new ("OVER_UNDER_65");

    public static MarketType OverUnder75 => new ("OVER_UNDER_75");

    public static MarketType OverUnder85 => new ("OVER_UNDER_85");

    public static MarketType Place => new ("PLACE");

    public static MarketType Win => new ("WIN");

    [DataMember(Name = "marketType")]
    public string Id { get; init; }

    public int MarketCount { get; init; } = -1;

    public static MarketType Create(string id) =>
        new (id);
}
