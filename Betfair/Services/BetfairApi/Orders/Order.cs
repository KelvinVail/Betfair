namespace Betfair.Services.BetfairApi.Orders
{
    using System;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// The order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets the bet id.
        /// </summary>
        [JsonProperty(PropertyName = "betId")]
        public string BetId { get; set; }

        /// <summary>
        /// Gets or sets the order type.
        /// </summary>
        [JsonProperty(PropertyName = "orderType")]
        public OrderType OrderType { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the persistence type.
        /// </summary>
        [JsonProperty(PropertyName = "persistenceType")]
        public PersistenceType PersistenceType { get; set; }

        /// <summary>
        /// Gets or sets the side.
        /// </summary>
        [JsonProperty(PropertyName = "side")]
        public Side Side { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public double Price { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public double Size { get; set; }

        /// <summary>
        /// Gets or sets the BSP liability.
        /// </summary>
        [JsonProperty(PropertyName = "bspLiability")]
        public double? BspLiability { get; set; }

        /// <summary>
        /// Gets or sets the placed date.
        /// </summary>
        [JsonProperty(PropertyName = "placedDate")]
        public DateTime? PlacedDate { get; set; }

        /// <summary>
        /// Gets or sets the avg price matched.
        /// </summary>
        [JsonProperty(PropertyName = "avgPriceMatched")]
        public double? AvgPriceMatched { get; set; }

        /// <summary>
        /// Gets or sets the size matched.
        /// </summary>
        [JsonProperty(PropertyName = "sizeMatched")]
        public double? SizeMatched { get; set; }

        /// <summary>
        /// Gets or sets the size remaining.
        /// </summary>
        [JsonProperty(PropertyName = "sizeRemaining")]
        public double? SizeRemaining { get; set; }

        /// <summary>
        /// Gets or sets the size lapsed.
        /// </summary>
        [JsonProperty(PropertyName = "sizeLapsed")]
        public double? SizeLapsed { get; set; }

        /// <summary>
        /// Gets or sets the size cancelled.
        /// </summary>
        [JsonProperty(PropertyName = "sizeCancelled")]
        public double? SizeCancelled { get; set; }

        /// <summary>
        /// Gets or sets the size voided.
        /// </summary>
        [JsonProperty(PropertyName = "sizeVoided")]
        public double? SizeVoided { get; set; }

        /// <summary>
        /// Gets or sets the customer order ref.
        /// </summary>
        [JsonProperty(PropertyName = "customerOrderRef")]
        public string CustomerOrderRef { get; set; }

        /// <summary>
        /// Gets or sets the customer strategy ref.
        /// </summary>
        [JsonProperty(PropertyName = "customerStrategyRef")]
        public string CustomerStrategyRef { get; set; }
    }
}