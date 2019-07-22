namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The filter to select desired markets. All markets that match the
    /// criteria in the filter are selected.
    /// </summary>
    public class MarketCatalogueFilter
    {
        /// <summary>
        /// Gets or sets the list of event type ids to be returned.
        /// </summary>
        [JsonProperty(PropertyName = "eventTypeIds")]
        public List<string> EventTypeIds { get; set; }

        /// <summary>
        /// Gets or sets the list of market countries to be returned.
        /// </summary>
        [JsonProperty(PropertyName = "marketCountries")]
        public List<string> MarketCountries { get; set; }

        /// <summary>
        /// Gets or sets the date range on market start time to be returned.
        /// </summary>
        [JsonProperty(PropertyName = "marketStartTime")]
        public DateRange MarketStartTime { get; set; }

        /// <summary>
        /// Gets or sets the list of market type codes to be returned.
        /// </summary>
        [JsonProperty(PropertyName = "marketTypeCodes")]
        public List<string> MarketTypeCodes { get; set; }
    }
}