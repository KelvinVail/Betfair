namespace Betfair.Entities
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The order status.
    /// </summary>
    public class PlacedOrder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacedOrder"/> class.
        /// </summary>
        public PlacedOrder()
        {
            this.PlaceConfirmationUtc = DateTime.UtcNow;
            this.PlaceConfirmationTick = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Gets or sets the selection id.
        /// </summary>
        public long SelectionId { get; set; }

        /// <summary>
        /// Gets or sets the bet id.
        /// </summary>
        public string BetId { get; set; }

        /// <summary>
        /// Gets or sets the price requested.
        /// </summary>
        public double PriceRequested { get; set; }

        /// <summary>
        /// Gets or sets the average price matched.
        /// </summary>
        public double AveragePriceMatched { get; set; }

        /// <summary>
        /// Gets or sets the size requested.
        /// </summary>
        public double SizeRequested { get; set; }

        /// <summary>
        /// Gets or sets the size matched.
        /// </summary>
        public double SizeMatched { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this order is fully matched.
        /// </summary>
        public bool IsFullyMatched { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this placed order has been replaced or cancelled.
        /// </summary>
        public bool IsReplacedOrCancelled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether execution failed.
        /// </summary>
        public bool ExecutionFailed { get; set; }

        /// <summary>
        /// Gets the date and time the place was confirmed confirmation.
        /// </summary>
        public DateTime PlaceConfirmationUtc { get; }

        /// <summary>
        /// Gets the system timer tick at place confirmation.
        /// </summary>
        public long PlaceConfirmationTick { get; }
    }
}
