namespace Betfair.Betting.Tests
{
    using System;
    using System.Threading.Tasks;
    using Betfair.Betting.Tests.TestDoubles;
    using Xunit;

    public class OrdersTests
    {
        private readonly ExchangeServiceSpy service = new ExchangeServiceSpy();

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
        public void ThrowIfExchangeServiceIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new Orders(null, "MarketId"));
            Assert.Equal("service", ex.ParamName);
            Assert.Equal("ExchangeService should not be null. (Parameter 'service')", ex.Message);
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
            var nullOrders = new Orders(this.service, "MarketId");
            Assert.Null(nullOrders.StrategyRef);
        }

        [Fact]
        public void StrategyReferenceCanBeEmpty()
        {
            var emptyOrders = new Orders(this.service, "MarketId", string.Empty);
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
        public async Task PlaceUsesBettingEndpoint()
        {
            var orders = this.GetOrdersWithOrders();
            await orders.PlaceAsync();
            Assert.Equal("betting", this.service.Endpoint);
        }

        [Fact]
        public async Task PlaceUsesPlaceOrdersMethod()
        {
            var orders = this.GetOrdersWithOrders();
            await orders.PlaceAsync();
            Assert.Equal("placeOrders", this.service.BetfairMethod);
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 1.01, 2.00)]
        [InlineData("1.2345678", 11111, Side.Lay, 1000, 99.71)]
        [InlineData("1.9876543", 98765, Side.Back, 3.5, 3.48)]
        public async Task LimitOrderIsPlaced(string marketId, long selectionId, Side side, double price, double size)
        {
            var limitOrder = new LimitOrder(selectionId, side, size, price);
            var orders = this.GetOrders(marketId);
            orders.Add(limitOrder);
            await orders.PlaceAsync();
            var expected = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            Assert.Equal(expected, this.service.SentParameters["placeOrders"]);
        }

        [Fact]
        public async Task MultipleLimitOrdersArePlaced()
        {
            var orders = this.GetOrders("MarketId");
            var limitOrderOne = new LimitOrder(12345, Side.Back, 2.00, 1.01);
            var limitOrderTwo = new LimitOrder(98765, Side.Lay, 9.99, 1000);
            orders.Add(limitOrderOne);
            orders.Add(limitOrderTwo);
            await orders.PlaceAsync();
            var expected = $"{{\"marketId\":\"MarketId\",\"instructions\":[{limitOrderOne.ToInstruction()},{limitOrderTwo.ToInstruction()}]}}";
            Assert.Equal(expected, this.service.SentParameters["placeOrders"]);
        }

        [Theory]
        [InlineData("MarketId", "Reference", 12345, Side.Back, 1.01, 2.00)]
        [InlineData("MarketId", "ABC", 12345, Side.Back, 1.01, 2.00)]
        [InlineData("MarketId", "123", 12345, Side.Back, 1.01, 2.00)]
        public async Task LimitOrderWithReferenceIsPlaced(string marketId, string reference, long selectionId, Side side, double price, double size)
        {
            var limitOrder = new LimitOrder(selectionId, side, size, price);
            var orders = this.GetOrders(marketId, reference);
            orders.Add(limitOrder);
            await orders.PlaceAsync();
            var expected = $"{{\"marketId\":\"{marketId}\",\"customerStrategyRef\":\"{reference}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            Assert.Equal(expected, this.service.SentParameters["placeOrders"]);
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 1.99, 1.01)]
        [InlineData("1.2345678", 11111, Side.Lay, 1, 2)]
        [InlineData("1.9876543", 98765, Side.Back, 0.10, 3.5)]
        public async Task BelowMinimumLimitOrdersArePlaced(string marketId, long selectionId, Side side, double size, double price)
        {
            var limitOrder = new LimitOrder(selectionId, side, size, price);
            var orders = this.GetOrders(marketId);
            orders.Add(limitOrder);
            await orders.PlaceAsync();
            var placeInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            var cancelInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToBelowMinimumCancelInstruction()}]}}";
            var replaceInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToBelowMinimumReplaceInstruction()}]}}";
            Assert.Equal(placeInstruction, this.service.SentParameters["placeOrders"]);
            Assert.Equal(cancelInstruction, this.service.SentParameters["cancelOrders"]);
            Assert.Equal(replaceInstruction, this.service.SentParameters["replaceOrders"]);
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 2, 1.01)]
        [InlineData("1.2345678", 11111, Side.Lay, 2, 2)]
        [InlineData("1.9876543", 98765, Side.Back, 2, 3.5)]
        public async Task BelowMinimumInstructionsNotCalledIfOrderIsAboveMinimum(string marketId, long selectionId, Side side, double size, double price)
        {
            var limitOrder = new LimitOrder(selectionId, side, size, price);
            var orders = this.GetOrders(marketId);
            orders.Add(limitOrder);
            await orders.PlaceAsync();
            Assert.False(this.service.SentParameters.ContainsKey("cancelOrders"));
            Assert.False(this.service.SentParameters.ContainsKey("replaceOrders"));
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 1.99, 1.01, "Ref")]
        [InlineData("1.2345678", 11111, Side.Lay, 1, 2, "MyRef")]
        [InlineData("1.9876543", 98765, Side.Back, 0.10, 3.5, "StrategyA")]
        public async Task BelowMinimumLimitOrdersArePlacedWithReference(string marketId, long selectionId, Side side, double size, double price, string reference)
        {
            var limitOrder = new LimitOrder(selectionId, side, size, price);
            var orders = this.GetOrders(marketId, reference);
            orders.Add(limitOrder);
            await orders.PlaceAsync();
            var placeInstruction = $"{{\"marketId\":\"{marketId}\",\"customerStrategyRef\":\"{reference}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            var cancelInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToBelowMinimumCancelInstruction()}]}}";
            var replaceInstruction = $"{{\"marketId\":\"{marketId}\",\"customerStrategyRef\":\"{reference}\",\"instructions\":[{limitOrder.ToBelowMinimumReplaceInstruction()}]}}";
            Assert.Equal(placeInstruction, this.service.SentParameters["placeOrders"]);
            Assert.Equal(cancelInstruction, this.service.SentParameters["cancelOrders"]);
            Assert.Equal(replaceInstruction, this.service.SentParameters["replaceOrders"]);
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 1.99, 1.01)]
        [InlineData("1.2345678", 11111, Side.Lay, 1, 2)]
        [InlineData("1.9876543", 98765, Side.Back, 0.10, 3.5)]
        public async Task OnlyBelowMinimumLimitOrdersAreReplacedWhenMixedWithAboveMinimumOrders(string marketId, long selectionId, Side side, double size, double price)
        {
            var limitOrder = new LimitOrder(selectionId, side, size, price);
            var orders = this.GetOrders(marketId);
            orders.Add(limitOrder);
            orders.Add(new LimitOrder(1, Side.Back, 10, 2));
            await orders.PlaceAsync();
            var cancelInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToBelowMinimumCancelInstruction()}]}}";
            var replaceInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToBelowMinimumReplaceInstruction()}]}}";
            Assert.Equal(cancelInstruction, this.service.SentParameters["cancelOrders"]);
            Assert.Equal(replaceInstruction, this.service.SentParameters["replaceOrders"]);
        }

        private Orders GetOrders(string marketId, string reference = null)
        {
            return reference is null ? new Orders(this.service, marketId) : new Orders(this.service, marketId, reference);
        }

        private Orders GetOrdersWithOrders()
        {
            var orders = this.GetOrders("MarketId");
            var limitOrder = new LimitOrder(12345, Side.Back, 2.00, 1.01);
            orders.Add(limitOrder);
            return orders;
        }
    }
}
