namespace Betfair.Betting.Tests
{
    using System;
    using System.Threading.Tasks;
    using Betfair.Betting.Tests.TestDoubles;
    using Xunit;

    public class OrdersTests
    {
        private readonly ExchangeClientSpy client = new ExchangeClientSpy();

        [Fact]
        public void MarketIdIsSetInConstructor()
        {
            Assert.Equal("MarketId", this.GetOrders("MarketId").MarketId);
        }

        [Fact]
        public void MarketIdIsSetInConstructorOverload()
        {
            var sut = this.GetOrders("MarketId", "Reference");
            Assert.Equal("MarketId", sut.MarketId);
        }

        [Fact]
        public void ThrowIfMarketIdIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => this.GetOrders(null));
            Assert.Equal("marketId", ex.ParamName);
            Assert.Equal("MarketId should not be null or empty. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public void ThrowIfMarketIdIsEmpty()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => this.GetOrders(string.Empty));
            Assert.Equal("marketId", ex.ParamName);
            Assert.Equal("MarketId should not be null or empty. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public void ThrowIfMarketIdIsNullInOverload()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => this.GetOrders(null, "Reference"));
            Assert.Equal("marketId", ex.ParamName);
            Assert.Equal("MarketId should not be null or empty. (Parameter 'marketId')", ex.Message);
        }

        [Theory]
        [InlineData("Ref")]
        [InlineData("StrategyRef")]
        [InlineData("MyOrders")]
        public void StrategyReferenceCanBeSetInConstructor(string reference)
        {
            var sut = this.GetOrders("MarketId", reference);
            Assert.Equal(reference, sut.StrategyRef);
        }

        [Theory]
        [InlineData("ThisIsLongerThanFifteenCharacters")]
        [InlineData("StrategyReference")]
        [InlineData("LotsAndLotsOfOrders")]
        public void ThrowIfStrategyReferenceIsOver15Characters(string reference)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => this.GetOrders("MarketId", reference));
            Assert.Equal("strategyRef", ex.ParamName);
            Assert.Equal($"{reference} must be less than 15 characters. (Parameter 'strategyRef')", ex.Message);
        }

        [Fact]
        public void StrategyReferenceCanBeNull()
        {
            var nullOrders = new Orders(this.client, "MarketId", null);
            Assert.Null(nullOrders.StrategyRef);
        }

        [Fact]
        public void StrategyReferenceCanBeEmpty()
        {
            var emptyOrders = new Orders(this.client, "MarketId", string.Empty);
            Assert.Null(emptyOrders.StrategyRef);
        }

        [Fact]
        public async Task PlaceThrowIfThereAreNoOrders()
        {
            var orders = this.GetOrders("MarketId");
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => orders.PlaceAsync());
            Assert.Equal("Does not contain any orders.", ex.Message);
            Assert.False(orders.Placed);
        }

        [Fact]
        public async Task PlaceSetsPlacedFlag()
        {
            var orders = this.GetOrdersWithOrders();
            Assert.False(orders.Placed);
            await orders.PlaceAsync();
            Assert.True(orders.Placed);
        }

        [Fact]
        public async Task PlaceUsesSportsEndpoint()
        {
            var orders = this.GetOrdersWithOrders();
            await orders.PlaceAsync();
            Assert.Equal("Sports", this.client.Endpoint);
        }

        [Fact]
        public async Task PlaceUsesPlaceOrdersMethod()
        {
            var orders = this.GetOrdersWithOrders();
            await orders.PlaceAsync();
            Assert.Equal("placeOrders", this.client.BetfairMethod);
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 1.01, 2.00)]
        [InlineData("1.2345678", 11111, Side.Lay, 1000, 99.71)]
        [InlineData("1.9876543", 98765, Side.Back, 3.5, 3.48)]
        public async Task LimitOrderCanBeAdded(string marketId, long selectionId, Side side, double price, double size)
        {
            var limitOrder = new LimitOrder(selectionId, side, price, size);
            var orders = this.GetOrders(marketId);
            orders.Add(limitOrder);
            await orders.PlaceAsync();
            var expected = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            Assert.Equal(expected, this.client.Parameters);
        }

        [Fact]
        public async Task MultipleLimitOrdersCanBeAdded()
        {
            var orders = this.GetOrders("MarketId");
            var limitOrderOne = new LimitOrder(12345, Side.Back, 1.01, 2.00);
            var limitOrderTwo = new LimitOrder(98765, Side.Lay, 1000, 9.99);
            orders.Add(limitOrderOne);
            orders.Add(limitOrderTwo);
            await orders.PlaceAsync();
            var expected = $"{{\"marketId\":\"MarketId\",\"instructions\":[{limitOrderOne.ToInstruction()},{limitOrderTwo.ToInstruction()}]}}";
            Assert.Equal(expected, this.client.Parameters);
        }

        [Theory]
        [InlineData("MarketId", "Reference", 12345, Side.Back, 1.01, 2.00)]
        public async Task LimitOrderWithReferenceCanBeAdded(string marketId, string reference, long selectionId, Side side, double price, double size)
        {
            var limitOrder = new LimitOrder(selectionId, side, price, size);
            var orders = this.GetOrders(marketId, reference);
            orders.Add(limitOrder);
            await orders.PlaceAsync();
            var expected = $"{{\"marketId\":\"{marketId}\",\"customerStrategyRef\":\"{reference}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            Assert.Equal(expected, this.client.Parameters);
        }

        private Orders GetOrders(string marketId, string reference = null)
        {
            return reference is null ? new Orders(this.client, marketId) : new Orders(this.client, marketId, reference);
        }

        private Orders GetOrdersWithOrders()
        {
            var orders = this.GetOrders("MarketId");
            var limitOrder = new LimitOrder(12345, Side.Back, 1.01, 2.00);
            orders.Add(limitOrder);
            return orders;
        }
    }
}
