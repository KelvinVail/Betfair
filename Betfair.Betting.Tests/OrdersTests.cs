namespace Betfair.Betting.Tests
{
    using System;
    using Xunit;

    public class OrdersTests
    {
        private readonly Orders orders = new Orders("MarketId");

        [Fact]
        public void MarketIdIsSetInConstructor()
        {
            Assert.Equal("MarketId", this.orders.MarketId);
        }

        [Fact]
        public void MarketIdIsSetInConstructorOverload()
        {
            var sut = new Orders("MarketId", "Reference");
            Assert.Equal("MarketId", sut.MarketId);
        }

        [Fact]
        public void ThrowIfMarketIdIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new Orders(null));
            Assert.Equal("marketId", ex.ParamName);
            Assert.Equal("MarketId should not be null or empty. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public void ThrowIfMarketIdIsNullInOverload()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new Orders(null, "Reference"));
            Assert.Equal("marketId", ex.ParamName);
            Assert.Equal("MarketId should not be null or empty. (Parameter 'marketId')", ex.Message);
        }

        [Theory]
        [InlineData("Ref")]
        [InlineData("StrategyRef")]
        [InlineData("MyOrders")]
        public void StrategyReferenceCanBeSetInConstructor(string reference)
        {
            var sut = new Orders("MarketId", reference);
            Assert.Equal(reference, sut.StrategyRef);
        }

        [Theory]
        [InlineData("ThisIsLongerThanFifteenCharacters")]
        [InlineData("StrategyReference")]
        [InlineData("LotsAndLotsOfOrders")]
        public void ThrowIfStrategyReferenceIsOver15Characters(string reference)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Orders("MarketId", reference));
            Assert.Equal("strategyRef", ex.ParamName);
            Assert.Equal($"{reference} must be less than 15 characters. (Parameter 'strategyRef')", ex.Message);
        }

        [Fact]
        public void StrategyReferenceCanBeNull()
        {
            var nullOrders = new Orders("MarketId", null);
            Assert.Null(nullOrders.StrategyRef);
        }

        [Fact]
        public void StrategyReferenceCanBeEmpty()
        {
            var emptyOrders = new Orders("MarketId", string.Empty);
            Assert.Null(emptyOrders.StrategyRef);
        }
    }
}
