namespace Betfair.Services.BetfairApi.Orders.PlaceOrders.Request
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The place orders.
    /// Place new orders into market. LIMIT orders below the minimum bet size are allowed if
    /// there is an unmatched bet at the same price in the market. This operation is atomic in that
    /// all orders will be placed or none will be placed.
    /// </summary>
    internal sealed class PlaceOrders
    {
        /// <summary>
        /// Gets or sets the async.
        /// If true places orders without waiting for a response
        /// </summary>
        [JsonProperty(PropertyName = "async")]
        internal bool? Async { get; set; }

        /// <summary>
        /// Gets or sets the customer ref.
        /// Used to de-dup
        /// 32 char limit
        /// </summary>
        [JsonProperty(PropertyName = "customerRef")]
        internal string CustomerRef { get; set; }

        /// <summary>
        /// Gets or sets the customer strategy ref.
        /// Passed back on Stream API 
        /// Possibly customerOrderRef on API-NG?
        /// 15 char limit
        /// </summary>
        [JsonProperty(PropertyName = "customerStrategyRef")]
        internal string CustomerStrategyRef { get; set; }

        /// <summary>
        /// Gets or sets the instructions.
        /// </summary>
        [JsonProperty(PropertyName = "instructions")]
        internal List<PlaceInstruction> Instructions { get; set; }

        /// <summary>
        /// Gets or sets the market id.
        /// </summary>
        [JsonProperty(PropertyName = "marketId")]
        internal string MarketId { get; set; }

        /// <summary>
        /// Gets or sets the market version.
        /// Used to avoid placing bets into an unintended version 
        /// i.e. any suspension (runner removal) will increment the version
        /// </summary>
        [JsonProperty(PropertyName = "marketVersion")]
        internal long? MarketVersion { get; set; }
    }
}