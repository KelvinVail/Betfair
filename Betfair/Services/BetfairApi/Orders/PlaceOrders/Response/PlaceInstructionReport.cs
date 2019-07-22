namespace Betfair.Services.BetfairApi.Orders.PlaceOrders.Response
{
    using System;

    using Betfair.Services.BetfairApi.Enums;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Request;

    using Newtonsoft.Json;

    /// <summary>
    /// The place instruction report.
    /// Response to a PlaceInstruction.
    /// </summary>
    internal sealed class PlaceInstructionReport
    {
        /// <summary>
        /// Gets or sets the status.
        /// Whether the command succeeded or failed
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        internal InstructionReportStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// Cause of failure, or null if command succeeds.
        /// </summary>
        [JsonProperty(PropertyName = "errorCode")]
        internal InstructionReportErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the instruction.
        /// The instruction that was requested.
        /// </summary>
        [JsonProperty(PropertyName = "instruction")]
        internal PlaceInstruction Instruction { get; set; }

        /// <summary>
        /// Gets or sets the bet id.
        /// The bet ID of the new bet. Will be null on failure or if order was placed asynchronously.
        /// </summary>
        [JsonProperty(PropertyName = "betId")]
        internal string BetId { get; set; }

        /// <summary>
        /// Gets or sets the placed date.
        /// Will be null if order was placed asynchronously.
        /// </summary>
        [JsonProperty(PropertyName = "placedDate")]
        internal DateTime? PlacedDate { get; set; }

        /// <summary>
        /// Gets or sets the average price matched.
        /// Will be null if order was placed asynchronously.
        /// This value is not meaningful for activity on LINE markets and is not guaranteed to be returned or maintained for these markets.
        /// </summary>
        [JsonProperty(PropertyName = "averagePriceMatched")]
        internal double? AveragePriceMatched { get; set; }

        /// <summary>
        /// Gets or sets the size matched.
        /// Will be null if order was placed asynchronously
        /// </summary>
        [JsonProperty(PropertyName = "sizeMatched")]
        internal double? SizeMatched { get; set; }

        /// <summary>
        /// Gets or sets the order status.
        /// The status of the order, if the instruction succeeded.
        /// If the instruction was unsuccessful, no value is provided.
        /// </summary>
        [JsonProperty(PropertyName = "orderStatus")]
        internal OrderStatus OrderStatus { get; set; }
    }
}