using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class PriceTests
{
    [Fact]
    public void AnObjectRepresentingAPriceOfNoneCanBeCreated()
    {
        var price = Price.None;

        price.Tick.Should().Be(-1);
        price.DecimalOdds.Should().Be(-1);
    }

    [Theory]
    [InlineData(1.00)]
    [InlineData(5.45)]
    [InlineData(49)]
    public void InvalidDecimalOddsAreFlagged(double decimalOdds)
    {
        var price = Price.Of(decimalOdds);

        price.IsValidPrice.Should().BeFalse();
    }

    [Theory]
    [InlineData(1.01)]
    [InlineData(10.5)]
    [InlineData(770)]
    public void TwoPricesWithTheSameDecimalOddsAreEqual(double decimalOdds)
    {
        var price1 = Price.Of(decimalOdds);
        var price2 = Price.Of(decimalOdds);

        price1.Should().BeEquivalentTo(price2);
    }

    [Theory]
    [InlineData(1.01)]
    [InlineData(10.5)]
    [InlineData(770)]
    public void TwoPricesWithDifferentDecimalOddsAreNotEqual(double decimalOdds)
    {
        var price1 = Price.Of(decimalOdds);
        var price2 = Price.Of(2);

        price1.Should().NotBe(price2);
    }

    [Theory]
    [InlineData(1.01)]
    [InlineData(10.5)]
    [InlineData(770)]
    public void ToStringDisplaysTheDecimalOddsOfThePrice(double decimalOdds)
    {
        var price = Price.Of(decimalOdds);

        price.ToString().Should().Be($"{decimalOdds:#.##}");
    }

    [Fact]
    public void TicksBetweenReturnsZeroIfComparedToNull()
    {
        var price = Price.Of(1.01);

        price.TicksBetween(null!).Should().Be(0);
    }

    [Theory]
    [InlineData(1.01, 1.02, 1)]
    [InlineData(3.05, 3.65, 12)]
    [InlineData(32, 110, 20)]
    [InlineData(1.02, 1.01, -1)]
    [InlineData(3.65, 3.05, -12)]
    [InlineData(110, 32, -20)]
    public void TicksBetweenTwoPricesCanBeCalculated(double start, double end, int diff)
    {
        var price1 = Price.Of(start);
        var price2 = Price.Of(end);

        var result = price1.TicksBetween(price2);

        result.Should().Be(diff);
    }

    [Theory]
    [InlineData(1.01)]
    [InlineData(1000)]
    public void ChanceIsCalculatedCorrectly(double value)
    {
        var price = Price.Of(value);

        price.Chance.Should().Be(1 / value);
    }

    [Theory]
    [InlineData(1.01, 1, 1.02)]
    [InlineData(1.02, 1, 1.03)]
    [InlineData(1.9, 10, 2.00)]
    [InlineData(2.02, 1, 2.04)]
    [InlineData(3.05, 1, 3.1)]
    [InlineData(4.1, 1, 4.2)]
    [InlineData(6.2, 1, 6.4)]
    [InlineData(10.5, 1, 11)]
    [InlineData(20, 1, 21)]
    [InlineData(32, 1, 34)]
    [InlineData(55, 1, 60)]
    [InlineData(110, 1, 120)]
    [InlineData(2.0, 1, 2.02)]
    [InlineData(2.02, -1, 2.0)]
    [InlineData(1.01, -1, 1.01)]
    [InlineData(1.01, -10, 1.01)]
    [InlineData(1000, 1, 1000)]
    [InlineData(1000, 10, 1000)]
    [InlineData(1000, -1, 990)]
    public void AddsTicksCorrectly(double start, int ticks, double expected)
    {
        var price = Price.Of(start);

        var actual = price.AddTicks(ticks);

        actual.DecimalOdds.Should().Be(expected);
    }

    [Theory]
    [InlineData(1.01, 1)]
    [InlineData(2.68, 1)]
    [InlineData(5, 1)]
    [InlineData(5.1, 1)]
    [InlineData(6.4, 1)]
    [InlineData(11.5, 0.87)]
    [InlineData(220, 0.05)]
    [InlineData(990, 0.02)]
    [InlineData(1000, 0.01)]
    public void MinimumSizeIsOneOrTheLowestSizeThatWouldReturnTenWhicheverIsLower(double price, double expected)
    {
        // Minimum size is £1 or the lowest size that would return £10, whichever is lower.
        // For example, to return £10 at a price of 1.01 the size would need to be £9.91 (10 / 1.01), £1 is lower than £9.91 so the minimum size is £1.
        // To return £10 at a price of 11.5 the size would need to be £0.87 (10 / 11.5), £0.87 is lower than £1 so the minimum size is £0.87.
        var actual = Price.Of(price).MinimumSize;

        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(1.01, 0)]
    [InlineData(1000, 349)]
    [InlineData(3.5, 159)]
    public void TickIsCalculatedCorrectly(double odds, int tick)
    {
        var price = Price.Of(odds);

        price.Tick.Should().Be(tick);
    }

    [Theory]
    [InlineData(1.001, 0)]
    [InlineData(1001, 349)]
    [InlineData(220.5, 271)]
    [InlineData(19.1, 227)]
    [InlineData(50.9, 249)]
    public void InvalidPricesAreDisplayedAtTheNearestTick(double odds, int tick) =>
        TickIsCalculatedCorrectly(odds, tick);

    [Theory]
    [InlineData(8.6, 16.2, 7.21)]
    [InlineData(2, 48.1, 1.04)]
    [InlineData(500, 29.8, 351)]
    [InlineData(5.2, 3.2, 5.03)]
    [InlineData(11, 2.5, 10.72)]
    public void AReductionFactorCanBeApplied(
        double starting,
        double reductionFactor,
        double expected)
    {
        var price = Price.Of(starting);

        var adjustedPrice = price.ReduceBy(reductionFactor);

        adjustedPrice.DecimalOdds.Should().Be(Math.Round(expected, 2));
        adjustedPrice.Chance.Should().Be(1 / expected);
    }

    [Theory]
    [InlineData(8.6, 2.49, 8.6)]
    [InlineData(2, 2, 2)]
    [InlineData(500, 1, 500)]
    [InlineData(5.2, 2.4, 5.2)]
    public void AReductionFactorOfLessThanTwoAndAHalfIsNotApplied(
        double starting,
        double reductionFactor,
        double expected) =>
        AReductionFactorCanBeApplied(starting, reductionFactor, expected);

    [Theory]
    [InlineData(8.6, 16.2, 195)]
    [InlineData(2, 48.1, 3)]
    [InlineData(500, 29.8, 284)]
    [InlineData(5.2, 3.2, 179)]
    [InlineData(11, 2.5, 210)]
    public void ReturnNearestTickOnReducedPrices(
        double starting,
        double reductionFactor,
        int expected)
    {
        var price = Price.Of(starting);

        var adjustedPrice = price.ReduceBy(reductionFactor);

        adjustedPrice.Tick.Should().Be(expected);
    }

    [Fact]
    public void CanBeSerialized()
    {
        var price = Price.Of(2.5);

        var json = JsonSerializer.Serialize(price);

        json.Should().Be("2.5");
    }

    [Fact]
    public void CanBeDeserialized()
    {
        var json = "2.5";

        var price = JsonSerializer.Deserialize<Price>(json);

        price!.DecimalOdds.Should().Be(2.5);
    }

    [Theory]
    [InlineData(1.01, 0.01)]
    [InlineData(1.01, 0.79)]
    [InlineData(1.01, 1.26)]
    [InlineData(1.01, 1.59)]
    [InlineData(1.25, 0.01)]
    [InlineData(1.25, 0.02)]
    [InlineData(1.25, 0.03)]
    [InlineData(1.25, 0.06)]
    public void OrdersIsNotAchievableIfItHasAnInvalidProfitRatio(double price, double size)
    {
        // See doc for INVALID_PROFIT_RATIO https://forum.developer.betfair.com/forum/developer-program/announcements/32066-retrospective-api-release-to-prevent-minimum-bet-abuse-19th-june
        // If customers attempt to place a bet (including a bet used a size-up edit or price change)
        // or cancel a bet down to a remaining size, such that it returns 20% less or 25% more than it ‘ought’ to,
        // that bet will fail with an INVALID_PROFIT_RATIO error returned to API customers.
        // For example – a 13p @ 1.06 back bet is ‘unfair’ as it ‘ought’ to win 0.78p, but actually - due to rounding - wins 1p, a 28% uplift.
        // So attempting to:
        // • Increase the size of a £2.00 bet @ 1.06 to £2.13 (which creates a new 13p bet @ 1.06)
        // • Cancel £1.83 of a £2.00 bet @ 1.06 (to leave a 13p remainder @ 1.06)
        // • Price-editing a 13p @ 1000 bet to 13p @ 1.06 (which cancels the bet @ 1000 and creates a new one @ 1.06)
        // Will all fail with the error above.
        Price.Of(price).IsSizeAchievable(size).Should().BeFalse();
    }

    [Theory]
    [InlineData(1.01, 1)]
    [InlineData(1.01, 0.8)]
    [InlineData(1.01, 1.25)]
    [InlineData(1.01, 1.60)]
    [InlineData(1.25, 0.04)]
    [InlineData(1.25, 0.05)]
    [InlineData(1.25, 0.07)]
    public void OrdersIsAchievableIfItHasAValidProfitRatio(double price, double size) =>
        Price.Of(price).IsSizeAchievable(size).Should().BeTrue();

    [Theory]
    [InlineData(4, 3.78, 1.26)]
    [InlineData(120, 99.99, 0.84)]
    [InlineData(11, 28.8, 2.88)]
    public void SizeNeedForProfitIsCalculatedCorrectly(double price, double profit, double size)
    {
        var actual = Price.Of(price).SizeNeededForProfit(profit);

        actual.Should().Be(size);
    }
}
