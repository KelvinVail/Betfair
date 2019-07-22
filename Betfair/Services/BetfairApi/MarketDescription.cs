namespace Betfair.Services.BetfairApi
{
    using System;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// The market description.
    /// </summary>
    public class MarketDescription
    {
        /// <summary>
        /// Gets or sets a value indicating whether is persistence enabled.
        /// </summary>
        [JsonProperty(PropertyName = "persistenceEnabled")]
        public bool IsPersistenceEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is BSP market.
        /// </summary>
        [JsonProperty(PropertyName = "bspMarket")]
        public bool IsBspMarket { get; set; }

        /// <summary>
        /// Gets or sets the market time.
        /// </summary>
        [JsonProperty(PropertyName = "marketTime")]
        public DateTime MarketTime { get; set; }

        /// <summary>
        /// Gets or sets the suspend time.
        /// </summary>
        [JsonProperty(PropertyName = "suspendTime")]
        public DateTime? SuspendTime { get; set; }

        /// <summary>
        /// Gets or sets the settle time.
        /// </summary>
        [JsonProperty(PropertyName = "settleTime")]
        public DateTime? SettleTime { get; set; }

        /// <summary>
        /// Gets or sets the betting type.
        /// </summary>
        [JsonProperty(PropertyName = "bettingType")]
        public MarketBettingType BettingType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is turn in play enabled.
        /// </summary>
        [JsonProperty(PropertyName = "turnInPlayEnabled")]
        public bool IsTurnInPlayEnabled { get; set; }

        /// <summary>
        /// Gets or sets the market type.
        /// </summary>
        [JsonProperty(PropertyName = "marketType")]
        public string MarketType { get; set; }

        /// <summary>
        /// Gets or sets the regulator.
        /// </summary>
        [JsonProperty(PropertyName = "regulator")]
        public string Regulator { get; set; }

        /// <summary>
        /// Gets or sets the market base rate.
        /// </summary>
        [JsonProperty(PropertyName = "marketBaseRate")]
        public double MarketBaseRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is discount allowed.
        /// </summary>
        [JsonProperty(PropertyName = "discountAllowed")]
        public bool IsDiscountAllowed { get; set; }

        /// <summary>
        /// Gets or sets the wallet.
        /// </summary>
        [JsonProperty(PropertyName = "wallet")]
        public string Wallet { get; set; }

        /// <summary>
        /// Gets or sets the rules.
        /// </summary>
        [JsonProperty(PropertyName = "rules")]
        public string Rules { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether rules has date.
        /// </summary>
        [JsonProperty(PropertyName = "rulesHasDate")]
        public bool RulesHasDate { get; set; }

        /// <summary>
        /// Gets or sets the each way divisor.
        /// </summary>
        [JsonProperty(PropertyName = "eachWayDivisor")]
        public double EachWayDivisor { get; set; }

        /// <summary>
        /// Gets or sets the clarifications.
        /// </summary>
        [JsonProperty(PropertyName = "clarifications")]
        public string Clarifications { get; set; }
    }
}