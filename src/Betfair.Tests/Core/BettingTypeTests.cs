using Betfair.Core;

namespace Betfair.Tests.Core;

public class BettingTypeTests
{
    [Fact]
    public void CanCreateBettingTypeOdds() =>
        BettingType.Odds.Id.Should().Be("ODDS");

    [Fact]
    public void CanCreateBettingTypeLine() =>
        BettingType.Line.Id.Should().Be("LINE");

    [Fact]
    public void CanCreateBettingTypeAsianHandicapDoubles() =>
        BettingType.AsianHandicapDoubles.Id.Should().Be("ASIAN_HANDICAP_DOUBLE_LINE");

    [Fact]
    public void CanCreateBettingTypeAsianHandicapSingles() =>
        BettingType.AsianHandicapSingles.Id.Should().Be("ASIAN_HANDICAP_SINGLE_LINE");
}
