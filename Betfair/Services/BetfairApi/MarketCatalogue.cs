namespace Betfair.Services.BetfairApi
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The market catalogue.
    /// </summary>
    public class MarketCatalogue
    {
        /// <summary>
        /// Gets or sets the market id.
        /// </summary>
        [JsonProperty(PropertyName = "marketId")]
        public string MarketId { get; set; }

        /// <summary>
        /// Gets or sets the market name.
        /// </summary>
        [JsonProperty(PropertyName = "marketName")]
        public string MarketName { get; set; }

        /// <summary>
        /// Gets or sets the market start time.
        /// </summary>
        [JsonProperty(PropertyName = "marketStartTime")]
        public DateTime MarketStartTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is market data delayed.
        /// </summary>
        [JsonProperty(PropertyName = "isMarketDataDelayed")]
        public bool IsMarketDataDelayed { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public MarketDescription Description { get; set; }

        /// <summary>
        /// Gets or sets the total matched.
        /// </summary>
        [JsonProperty(PropertyName = "totalMatched")]
        public double TotalMatched { get; set; }

        /// <summary>
        /// Gets or sets the runners.
        /// </summary>
        [JsonProperty(PropertyName = "runners")]
        public List<RunnerCatalog> Runners { get; set; }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        [JsonProperty(PropertyName = "eventType")]
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the competition.
        /// </summary>
        [JsonProperty(PropertyName = "competition")]
        public Competition Competition { get; set; }

        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        [JsonProperty(PropertyName = "event")]
        public Event Event { get; set; }
    }
}