namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// Market catalogue parameters holds the parameters
    /// used in a Market Catalogue Request.
    /// </summary>
    public class MarketCatalogueParams
    {
        /// <summary>
        /// Gets or sets the filter to select desired markets.
        /// All markets that match the criteria in the filter are selected.
        /// Required.
        /// </summary>
        [JsonProperty(PropertyName = "filter")]
        public MarketCatalogueFilter Filter { get; set; }

        /// <summary>
        /// Gets or sets the type and amount of data returned about the market.
        /// </summary>
        [JsonProperty(PropertyName = "marketProjection")]
        public List<MarketProjection> MarketProjection { get; set; }

        /// <summary>
        /// Required.
        /// Limit on the total number of results returned, must be greater
        /// than 0 and less than or equal to 1000.
        /// </summary>
        [JsonProperty(PropertyName = "maxResults")]
        public string MaxResults => "100";

        /// <summary>
        /// The order of the results. Will default to RANK if not passed.
        /// RANK is an assigned priority that is determined by our Market
        /// Operations team in our back-end system. A result's overall rank
        /// is derived from the ranking given to the flowing attributes for the
        /// result. EventType, Competition, StartTime, MarketType,
        /// MarketId. For example, EventType is ranked by the most
        /// popular sports types and marketTypes are ranked in the
        /// following order: ODDS ASIAN LINE RANGE If all other
        /// dimensions of the result are equal, then the results are ranked
        /// in MarketId order.
        /// </summary>
        [JsonProperty(PropertyName = "sort")]
        public MarketSort Sort => MarketSort.FIRST_TO_START;
    }
}