namespace Betfair.Tests
{
    using System.Threading.Tasks;

    using Betfair.Entities;
    using Betfair.Services;
    using Betfair.Services.BetfairApi.Enums;
    using Betfair.Tests.Mocks;

    using Xunit;

    /// <summary>
    /// The order service should...
    /// </summary>
    public class OrderServiceShould
    {
        /// <summary>
        /// ...place an order.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task PlaceAnOrder()
        {
            // Arrange
            var marketOrder = new MarketOrders<Order>("fakeMarket");
            var order = new Order(1234, Side.BACK, 2, 2);
            marketOrder.AddOrder(order);

            // Act
            var betfairApiFake = new BetfairApiFake();
            var orderService = new OrderService(betfairApiFake);
            await orderService.PlaceOrdersAsync<MarketOrders<Order>, Order>(marketOrder);

            // Assert
            Assert.Equal(1, betfairApiFake.PlaceOrderCount);
        }
    }
}
