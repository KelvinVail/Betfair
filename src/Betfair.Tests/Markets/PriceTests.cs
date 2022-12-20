using Betfair.Markets;

namespace Betfair.Tests.Markets;

public class PriceTests
{
    [Theory]
    [InlineData(1.01)]
    [InlineData(3.25)]
    [InlineData(11)]
    public void CanBeEqual(decimal value)
    {
        var price1 = Price.Of(value);
        var price2 = Price.Of(value);

        price1.Should().Be(price2);
        price1.CompareTo(price2).Should().Be(0);
    }

    [Theory]
    [InlineData(2, 1.01)]
    [InlineData(9.8, 9.6)]
    public void CanBeComparedUsingGreaterAndLessThan(decimal value1, decimal value2)
    {
        var price1 = Price.Of(value1);
        var price2 = Price.Of(value2);

        price1.Should().BeGreaterThan(price2);
        price1.CompareTo(price2).Should().Be(1);
        price2.Should().BeLessThan(price1);
        price2.CompareTo(price1).Should().Be(-1);
    }

    [Theory]
    [InlineData(2, 1.01)]
    [InlineData(9.8, 9.6)]
    [InlineData(10, 10)]
    public void CanBeComparedUsingGreaterOrEqualAndLessThanOrEqual(decimal value1, decimal value2)
    {
        var price1 = Price.Of(value1);
        var price2 = Price.Of(value2);

        price1.Should().BeGreaterOrEqualTo(price2);
        price2.Should().BeLessThanOrEqualTo(price1);
    }

    [Fact]
    public void StringIsFormattedWithCommaSeparator()
    {
        var size = Price.Of(1.01m);

        size.ToString().Should().Be("1.01");
    }

    [Fact]
    public void InvalidPriceIsReturnedAs1000()
    {
        var price = Price.Of(9.9m);

        price.DecimalOdds.Should().Be(1000m);
    }

    [Theory]
    [InlineData(1.01, 2)]
    [InlineData(5, 2)]
    [InlineData(7, 1.43)]
    [InlineData(10, 1)]
    [InlineData(100, 0.10)]
    [InlineData(1000, 0.01)]
    public void MinimumStakeIsCalculated(decimal value, decimal size)
    {
        var price = Price.Of(value);

        price.MinimumStake.Should().Be(Size.Of(size));
    }

    [Theory]
    [InlineData(1.01, 0.99)]
    [InlineData(5, 0.2)]
    [InlineData(7, 0.142)]
    [InlineData(10, 0.1)]
    [InlineData(100, 0.01)]
    [InlineData(1000, 0.001)]
    public void ChanceIsCalculated(decimal value, decimal chance)
    {
        var price = Price.Of(value);

        price.Chance.Should().BeApproximately(chance, 0.001m);
    }

    [Theory]
    [InlineData(1.01, 1, 1.02)]
    [InlineData(3.05, 12, 3.65)]
    public void TicksCanBeAdded(decimal start, int ticks, decimal target)
    {
        var price = Price.Of(start);

        var result = price.AddTicks(ticks);

        result.DecimalOdds.Should().Be(target);
    }

    [Theory]
    [InlineData(1.02, -1, 1.01)]
    [InlineData(110, -20, 32)]
    public void TicksCanBeSubtracted(decimal start, int ticks, decimal target) =>
        TicksCanBeAdded(start, ticks, target);

    [Theory]
    [InlineData(1.01, -1, 1.01)]
    [InlineData(1000, 1, 1000)]
    public void TicksStayInBounds(decimal start, int ticks, decimal target) =>
        TicksCanBeAdded(start, ticks, target);

    [Theory]
    [InlineData(1.01, 1.02, 1)]
    [InlineData(3.05, 3.65, 12)]
    [InlineData(32, 110, 20)]
    [InlineData(1.02, 1.01, -1)]
    [InlineData(3.65, 3.05, -12)]
    [InlineData(110, 32, -20)]
    public void TicksBetweenTwoPricesCanBeCalculated(decimal start, decimal end, int diff)
    {
        var price1 = Price.Of(start);
        var price2 = Price.Of(end);

        var result = price1.TicksBetween(price2);

        result.Should().Be(diff);
    }
}
