namespace Betfair.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Betfair.Entities;
    using Betfair.Services;
    using Betfair.Services.BetfairApi;
    using Betfair.Services.BetfairApi.Enums;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Response;
    using Betfair.Tests.Mocks;

    using Moq;
    using Moq.Protected;

    using Newtonsoft.Json;

    using Xunit;

    /// <summary>
    /// The order book should...
    /// </summary>
    public class OrderBookShould
    {
        /// <summary>
        /// The betfair client fake.
        /// </summary>
        private readonly BetfairClientFake betfairClientFake;

        /// <summary>
        /// The betfair API fake.
        /// </summary>
        private readonly BetfairApiFake betfairApiFake = new BetfairApiFake();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBookShould"/> class.
        /// </summary>
        public OrderBookShould()
        {
            var orderService = new OrderService(this.betfairApiFake);
            this.betfairClientFake = new BetfairClientFake(orderService);
        }

        /// <summary>
        /// ... execute a single back order.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ExecuteASingleBackOrder()
        {
            // Arrange
            const string MarketId = "fakeMarketId";
            var order = new Order(12345, Side.BACK, 1.01, 2);
            var sut = new OrderBook(this.betfairClientFake, MarketId);
            sut.AddOrder(order);

            // Act
            await sut.ExecuteAsync();

            // Assert
            var placed = sut.Orders.Where(w => w.SelectionId == 12345).Select(s => s.Placed).FirstOrDefault();
            Assert.True(placed);
        }

        /// <summary>
        /// ... execute a single lay order.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ExecuteASingleLayOrder()
        {
            // Arrange
            var sut = new OrderBook(this.betfairClientFake, "fakeMarketId");
            var order = new Order(12345, Side.LAY, 1.01, 2);
            sut.AddOrder(order);

            // Act
            await sut.ExecuteAsync();

            // Assert
            var placed = sut.Orders.Where(w => w.SelectionId == 12345).Select(s => s.Placed).FirstOrDefault();
            Assert.True(placed);
        }

        /// <summary>
        /// ... not except an order if the price is invalid.
        /// </summary>
        [Fact]
        public void NotExceptAnOrderIfThePriceIsInvalid()
        {
            // Arrange
            var sut = new OrderBook(this.betfairClientFake, "fakeMarketId");
            var order = new Order(12345, Side.LAY, 1001, 2);

            // Act
            var actual = sut.AddOrder(order);

            // Assert
            Assert.False(actual.IsSuccess);
        }

        /// <summary>
        /// ... not execute if there are no orders.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task NotExecuteIfThereAreNoOrders()
        {
            // Arrange
            var sut = new OrderBook(this.betfairClientFake, "fakeMarketId");

            // Act
            await sut.ExecuteAsync();

            // Assert
            var actual = this.betfairApiFake.PlaceOrderCount;
            Assert.Equal(0, actual);
        }

        /// <summary>
        /// ... not execute if customer ref is invalid.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task NotExecuteIfCustomerRefIsInvalid()
        {
            // Arrange
            var sut = new OrderBook(this.betfairClientFake, "fakeMarketId", "0123456789012345");
            var order = new Order(12345, Side.LAY, 1.01, 2);
            sut.AddOrder(order);

            // Act
            await sut.ExecuteAsync();

            // Assert
            var actual = this.betfairApiFake.PlaceOrderCount;
            Assert.Equal(0, actual);
        }
    }
}
