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
        /// Gets the bet id.
        /// </summary>
        public string BetId { get; internal set; }

        /// <summary>
        /// Gets the price requested.
        /// </summary>
        public double PriceRequested { get; internal set; }

        /// <summary>
        /// Gets the average price matched.
        /// </summary>
        public double AveragePriceMatched { get; internal set; }

        /// <summary>
        /// Gets the size requested.
        /// </summary>
        public double SizeRequested { get; internal set; }

        /// <summary>
        /// Gets the size matched.
        /// </summary>
        public double SizeMatched { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this placed order has been replaced or cancelled.
        /// </summary>
        public bool IsReplacedOrCancelled { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether execution failed.
        /// </summary>
        public bool ExecutionFailed { get; internal set; }

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
