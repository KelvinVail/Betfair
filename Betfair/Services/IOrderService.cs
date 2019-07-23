namespace Betfair.Services
{
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
        /// <param name="orderBook">
        /// The order book.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task PlaceOrdersAsync(OrderBook orderBook);
    }
}