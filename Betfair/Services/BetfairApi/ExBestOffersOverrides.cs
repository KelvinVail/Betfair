namespace Betfair.Services.BetfairApi
{
    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// The ex best offers overrides.
    /// </summary>
    public class ExBestOffersOverrides
    {
        /// <summary>
        /// Gets or sets the best prices depth.
        /// </summary>
        [JsonProperty(PropertyName = "bestPricesDepth")]
        public int BestPricesDepth { get; set; }

        /// <summary>
        /// Gets or sets the roll up model.
        /// </summary>
        [JsonProperty(PropertyName = "rollupModel")]
        public RollUpModel RollUpModel { get; set; }

        /// <summary>
        /// Gets or sets the roll up limit.
        /// </summary>
        [JsonProperty(PropertyName = "rollupLimit")]
        public int RollUpLimit { get; set; }

        /// <summary>
        /// Gets or sets the roll up liability threshold.
        /// </summary>
        [JsonProperty(PropertyName = "rollupLiabilityThreshold")]
        public double RollUpLiabilityThreshold { get; set; }

        /// <summary>
        /// Gets or sets the roll up liability factor.
        /// </summary>
        [JsonProperty(PropertyName = "rollupLiabilityFactor")]
        public int RollUpLiabilityFactor { get; set; }
    }
}