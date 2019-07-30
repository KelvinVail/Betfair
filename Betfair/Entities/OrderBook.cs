namespace Betfair.Entities
{
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// An order book.
    /// </summary>
    public class OrderBook : OrderBookBase<Order>
    {
        /// <summary>
        /// The betfair betfairClient.
        /// </summary>
        private readonly IBetfairClient betfairClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBook"/> class. 
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
        public OrderBook(IBetfairClient betfairClient, string marketId, string customerStrategyRef = null)
            : base(marketId, customerStrategyRef)
        {
            this.betfairClient = betfairClient;
        }

        /// <summary>
        /// Gets or sets a value indicating whether any orders are below the minimum stake.
        /// </summary>
        internal bool HasOrdersBelowMinimum => this.Orders.Any(o => o.IsStakeBelowMinimum);

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
                var reports = await this.betfairClient.OrderService.PlaceOrdersAsync(this);
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
