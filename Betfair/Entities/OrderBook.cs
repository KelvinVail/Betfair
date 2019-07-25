namespace Betfair.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Betfair.Services.BetfairApi.Enums;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Request;
    using Betfair.Utils;

    using CSharpFunctionalExtensions;

    /// <summary>
    /// An order book.
    /// </summary>
    public class OrderBook
    {
        /// <summary>
        /// The orders.
        /// </summary>
        private readonly List<Order> orders;

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
        {
            this.MarketId = marketId;
            this.betfairClient = betfairClient;
            this.CustomerStrategyRef = customerStrategyRef;
            this.orders = new List<Order>();
        }

        /// <summary>
        /// The order book id.
        /// </summary>
        public string OrderBookId => Guid.NewGuid().ToString();

        /// <summary>
        /// Gets the market id.
        /// </summary>
        public string MarketId { get; }

        /// <summary>
        /// Gets the customer strategy ref.
        /// An optional reference customers can use to specify which strategy has sent the order. 
        /// The reference will be returned on order change messages through the stream API. The string is 
        /// limited to 15 characters. If an empty string is provided it will be treated as null.
        /// </summary>
        public string CustomerStrategyRef { get; }

        /// <summary>
        /// The orders.
        /// </summary>
        public IReadOnlyList<Order> Orders => this.orders;

        /// <summary>
        /// Gets or sets a value indicating whether any orders are below the minimum stake.
        /// </summary>
        internal bool HasOrdersBelowMinimum => this.Orders.Any(o => o.IsStakeBelowMinimum);

        /// <summary>
        /// The add order.
        /// </summary>
        /// <param name="selectionId">
        /// The selection id.
        /// </param>
        /// <param name="side">
        /// The side.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        public Result AddOrder(long selectionId, Side side, double price, double size)
        {
            var order = new Order(selectionId, side, price, Math.Round(size, 2));
            return this.AddOrder(order);
        }

        /// <summary>
        /// The add order.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="Result"/>.
        /// </returns>
        public Result AddOrder(Order order)
        {
            if (!PriceHelper.IsValidPrice(order.Price))
            {
                return Result.Fail("Order price is invalid.");
            }

            this.orders.Add(order);
            return Result.Ok();
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

            return this.orders.Any(w => !w.Placed);
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
                foreach (var order in this.orders)
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

        /// <summary>
        /// The place orders request.
        /// </summary>
        /// <returns>
        /// The <see cref="PlaceOrdersRequest"/>.
        /// </returns>
        internal PlaceOrdersRequest PlaceOrdersRequest()
        {
            return new PlaceOrdersRequest
                       {
                Params = new PlaceOrders
                             { 
                    MarketId = this.MarketId,
                    CustomerStrategyRef = this.CustomerStrategyRef,
                    Instructions = this.orders.Select(o => o.PlaceInstruction()).ToList()
                }
            };
        }
    }
}
