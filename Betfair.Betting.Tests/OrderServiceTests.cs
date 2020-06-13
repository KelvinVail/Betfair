namespace Betfair.Betting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Betfair.Betting.Tests.TestDoubles;
    using Xunit;

    public class OrderServiceTests
    {
        private readonly ExchangeServiceSpy exchange = new ExchangeServiceSpy();

        private readonly OrderService orderService;

        private readonly List<LimitOrder> testOrders = new List<LimitOrder>
        {
            new LimitOrder(1, Side.Back, 2.5, 10.99),
        };

        public OrderServiceTests()
        {
            this.orderService = new OrderService(this.exchange);
        }

        [Fact]
        public void InheritsIOrderService()
        {
            Assert.True(typeof(IOrderService).IsAssignableFrom(typeof(OrderService)));
        }

        [Fact]
        public void ThrowIfExchangeServiceIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OrderService(null));
            Assert.Equal("exchange", ex.ParamName);
            Assert.Equal("ExchangeService should not be null. (Parameter 'exchange')", ex.Message);
        }

        [Fact]
        public async Task ThrowIfMarketIdIsNull()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this.orderService.Place(null, this.testOrders));
            Assert.Equal("marketId", ex.ParamName);
            Assert.Equal("MarketId should not be null or empty. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public async Task ThrowIfMarketIdIsEmpty()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this.orderService.Place(string.Empty, this.testOrders));
            Assert.Equal("marketId", ex.ParamName);
            Assert.Equal("MarketId should not be null or empty. (Parameter 'marketId')", ex.Message);
        }

        [Theory]
        [InlineData("ThisIsLongerThanFifteenCharacters")]
        [InlineData("StrategyReference")]
        [InlineData("LotsAndLotsOfOrders")]
        public async Task ThrowIfStrategyReferenceIsOver15Characters(string reference)
        {
            var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                this.orderService.Place("MarketId", this.testOrders, reference));
            Assert.Equal("strategyRef", ex.ParamName);
            Assert.Equal($"{reference} must be less than 15 characters. (Parameter 'strategyRef')", ex.Message);
        }

        [Fact]
        public async Task PlaceThrowIfThereAreNoOrders()
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                this.orderService.Place("1.2345", new List<LimitOrder>()));
            Assert.Equal("Does not contain any orders.", ex.Message);
        }

        [Fact]
        public async Task PlaceUsesBettingEndpoint()
        {
            await this.orderService.Place("1.2345", this.testOrders);
            Assert.Equal("betting", this.exchange.Endpoint);
        }

        [Fact]
        public async Task PlaceUsesPlaceOrdersMethod()
        {
            await this.orderService.Place("1.2345", this.testOrders);
            Assert.Equal("placeOrders", this.exchange.BetfairMethod);
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 1.01, 2.00)]
        [InlineData("1.2345678", 11111, Side.Lay, 1000, 99.71)]
        [InlineData("1.9876543", 98765, Side.Back, 3.5, 3.48)]
        public async Task LimitOrderIsPlaced(string marketId, long selectionId, Side side, double price, double size)
        {
            var limitOrder = new LimitOrder(selectionId, side, price, size);
            await this.orderService.Place(marketId, new List<LimitOrder> { limitOrder });
            var expected = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            Assert.Equal(expected, this.exchange.SentParameters["placeOrders"]);
        }

        [Fact]
        public async Task MultipleLimitOrdersArePlaced()
        {
            var order1 = new LimitOrder(12345, Side.Back, 1.01, 2.00);
            var order2 = new LimitOrder(98765, Side.Lay, 1000, 9.99);
            var limitOrders = new List<LimitOrder> { order1, order2 };
            await this.orderService.Place("MarketId", limitOrders);
            var expected = $"{{\"marketId\":\"MarketId\",\"instructions\":[{order1.ToInstruction()},{order2.ToInstruction()}]}}";
            Assert.Equal(expected, this.exchange.SentParameters["placeOrders"]);
        }

        [Theory]
        [InlineData("MarketId", "Reference", 12345, Side.Back, 1.01, 2.00)]
        [InlineData("MarketId", "ABC", 12345, Side.Back, 1.01, 2.00)]
        [InlineData("MarketId", "123", 12345, Side.Back, 1.01, 2.00)]
        public async Task LimitOrderWithReferenceIsPlaced(string marketId, string reference, long selectionId, Side side, double price, double size)
        {
            var limitOrders = new List<LimitOrder>
            {
                new LimitOrder(selectionId, side, price, size),
            };
            await this.orderService.Place(marketId, limitOrders, reference);
            var expected = $"{{\"marketId\":\"{marketId}\",\"customerStrategyRef\":\"{reference}\",\"instructions\":[{limitOrders.First().ToInstruction()}]}}";
            Assert.Equal(expected, this.exchange.SentParameters["placeOrders"]);
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 1.99, 1.01)]
        [InlineData("1.2345678", 11111, Side.Lay, 1, 2)]
        [InlineData("1.9876543", 98765, Side.Back, 0.10, 3.5)]
        public async Task BelowMinimumLimitOrdersArePlaced(string marketId, long selectionId, Side side, double size, double price)
        {
            var limitOrder = new LimitOrder(selectionId, side, price, size);
            await this.orderService.Place(marketId, new List<LimitOrder> { limitOrder });
            var placeInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            var cancelInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToBelowMinimumCancelInstruction()}]}}";
            var replaceInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToBelowMinimumReplaceInstruction()}]}}";
            Assert.Equal(placeInstruction, this.exchange.SentParameters["placeOrders"]);
            Assert.Equal(cancelInstruction, this.exchange.SentParameters["cancelOrders"]);
            Assert.Equal(replaceInstruction, this.exchange.SentParameters["replaceOrders"]);
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 2, 1.01)]
        [InlineData("1.2345678", 11111, Side.Lay, 2, 2)]
        [InlineData("1.9876543", 98765, Side.Back, 2, 3.5)]
        public async Task BelowMinimumInstructionsNotCalledIfOrderIsAboveMinimum(string marketId, long selectionId, Side side, double size, double price)
        {
            var limitOrder = new LimitOrder(selectionId, side, price, size);
            await this.orderService.Place(marketId, new List<LimitOrder> { limitOrder });
            Assert.False(this.exchange.SentParameters.ContainsKey("cancelOrders"));
            Assert.False(this.exchange.SentParameters.ContainsKey("replaceOrders"));
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 1.99, 1.01, "Ref")]
        [InlineData("1.2345678", 11111, Side.Lay, 1, 2, "MyRef")]
        [InlineData("1.9876543", 98765, Side.Back, 0.10, 3.5, "StrategyA")]
        public async Task BelowMinimumLimitOrdersArePlacedWithReference(string marketId, long selectionId, Side side, double size, double price, string reference)
        {
            var limitOrder = new LimitOrder(selectionId, side, price, size);
            await this.orderService.Place(marketId, new List<LimitOrder> { limitOrder }, reference);
            var placeInstruction = $"{{\"marketId\":\"{marketId}\",\"customerStrategyRef\":\"{reference}\",\"instructions\":[{limitOrder.ToInstruction()}]}}";
            var cancelInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToBelowMinimumCancelInstruction()}]}}";
            var replaceInstruction = $"{{\"marketId\":\"{marketId}\",\"customerStrategyRef\":\"{reference}\",\"instructions\":[{limitOrder.ToBelowMinimumReplaceInstruction()}]}}";
            Assert.Equal(placeInstruction, this.exchange.SentParameters["placeOrders"]);
            Assert.Equal(cancelInstruction, this.exchange.SentParameters["cancelOrders"]);
            Assert.Equal(replaceInstruction, this.exchange.SentParameters["replaceOrders"]);
        }

        [Theory]
        [InlineData("MarketId", 12345, Side.Back, 1.99, 1.01)]
        [InlineData("1.2345678", 11111, Side.Lay, 1, 2)]
        [InlineData("1.9876543", 98765, Side.Back, 0.10, 3.5)]
        public async Task OnlyBelowMinimumLimitOrdersAreReplacedWhenMixedWithAboveMinimumOrders(string marketId, long selectionId, Side side, double size, double price)
        {
            var limitOrder = new LimitOrder(selectionId, side, price, size);
            var aboveMinOrder = new LimitOrder(1, Side.Back, 2, 10);
            await this.orderService.Place(marketId, new List<LimitOrder> { limitOrder, aboveMinOrder });
            var cancelInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToBelowMinimumCancelInstruction()}]}}";
            var replaceInstruction = $"{{\"marketId\":\"{marketId}\",\"instructions\":[{limitOrder.ToBelowMinimumReplaceInstruction()}]}}";
            Assert.Equal(cancelInstruction, this.exchange.SentParameters["cancelOrders"]);
            Assert.Equal(replaceInstruction, this.exchange.SentParameters["replaceOrders"]);
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("9.8765")]
        [InlineData("MarketId")]
        public async Task AllOrdersOnAMarketCanBeCancelled(string marketId)
        {
            this.exchange.WithReturnContent("cancelOrders", "{\"Test\":\"Test\"}");
            var expected = $"{{\"marketId\":\"{marketId}\"}}";
            await this.orderService.CancelAll(marketId);
            Assert.Equal(expected, this.exchange.SentParameters["cancelOrders"]);
            Assert.Equal("betting", this.exchange.Endpoint);
            Assert.Equal("cancelOrders", this.exchange.BetfairMethod);
        }
    }
}
