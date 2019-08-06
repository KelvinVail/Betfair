namespace Betfair.Examples
{
    using System.Linq;
    using System.Threading.Tasks;

    using Betfair.Entities;

    /// <summary>
    /// An example of Bookmaking.
    /// </summary>
    public class BookmakingExample : MarketOrders<OrderExtension>
    {
        /// <summary>
        /// The betfair betfairClient.
        /// </summary>
        private readonly IBetfairClient betfairClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmakingExample"/> class. 
        /// </summary>
        /// <param name="betfairClient">
        /// The betfair BetfairClient.
        /// </param>
        /// <param name="marketId">
        /// The market id.
        /// </param>
        /// <param name="customerStrategyRef">
        /// The customer strategy ref.
        /// </param>
        public BookmakingExample(IBetfairClient betfairClient, string marketId, string customerStrategyRef = null)
            : base(marketId, customerStrategyRef)
        {
            this.betfairClient = betfairClient;
        }

        /// <summary>
        /// This order book can execute.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute()
        {
            if (this.CustomerStrategyRef != null && this.CustomerStrategyRef.Length > 15)
            {
                return false;
            }

            return this.Orders.Any(w => !w.Placed);
        }

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ExecuteAsync()
        {
            if (this.CanExecute())
            {
                var reports = await this.betfairClient.OrderService.PlaceOrdersAsync<BookmakingExample, OrderExtension>(this);
                foreach (var order in this.Orders)
                {
                    var report = reports.FirstOrDefault(w => w.SelectionId == order.SelectionId);
                    if (report == null)
                    {
                        continue;
                    }

                    order.PlacedOrder = report;
                    order.Placed = !report.ExecutionFailed;
                }
            }
        }
    }
}
