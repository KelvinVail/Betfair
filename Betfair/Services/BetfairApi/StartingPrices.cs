namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The starting prices.
    /// </summary>
    public class StartingPrices
    {
        /// <summary>
        /// Gets or sets the near price.
        /// </summary>
        [JsonProperty(PropertyName = "nearPrice")]
        public double NearPrice { get; set; }

        /// <summary>
        /// Gets or sets the far price.
        /// </summary>
        [JsonProperty(PropertyName = "farPrice")]
        public double FarPrice { get; set; }

        /// <summary>
        /// Gets or sets the back stake taken.
        /// </summary>
        [JsonProperty(PropertyName = "backStakeTaken")]
        public List<PriceSize> BackStakeTaken { get; set; }

        /// <summary>
        /// Gets or sets the lay liability taken.
        /// </summary>
        [JsonProperty(PropertyName = "layLiabilityTaken")]
        public List<PriceSize> LayLiabilityTaken { get; set; }

        /// <summary>
        /// Gets or sets the actual SP.
        /// </summary>
        [JsonProperty(PropertyName = "actualSP")]
        public double ActualSp { get; set; }
    }
}