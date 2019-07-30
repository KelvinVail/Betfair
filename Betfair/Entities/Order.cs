namespace Betfair.Entities
{
    using System;
    using System.Diagnostics;

    using Betfair.Services.BetfairApi.Enums;

    using Side = Services.BetfairApi.Enums.Side;

    /// <summary>
    /// A single order.
    /// </summary>
    public class Order : OrderBase
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
            : base(selectionId, side, price, size)
        {
            this.OrderCreatedUtc = DateTime.UtcNow;
            this.OrderCreatedTick = Stopwatch.GetTimestamp();
        }

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
        /// Gets a value indicating whether this order is fully matched.
        /// </summary>
        public bool IsFullyMatched { get; internal set; }

        /// <summary>
        /// Sets this order to persist on market suspension.
        /// </summary>
        public void Persist()
        {
            this.PersistenceType = PersistenceType.PERSIST;
        }
    }
}
