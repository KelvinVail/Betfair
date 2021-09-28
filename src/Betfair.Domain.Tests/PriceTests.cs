using Xunit;

namespace Betfair.Domain.Tests
{
    public class PriceTests
    {
        [Fact]
        public void Exists()
        {
            var price = Price.Of(1.01m);
        }

        [Fact]
        public void ThrowIfPriceIsNotValid()
        {
            var price = Price.Of(501);
            Assert.False(price.IsSuccess);
            Assert.Equal("'501' is an invalid price.", price.Error);
        }
    }
}
