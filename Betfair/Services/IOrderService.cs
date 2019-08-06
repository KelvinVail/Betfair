namespace Betfair.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Betfair.Entities;

    /// <summary>
    /// The OrderService interface.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Place orders.
        /// </summary>
        /// <typeparam name="TOrderBook">
        /// The OrderBook type.
        /// </typeparam>
        /// <typeparam name="TOrder">
        /// The Order type.
        /// </typeparam>
        /// <param name="orderBook">
        /// The order book.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<List<PlacedOrder>> PlaceOrdersAsync<TOrderBook, TOrder>(TOrderBook orderBook)
            where TOrderBook : MarketOrders<TOrder> where TOrder : Order;
    }
}