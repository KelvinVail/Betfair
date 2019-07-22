namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// The price projection.
    /// </summary>
    public class PriceProjection
    {
        /// <summary>
        /// Gets or sets the price data.
        /// </summary>
        [JsonProperty(PropertyName = "priceData")]
        public ISet<PriceData> PriceData { get; set; }

        /// <summary>
        /// Gets or sets the ex best offers overrides.
        /// </summary>
        [JsonProperty(PropertyName = "exBestOffersOverrides")]
        public ExBestOffersOverrides ExBestOffersOverrides { get; set; }

        /// <summary>
        /// Gets or sets the virtualise.
        /// </summary>
        [JsonProperty(PropertyName = "virtualise")]
        public bool? Virtualise { get; set; }

        /// <summary>
        /// Gets or sets the rollover stakes.
        /// </summary>
        [JsonProperty(PropertyName = "rolloverStakes")]
        public bool? RolloverStakes { get; set; }
    }
}