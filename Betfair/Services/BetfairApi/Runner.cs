namespace Betfair.Services.BetfairApi
{
    using System;
    using System.Collections.Generic;

    using Betfair.Services.BetfairApi.Enums;
    using Betfair.Services.BetfairApi.Orders;

    using Newtonsoft.Json;

    /// <summary>
    /// The runner.
    /// </summary>
    public class Runner
    {
        /// <summary>
        /// Gets or sets the market book.
        /// </summary>
        public MarketBook MarketBook { get; set; }

        /// <summary>
        /// Gets or sets the selection id.
        /// </summary>
        [JsonProperty(PropertyName = "selectionId")]
        public long SelectionId { get; set; }

        /// <summary>
        /// Gets or sets the handicap.
        /// </summary>
        [JsonProperty(PropertyName = "handicap")]
        public double? Handicap { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public RunnerStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the adjustment factor.
        /// </summary>
        [JsonProperty(PropertyName = "adjustmentFactor")]
        public double? AdjustmentFactor { get; set; }

        /// <summary>
        /// Gets or sets the last price traded.
        /// </summary>
        [JsonProperty(PropertyName = "lastPriceTraded")]
        public double? LastPriceTraded { get; set; }

        /// <summary>
        /// Gets or sets the total matched.
        /// </summary>
        [JsonProperty(PropertyName = "totalMatched")]
        public double TotalMatched { get; set; }

        /// <summary>
        /// Gets or sets the removal date.
        /// </summary>
        [JsonProperty(PropertyName = "removalDate")]
        public DateTime? RemovalDate { get; set; }

        /// <summary>
        /// Gets or sets the starting prices.
        /// </summary>
        [JsonProperty(PropertyName = "sp")]
        public StartingPrices StartingPrices { get; set; }

        /// <summary>
        /// Gets or sets the exchange prices.
        /// </summary>
        [JsonProperty(PropertyName = "ex")]
        public ExchangePrices ExchangePrices { get; set; }

        /// <summary>
        /// Gets or sets the orders.
        /// </summary>
        [JsonProperty(PropertyName = "orders")]
        public List<Order> Orders { get; set; }

        /// <summary>
        /// Gets or sets the matches.
        /// </summary>
        [JsonProperty(PropertyName = "matches")]
        public List<Match> Matches { get; set; }

        /// <summary>
        /// Gets or sets the matched by strategy.
        /// </summary>
        [JsonProperty(PropertyName = "matchesByStrategy")]
        public Dictionary<string, Dictionary<string, List<Match>>> MatchedByStrategy { get; set; }
    }
}