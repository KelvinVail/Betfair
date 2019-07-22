namespace Betfair.Services.BetfairApi
{
    using System;
    using System.Collections.Generic;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// The market book.
    /// </summary>
    public class MarketBook
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketBook"/> class.
        /// </summary>
        public MarketBook()
        {
            this.Created = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the millisecond latency.
        /// </summary>
        public long MillisecondLatency { get; set; }

        /// <summary>
        /// Gets or sets the market id.
        /// </summary>
        [JsonProperty(PropertyName = "marketId")]
        public string MarketId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is market data delayed.
        /// </summary>
        [JsonProperty(PropertyName = "isMarketDataDelayed")]
        public bool IsMarketDataDelayed { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public MarketStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the bet delay.
        /// </summary>
        [JsonProperty(PropertyName = "betDelay")]
        public int BetDelay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is BSP reconciled.
        /// </summary>
        [JsonProperty(PropertyName = "bspReconciled")]
        public bool IsBspReconciled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is complete.
        /// </summary>
        [JsonProperty(PropertyName = "complete")]
        public bool IsComplete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is in play.
        /// </summary>
        [JsonProperty(PropertyName = "inplay")]
        public bool IsInplay { get; set; }

        /// <summary>
        /// Gets or sets the number of winners.
        /// </summary>
        [JsonProperty(PropertyName = "numberOfWinners")]
        public int NumberOfWinners { get; set; }

        /// <summary>
        /// Gets or sets the number of runners.
        /// </summary>
        [JsonProperty(PropertyName = "numberOfRunners")]
        public int NumberOfRunners { get; set; }

        /// <summary>
        /// Gets or sets the number of active runners.
        /// </summary>
        [JsonProperty(PropertyName = "numberOfActiveRunners")]
        public int NumberOfActiveRunners { get; set; }

        /// <summary>
        /// Gets or sets the last match time.
        /// </summary>
        [JsonProperty(PropertyName = "lastMatchTime")]
        public DateTime? LastMatchTime { get; set; }

        /// <summary>
        /// Gets or sets the total matched.
        /// </summary>
        [JsonProperty(PropertyName = "totalMatched")]
        public double TotalMatched { get; set; }

        /// <summary>
        /// Gets or sets the total available.
        /// </summary>
        [JsonProperty(PropertyName = "totalAvailable")]
        public double TotalAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is cross matching.
        /// </summary>
        [JsonProperty(PropertyName = "crossMatching")]
        public bool IsCrossMatching { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is runners voidable.
        /// </summary>
        [JsonProperty(PropertyName = "runnersVoidable")]
        public bool IsRunnersVoidable { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public long Version { get; set; }

        /// <summary>
        /// Gets or sets the runners.
        /// </summary>
        [JsonProperty(PropertyName = "runners")]
        public List<Runner> Runners { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the db id.
        /// </summary>
        public int DbId { get; set; }
    }
}