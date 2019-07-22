namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    /// <summary>
    /// The price size.
    /// </summary>
    public class PriceSize
    {
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
    }
}