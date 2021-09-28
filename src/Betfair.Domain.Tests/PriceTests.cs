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
            var ex = Assert.Throws<InvalidOperationException>(() => Price.Of(value));
            Assert.Equal(Errors.InvalidPrice(value).Message, ex.Message);
        }

        [Theory]
        [InlineData(1.01)]
        [InlineData(1000)]
        public void ImpliedOddsIsCalculatedCorrectly(decimal value)
        {
            var price = Price.Of(value);

            Assert.Equal(1 / value, price.ImpliedOdds);
        }

        // AddTick
        // TicksBetween
    }
}
