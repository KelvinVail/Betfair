namespace Betfair.Extensions.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Betfair.Betting;
    using Betfair.Extensions.Tests.TestDoubles;
    using Xunit;

    public class OrderManagerBaseTests
    {
        private readonly OrderServiceSpy orderService = new OrderServiceSpy();

        private readonly OrderManagerSpy orderManager;

        public OrderManagerBaseTests()
        {
            this.orderManager = new OrderManagerSpy(this.orderService);
            this.orderManager.LinkToMarket(new MarketCache("1.2345"));
        }

        [Fact]
        public void CanBeLinkedToAMarket()
        {
            Assert.Equal("1.2345", this.orderManager.MarketCache.MarketId);
        }

        [Fact]
        public async Task PlaceCallsOrderService()
        {
            var order = new LimitOrder(1, Side.Back, 2.5, 9.99);
            await this.orderManager.Place(new List<LimitOrder> { order });
            Assert.Contains(order, this.orderService.LastOrdersPlaced);
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("9.8765")]
        public async Task PlaceOrdersUsesMarketIdFromCache(string marketId)
        {
            this.orderManager.LinkToMarket(new MarketCache(marketId));
            var order = new LimitOrder(1, Side.Back, 2.5, 9.99);
            await this.orderManager.Place(new List<LimitOrder> { order });
            Assert.Equal(marketId, this.orderService.MarketId);
        }

        [Theory]
        [InlineData("Ref")]
        [InlineData("OtherRef")]
        public async Task StrategyRefCanBeSet(string strategyRef)
        {
            this.orderManager.LinkToMarket(new MarketCache("1.2345"));
            var order = new LimitOrder(1, Side.Back, 2.5, 9.99);
            await this.orderManager.Place(new List<LimitOrder> { order }, strategyRef);
            Assert.Equal(strategyRef, this.orderService.StrategyRef);
        }

        [Fact]
        public void OrderServiceIsProtected()
        {
            Assert.True(this.orderManager.CanAccessOrderService());
        }
    }
}
