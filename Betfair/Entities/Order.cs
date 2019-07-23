namespace Betfair.Entities
{
    using System;
    using System.Diagnostics;

    using Betfair.Services.BetfairApi.Enums;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Request;
    using Betfair.Utils;

    using Side = Services.BetfairApi.Enums.Side;

    /// <summary>
    /// A single order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
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
        public Order(
            long selectionId,
            Side side,
            double price,
            double size)
        {
            this.SelectionId = selectionId;
            this.Side = side;
            this.Price = price;
            this.Size = size;
            this.PersistenceType = PersistenceType.LAPSE;
            this.OrderCreatedUtc = DateTime.UtcNow;
            this.OrderCreatedTick = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Gets the selection id.
        /// </summary>
        public long SelectionId { get; }

        /// <summary>
        /// Gets the side.
        /// </summary>
        public Side Side { get; }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public double Price { get; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        public double Size { get; }

        /// <summary>
        /// Gets or sets the persistence type.
        /// </summary>
        public PersistenceType PersistenceType { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this order has been placed.
        /// </summary>
        public bool Placed { get; internal set; }

        /// <summary>
        /// Gets the order status.
        /// </summary>
        public PlacedOrder PlacedOrder { get; internal set; }

        /// <summary>
        /// Gets the date and time the order was created.
        /// </summary>
        public DateTime OrderCreatedUtc { get; }

        /// <summary>
        /// Gets the system timer tick at order created.
        /// </summary>
        public long OrderCreatedTick { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this order is fully matched.
        /// </summary>
        public bool IsFullyMatched { get; protected set; }

        /// <summary>
        /// Is stake below the minimum?
        /// </summary>
        internal bool IsStakeBelowMinimum => StakeHelper.IsStakeBelowMinimum(this.Size, this.Price);

        /// <summary>
        /// Sets this order to persist on market suspension.
        /// </summary>
        public void Persist()
        {
            this.PersistenceType = PersistenceType.PERSIST;
        }

        /// <summary>
        /// The place instruction.
        /// </summary>
        /// <returns>
        /// The <see cref="PlaceInstruction"/>.
        /// </returns>
        internal PlaceInstruction PlaceInstruction()
        {
            if (this.IsStakeBelowMinimum)
            {
                return new PlaceInstruction()
                           {
                               SelectionId = this.SelectionId,
                               Side = this.Side,
                               OrderType = OrderType.LIMIT,
                               LimitOrder = new LimitOrder()
                                                {
                                                    PersistenceType = this.PersistenceType,
                                                    Price = this.Side == Side.BACK ? 1000 : 1.01,
                                                    Size = 2.0
                                                }
                           };
            }

            return new PlaceInstruction()
                       {
                           SelectionId = this.SelectionId,
                           Side = this.Side,
                           OrderType = OrderType.LIMIT,
                           LimitOrder = new LimitOrder()
                                            {
                                                PersistenceType = this.PersistenceType,
                                                Price = this.Price,
                                                Size = this.Size
                                            }
                       };
        }
    }
}
