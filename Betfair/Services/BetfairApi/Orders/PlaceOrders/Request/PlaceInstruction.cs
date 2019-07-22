namespace Betfair.Services.BetfairApi.Orders.PlaceOrders.Request
{
    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// The place instruction.
    /// </summary>
    internal sealed class PlaceInstruction
    {
        /// <summary>
        /// Gets or sets the order type.
        /// </summary>
        [JsonProperty(PropertyName = "orderType")]
        internal OrderType OrderType { get; set; }

        /// <summary>
        /// Gets or sets the selection id.
        /// </summary>
        [JsonProperty(PropertyName = "selectionId")]
        internal long SelectionId { get; set; }

        /// <summary>
        /// Gets or sets the handicap.
        /// </summary>
        [JsonProperty(PropertyName = "handicap")]
        internal double? Handicap { get; set; }

        /// <summary>
        /// Gets or sets the side.
        /// </summary>
        [JsonProperty(PropertyName = "side")]
        internal Side Side { get; set; }

        /// <summary>
        /// Gets or sets the limit order.
        /// </summary>
        [JsonProperty(PropertyName = "limitOrder")]
        internal LimitOrder LimitOrder { get; set; }

        /// <summary>
        /// Gets or sets the limit on close order.
        /// </summary>
        [JsonProperty(PropertyName = "limitOnCloseOrder")]
        internal LimitOnCloseOrder LimitOnCloseOrder { get; set; }

        /// <summary>
        /// Gets or sets the market on close order.
        /// </summary>
        [JsonProperty(PropertyName = "marketOnCloseOrder")]
        internal MarketOnCloseOrder MarketOnCloseOrder { get; set; }
    }
}