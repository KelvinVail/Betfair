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

        /// <inheritdoc/>
        public async Task<List<PlacedOrder>> PlaceOrdersAsync<TOrderBook, TOrder>(TOrderBook orderBook)
            where TOrderBook : MarketOrders<TOrder> where TOrder : Order
        {
            this.PlaceOrdersAsyncExecuted = true;
            return await Task.Run(() => new List<PlacedOrder>());
        }
    }
}
