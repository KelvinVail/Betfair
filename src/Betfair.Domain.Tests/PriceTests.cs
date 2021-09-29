using System;
using Xunit;

namespace Betfair.Domain.Tests
{
    public class PriceTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(1001)]
        [InlineData(2.01)]
        [InlineData(3.02)]
        [InlineData(4.05)]
        [InlineData(6.1)]
        [InlineData(10.3)]
        public void ThrowIfPriceIsNotValid(decimal value)
        {
            var ex = Assert.Throws<ArgumentException>(() => Price.Of(value));
            Assert.Equal(Errors.InvalidPrice(value).Message, ex.Message);
        }

        [Theory]
        [InlineData(1.01)]
        [InlineData(1000)]
        public void ChanceIsCalculatedCorrectly(decimal value)
        {
            var price = Price.Of(value);

            Assert.Equal(1 / value, price.Chance);
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
        public void AddsTicksCorrectly(decimal start, int ticks, decimal expected)
        {
            var price = Price.Of(start);

            var actual = price.AddTicks(ticks);

            Assert.Equal(expected, actual.DecimalOdds);
        }

        [Theory]
        [InlineData(1.01, 1.02, 1)]
        [InlineData(1.02, 1.03, 1)]
        [InlineData(1.9, 2.00, 10)]
        [InlineData(2.02, 2.04, 1)]
        [InlineData(3.05, 3.1, 1)]
        [InlineData(4.1, 4.2, 1)]
        [InlineData(6.2, 6.4, 1)]
        [InlineData(10.5, 11, 1)]
        [InlineData(20, 21, 1)]
        [InlineData(32, 34, 1)]
        [InlineData(55, 60, 1)]
        [InlineData(110, 120, 1)]
        [InlineData(2.0, 2.02, 1)]
        [InlineData(2.02, 2.0, -1)]
        [InlineData(1000, 990, -1)]
        public void TicksBetweenIsCalculatedCorrectly(decimal start, decimal end, int expected)
        {
            var startPrice = Price.Of(start);
            var endPrice = Price.Of(end);

            var actual = startPrice.TicksBetween(endPrice);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TicksBetweenReturnsZeroIfParameterIsNull()
        {
            var actual = Price.Of(1.01m).TicksBetween(null);

            Assert.Equal(0, actual);
        }

        // Minimum Stake
    }
}
