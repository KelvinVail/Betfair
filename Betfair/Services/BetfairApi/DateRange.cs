namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    /// <summary>
    /// The date range.
    /// </summary>
    public class DateRange
    {
        /// <summary>
        /// Gets or sets the from date.
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the to date.
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }
    }
}