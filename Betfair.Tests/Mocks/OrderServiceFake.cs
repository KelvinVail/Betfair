namespace Betfair.Tests.Mocks
{
    using System.Threading.Tasks;

    using Betfair.Entities;
    using Betfair.Services;

    /// <summary>
    /// The betfair betfairClient fake.
    /// </summary>
    public class OrderServiceFake : IOrderService
    {
        /// <summary>
        /// Gets or sets the order book.
        /// </summary>
        public OrderBook OrderBook { get; set; }

        /// <summary>
        /// The place orders async.
        /// </summary>
        /// <param name="orderBook">
        /// The order book.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<OrderBookResult> PlaceOrdersAsync(OrderBook orderBook)
        {
            this.OrderBook = orderBook;
            return await Task.Run(() => new OrderBookResult());
        }
    }
}
