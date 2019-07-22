namespace Betfair.Services.BetfairApi
{
    using System;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// The matched orders.
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Gets or sets the bet id.
        /// </summary>
        [JsonProperty(PropertyName = "betId")]
        public string BetId { get; set; }

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
        /// Gets or sets the match date.
        /// </summary>
        [JsonProperty(PropertyName = "matchDate")]
        public DateTime MatchDate { get; set; }
    }
}