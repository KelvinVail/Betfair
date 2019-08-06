namespace Example.Tests
{
    using System.Linq;
    using System.Threading.Tasks;

    using Betfair.Examples;
    using Betfair.Services.BetfairApi.Enums;

    using Example.Tests.Mocks;

    using Xunit;

    /// <summary>
    /// The bookmaking example should...
    /// </summary>
    public class BookmakingExampleShould
    {
        /// <summary>
        /// The betfair client fake.
        /// </summary>
        private readonly BetfairClientFake betfairClientFake;

        /// <summary>
        /// The order service fake.
        /// </summary>
        private readonly OrderServiceFake orderServiceFake = new OrderServiceFake();

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmakingExampleShould"/> class.
        /// </summary>
        public BookmakingExampleShould()
        {
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
            var order = new OrderExtension(12345, Side.BACK, 1.01, 2);
            var sut = new BookmakingExample(this.betfairClientFake, MarketId);
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
            var sut = new BookmakingExample(this.betfairClientFake, "fakeMarketId");
            var order = new OrderExtension(12345, Side.LAY, 1.01, 2);
            sut.AddOrder(order);

            // Act
            await sut.ExecuteAsync();

            // Assert
            var placed = sut.Orders.Where(w => w.SelectionId == 12345).Select(s => s.Placed).FirstOrDefault();
            Assert.True(placed);
        }

        /// <summary>
        /// ... not accept an order if the price is invalid.
        /// </summary>
        [Fact]
        public void NotAcceptAnOrderIfThePriceIsInvalid()
        {
            // Arrange
            var sut = new BookmakingExample(this.betfairClientFake, "fakeMarketId");
            var order = new OrderExtension(12345, Side.LAY, 1001, 2);

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
            var sut = new BookmakingExample(this.betfairClientFake, "fakeMarketId");

            // Act
            await sut.ExecuteAsync();

            // Assert
            Assert.False(sut.CanExecute());
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
            var sut = new BookmakingExample(this.betfairClientFake, "fakeMarketId", "0123456789012345");
            var order = new OrderExtension(12345, Side.LAY, 1.01, 2);
            sut.AddOrder(order);

            // Act
            await sut.ExecuteAsync();

            // Assert
            Assert.False(sut.CanExecute());
        }
    }
}
