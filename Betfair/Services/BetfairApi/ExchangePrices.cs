namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The exchange prices.
    /// </summary>
    public class ExchangePrices
    {
        /// <summary>
        /// Gets or sets the available to back.
        /// </summary>
        [JsonProperty(PropertyName = "availableToBack")]
        public List<PriceSize> AvailableToBack { get; set; }

        /// <summary>
        /// Gets or sets the available to lay.
        /// </summary>
        [JsonProperty(PropertyName = "availableToLay")]
        public List<PriceSize> AvailableToLay { get; set; }

        /// <summary>
        /// Gets or sets the traded volume.
        /// </summary>
        [JsonProperty(PropertyName = "tradedVolume")]
        public List<PriceSize> TradedVolume { get; set; }
    }
}