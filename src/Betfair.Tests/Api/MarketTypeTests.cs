using Betfair.Api;

namespace Betfair.Tests.Api;

public class MarketTypeTests
{
    [Fact]
    public void CanCreateMarketTypeAltTotalGoals() =>
        MarketType.AltTotalGoals.Id.Should().Be("ALT_TOTAL_GOALS");

    [Fact]
    public void CanCreateMarketTypeAsianHandicap() =>
        MarketType.AsianHandicap.Id.Should().Be("ASIAN_HANDICAP");

    [Fact]
    public void CanCreateMarketTypeBothTeamsToScore() =>
        MarketType.BothTeamsToScore.Id.Should().Be("BOTH_TEAMS_TO_SCORE");

    [Fact]
    public void CanCreateMarketTypeCombinedTotal() =>
        MarketType.CombinedTotal.Id.Should().Be("COMBINED_TOTAL");

    [Fact]
    public void CanCreateMarketTypeCorrectScore() =>
        MarketType.CorrectScore.Id.Should().Be("CORRECT_SCORE");

    [Fact]
    public void CanCreateMarketTypeDoubleChance() =>
        MarketType.DoubleChance.Id.Should().Be("DOUBLE_CHANCE");

    [Fact]
    public void CanCreateMarketTypeDrawNoBet() =>
        MarketType.DrawNoBet.Id.Should().Be("DRAW_NO_BET");

    [Fact]
    public void CanCreateMarketTypeFirstHalfGoals05() =>
        MarketType.FirstHalfGoals05.Id.Should().Be("FIRST_HALF_GOALS_05");

    [Fact]
    public void CanCreateMarketTypeFirstHalfGoals15() =>
        MarketType.FirstHalfGoals15.Id.Should().Be("FIRST_HALF_GOALS_15");

    [Fact]
    public void CanCreateMarketTypeFirstHalfGoals25() =>
        MarketType.FirstHalfGoals25.Id.Should().Be("FIRST_HALF_GOALS_25");

    [Fact]
    public void CanCreateMarketTypeForecast() =>
        MarketType.Forecast.Id.Should().Be("FORECAST");

    [Fact]
    public void CanCreateMarketTypeHalfTime() =>
        MarketType.HalfTime.Id.Should().Be("HALF_TIME");

    [Fact]
    public void CanCreateMarketTypeHalfTimeFullTime() =>
        MarketType.HalfTimeFullTime.Id.Should().Be("HALF_TIME_FULL_TIME");

    [Fact]
    public void CanCreateMarketTypeHalfTimeScore() =>
        MarketType.HalfTimeScore.Id.Should().Be("HALF_TIME_SCORE");

    [Fact]
    public void CanCreateMarketTypeHandicap() =>
        MarketType.Handicap.Id.Should().Be("HANDICAP");

    [Fact]
    public void CanCreateMarketTypeMatchOdds() =>
        MarketType.MatchOdds.Id.Should().Be("MATCH_ODDS");

    [Fact]
    public void CanCreateMarketTypeOverUnder05() =>
        MarketType.OverUnder05.Id.Should().Be("OVER_UNDER_05");

    [Fact]
    public void CanCreateMarketTypeOverUnder15() =>
        MarketType.OverUnder15.Id.Should().Be("OVER_UNDER_15");

    [Fact]
    public void CanCreateMarketTypeOverUnder25() =>
        MarketType.OverUnder25.Id.Should().Be("OVER_UNDER_25");

    [Fact]
    public void CanCreateMarketTypeOverUnder35() =>
        MarketType.OverUnder35.Id.Should().Be("OVER_UNDER_35");

    [Fact]
    public void CanCreateMarketTypeOverUnder45() =>
        MarketType.OverUnder45.Id.Should().Be("OVER_UNDER_45");

    [Fact]
    public void CanCreateMarketTypeOverUnder55() =>
        MarketType.OverUnder55.Id.Should().Be("OVER_UNDER_55");

    [Fact]
    public void CanCreateMarketTypeOverUnder65() =>
        MarketType.OverUnder65.Id.Should().Be("OVER_UNDER_65");

    [Fact]
    public void CanCreateMarketTypeOverUnder75() =>
        MarketType.OverUnder75.Id.Should().Be("OVER_UNDER_75");

    [Fact]
    public void CanCreateMarketTypeOverUnder85() =>
        MarketType.OverUnder85.Id.Should().Be("OVER_UNDER_85");

    [Fact]
    public void CanCreateMarketTypePlace() =>
        MarketType.Place.Id.Should().Be("PLACE");

    [Fact]
    public void CanCreateMarketTypeWin() =>
        MarketType.Win.Id.Should().Be("WIN");

    [Theory]
    [InlineData("OTHER_PLACE")]
    [InlineData("ANTEPOST_WIN")]
    public void CustomMarketTypesCanBeCreated(string id)
    {
        var type = MarketType.Of(id);

        type.Id.Should().Be(id);
    }
}
