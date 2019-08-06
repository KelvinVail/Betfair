namespace Example.Tests.Mocks
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Betfair.Entities;
    using Betfair.Services;

    /// <summary>
    /// The betfair betfairClient fake.
    /// </summary>
    public class OrderServiceFake : IOrderService
    {
        /// <summary>
        /// Gets a value indicating whether place orders async executed.
        /// </summary>
        public bool PlaceOrdersAsyncExecuted { get; internal set; }

        /// <inheritdoc/>
        public async Task<List<PlacedOrder>> PlaceOrdersAsync<TMarketOrders, TOrder>(TMarketOrders orderBook)
            where TMarketOrders : MarketOrders<TOrder> where TOrder : Order
        {
            this.PlaceOrdersAsyncExecuted = true;
            return await Task.Run(() => FullMatch<TMarketOrders, TOrder>(orderBook));
        }

        /// <summary>
        /// The full match.
        /// </summary>
        /// <param name="orderBook">
        /// The order book.
        /// </param>
        /// <typeparam name="TMarketOrders">
        /// The type of the market orders.
        /// </typeparam>
        /// <typeparam name="TOrder">
        /// The type of the orders.
        /// </typeparam>
        /// <returns>
        /// The <see cref="List{PlaceOrder}"/>.
        /// </returns>
        private static List<PlacedOrder> FullMatch<TMarketOrders, TOrder>(TMarketOrders orderBook)
            where TMarketOrders : MarketOrders<TOrder> where TOrder : Order
        {
            return orderBook.Orders.Select(
                    order => new PlacedOrder()
                                 {
                                     AveragePriceMatched = order.Price,
                                     BetId = "1",
                                     IsFullyMatched = true,
                                     PriceRequested = order.Price,
                                     SelectionId = order.SelectionId,
                                     SizeMatched = order.Size,
                                     SizeRequested = order.Size
                                 })
                .ToList();
        }
    }
}
