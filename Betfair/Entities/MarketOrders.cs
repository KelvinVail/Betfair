namespace Betfair.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Betfair.Services.BetfairApi.Enums;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Request;
    using Betfair.Utils;

    using CSharpFunctionalExtensions;

    /// <summary>
    /// Market Orders.
    /// </summary>
    /// <typeparam name="TOrderBase">
    /// The Order type.
    /// </typeparam>
    public class MarketOrders<TOrderBase>
        where TOrderBase : Order
    {
        /// <summary>
        /// The orders.
        /// </summary>
        private readonly List<TOrderBase> orders;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOrders{TOrderBase}"/> class. 
        /// </summary>
        /// <param name="marketId">
        /// The market id.
        /// </param>
        /// <param name="customerStrategyRef">
        /// The customer strategy ref.
        /// </param>
        public MarketOrders(string marketId, string customerStrategyRef = null)
        {
            this.MarketId = marketId;
            this.CustomerStrategyRef = customerStrategyRef;
            this.orders = new List<TOrderBase>();
        }

        /// <summary>
        /// A unique id for this set of orders.
        /// </summary>
        public string Id => Guid.NewGuid().ToString();

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
        public IReadOnlyList<TOrderBase> Orders => this.orders;

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
            var order = new Order(selectionId, side, price, Math.Round(size, 2)) as TOrderBase;
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
        public Result AddOrder(TOrderBase order)
        {
            if (!PriceHelper.IsValidPrice(order.Price))
            {
                return Result.Fail("Order price is invalid.");
            }

            this.orders.Add(order);
            return Result.Ok();
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
