namespace Betfair.Tests.Mocks
{
    using System.Collections.Generic;
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

        /// <summary>
        /// The place orders async.
        /// </summary>
        /// <param name="orderBook">
        /// The order book.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<List<PlacedOrder>> PlaceOrdersAsync(OrderBook orderBook)
        {
            this.PlaceOrdersAsyncExecuted = true;
            return await Task.Run(() => new List<PlacedOrder>());
        }
    }
}
