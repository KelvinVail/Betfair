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
        /// The http message handler mock.
        /// </summary>
        private readonly Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        /// <summary>
        /// The betfair client fake.
        /// </summary>
        private readonly BetfairClientFake betfairClientFake;

        /// <summary>
        /// The order service fake.
        /// </summary>
        private readonly OrderServiceFake orderServiceFake = new OrderServiceFake();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBookShould"/> class.
        /// </summary>
        public OrderBookShould()
        {
            this.orderServiceFake = new OrderServiceFake();
            this.betfairClientFake = new BetfairClientFake(this.orderServiceFake);
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
            var orders = new List<Order> { order };
            var response = this.FullMatchedOrderResponse(orders, MarketId);

            this.httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()).ReturnsAsync(
                    new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = response });

            var httpClient = new HttpClient(this.httpMessageHandlerMock.Object);

            // Act
            var betfairApiClientMock = new BetfairApiClient("fakeKey", "fakeSession", httpClient);
            var orderService = new OrderService(betfairApiClientMock);
            var betfairClientMock = new BetfairClientFake(orderService);
            var sut = new OrderBook(betfairClientMock, MarketId);
            sut.AddOrder(order);
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
        [Fact(Skip = "To be implemented")]
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
            Assert.False(this.orderServiceFake.PlaceOrdersAsyncExecuted);
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
            Assert.False(this.orderServiceFake.PlaceOrdersAsyncExecuted);
        }

        /// <summary>
        /// The full matched order response.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <param name="marketId">
        /// The market Id.
        /// </param>
        /// <returns>
        /// The <see cref="PlaceOrdersResponse"/>.
        /// </returns>
        private StringContent FullMatchedOrderResponse(List<Order> orders, string marketId)
        {
            var betCount = 0;
            var instructionReports = orders.Where(w => !w.Placed)
                .Select(
                    order => new PlaceInstructionReport
                                 {
                                     AveragePriceMatched = order.Price,
                                     BetId = (betCount++).ToString(),
                                     ErrorCode = InstructionReportErrorCode.SUCCESS,
                                     PlacedDate = DateTime.UtcNow,
                                     Instruction = order.PlaceInstruction(),
                                     OrderStatus = OrderStatus.EXECUTION_COMPLETE,
                                     SizeMatched = order.Size,
                                     Status = InstructionReportStatus.SUCCESS
                                 })
                .ToList();

            var placeResponse = new PlaceOrdersResponse
                       {
                           Id = 1,
                           Jsonrpc = "1",
                           Result = new PlaceExecutionReport
                                        {
                                            CustomerRef = null,
                                            ErrorCode = ExecutionReportErrorCode.SUCCESS,
                                            MarketId = marketId,
                                            Status = ExecutionReportStatus.SUCCESS,
                                            InstructionReports = instructionReports
                                        }
                       };
            return new StringContent(JsonConvert.SerializeObject(placeResponse));
        }
    }
}
