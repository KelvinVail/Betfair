namespace Betfair.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Betfair.Entities;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Response;

    /// <summary>
    /// The OrderService interface.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Place orders.
        /// </summary>
        /// <param name="orderBook">
        /// The order book.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<List<PlacedOrder>> PlaceOrdersAsync(OrderBook orderBook);
    }
}