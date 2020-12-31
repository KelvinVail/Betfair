using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betfair.Betting;
using Betfair.Extensions.Tests.TestDoubles;
using Xunit;

namespace Betfair.Extensions.Tests
{
    public class OrderManagerBaseTests
    {
        private readonly OrderServiceSpy _orderService = new OrderServiceSpy();
        private readonly OrderManagerSpy _orderManager;

        public OrderManagerBaseTests()
        {
            _orderManager = new OrderManagerSpy(_orderService);
            _orderManager.LinkToMarket(new MarketCache("1.2345"));
        }

        [Fact]
        public void CanBeLinkedToAMarket()
        {
            Assert.Equal("1.2345", _orderManager.MarketCache.MarketId);
        }

        [Fact]
        public async Task PlaceCallsOrderService()
        {
            var order = new LimitOrder(1, Side.Back, 2.5, 9.99);
            await _orderManager.Place(new List<LimitOrder> { order });
            Assert.Contains(order, _orderService.LastOrdersPlaced);
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("9.8765")]
        public async Task PlaceOrdersUsesMarketIdFromCache(string marketId)
        {
            _orderManager.LinkToMarket(new MarketCache(marketId));
            var order = new LimitOrder(1, Side.Back, 2.5, 9.99);
            await _orderManager.Place(new List<LimitOrder> { order });
            Assert.Equal(marketId, _orderService.MarketId);
        }

        [Theory]
        [InlineData("Ref")]
        [InlineData("OtherRef")]
        public async Task StrategyRefCanBeSet(string strategyRef)
        {
            _orderManager.LinkToMarket(new MarketCache("1.2345"));
            var order = new LimitOrder(1, Side.Back, 2.5, 9.99);
            await _orderManager.Place(new List<LimitOrder> { order }, strategyRef);
            Assert.Equal(strategyRef, _orderService.StrategyRef);
        }

        [Fact]
        public void OrderServiceIsProtected()
        {
            Assert.True(_orderManager.CanAccessOrderService());
        }

        [Fact]
        public async Task DoNotPlaceOrdersIfOrderListIsEmpty()
        {
            await _orderManager.Place(new List<LimitOrder>());
            Assert.DoesNotContain("P", _orderService.Actions, StringComparison.CurrentCulture);
        }

        [Fact]
        public async Task DoNotPlaceOrdersIfOrderListIsNull()
        {
            await _orderManager.Place(null);
            Assert.DoesNotContain("P", _orderService.Actions, StringComparison.CurrentCulture);
        }
    }
}
