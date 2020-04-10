namespace Betfair.Betting.Tests
{
    using System;
    using Xunit;

    public class OrdersTests
    {
        [Fact]
        public void MarketIdIsSetInConstructor()
        {
            Assert.Equal("MarketId", new Orders("MarketId").MarketId);
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
        public void ThrowIfMarketIdIsEmpty()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new Orders(string.Empty));
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

        [Fact]
        public void ToParamsReturnJsonInCorrectStructure()
        {
            var expected = "{\"marketId\":\"MarketId\",\"instructions\":[]}";
            Assert.Equal(expected, new Orders("MarketId").ToParams());
        }

        [Theory]
        [InlineData("MarketId")]
        [InlineData("1.2345678")]
        [InlineData("1.9876543")]
        public void ToParamsReturnsMarketId(string marketId)
        {
            var orders = new Orders(marketId);
            var expected = $"{{\"marketId\":\"{marketId}\",\"instructions\":[]}}";
            Assert.Equal(expected, orders.ToParams());
        }

        [Theory]
        [InlineData("MarketId", "Reference")]
        [InlineData("1.2345678", "StrategyRef")]
        [InlineData("1.9876543", "MyOrders")]
        public void ToParamsReturnsStrategyRef(string marketId, string reference)
        {
            var orders = new Orders(marketId, reference);
            var expected = $"{{\"marketId\":\"{marketId}\",\"customerStrategyRef\":\"{reference}\",\"instructions\":[]}}";
            Assert.Equal(expected, orders.ToParams());
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 1.01, 2.00)]
        [InlineData("1.2345678", 11111, Side.Lay, 1000, 99.71)]
        [InlineData("1.9876543", 98765, Side.Back, 3.5, 3.48)]
        public void LimitOrderCanBeAdded(string marketId, long selectionId, Side side, double price, double size)
        {
            var limitOrder = new LimitOrder(selectionId, side, price, size);
            var orders = new Orders(marketId);
            orders.Add(limitOrder);
            var expected = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            Assert.Equal(expected, orders.ToParams());
        }

        [Fact]
        public void MultipleLimitOrdersCanBeAdded()
        {
            var orders = new Orders("MarketId");
            var limitOrderOne = new LimitOrder(12345, Side.Back, 1.01, 2.00);
            var limitOrderTwo = new LimitOrder(98765, Side.Lay, 1000, 9.99);
            orders.Add(limitOrderOne);
            orders.Add(limitOrderTwo);
            var expected = $"{{\"marketId\":\"MarketId\",\"instructions\":[{limitOrderOne.ToInstruction()},{limitOrderTwo.ToInstruction()}]}}";
            Assert.Equal(expected, orders.ToParams());
        }

        [Theory]
        [InlineData("MarketId", "Reference", 12345, Side.Back, 1.01, 2.00)]
        public void LimitOrderWithReferenceCanBeAdded(string marketId, string reference, long selectionId, Side side, double price, double size)
        {
            var limitOrder = new LimitOrder(selectionId, side, price, size);
            var orders = new Orders(marketId, reference);
            orders.Add(limitOrder);
            var expected = $"{{\"marketId\":\"{marketId}\",\"customerStrategyRef\":\"{reference}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            Assert.Equal(expected, orders.ToParams());
        }
    }
}
