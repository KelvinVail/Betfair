namespace Betfair.Services.BetfairApi
{
    using System;
    using System.Collections.Generic;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// Requests a list of information about published (ACTIVE/SUSPENDED) markets that does not change (or changes very rarely). You
    /// use MarketCatalogueRequest to retrieve the name of the market, the names of selections and other information about markets. Market
    /// Data Request Limits apply to requests made to MarketCatalogueRequest.
    /// Please note: MarketCatalogueRequest does not return markets that are CLOSED.
    /// </summary>
    public class MarketCatalogueRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketCatalogueRequest"/> class. 
        /// </summary>
        /// <param name="from">
        /// Get markets starting after the from date.
        /// </param>
        /// <param name="to">
        /// Get markets starting before to to date.
        /// </param>
        /// <returns>
        /// A <see cref="MarketCatalogueRequest"/>.
        /// </returns>
        public MarketCatalogueRequest(DateTime from, DateTime to)
        {
            var timeRange = new DateRange
            {
                From = from.Date.ToString("s") + "Z",
                To = to.Date.ToString("s") + "Z"
            };

            var filter = new MarketCatalogueFilter
            {
                EventTypeIds = new List<string> { "7" },
                MarketCountries = new List<string> { "GB", "IE" },
                MarketTypeCodes = new List<string> { "WIN" },
                MarketStartTime = timeRange
            };

            var marketProjection = new List<MarketProjection>
            {
                MarketProjection.MARKET_START_TIME,
                MarketProjection.EVENT
            };

            var marketCatalogueParameters = new MarketCatalogueParams()
            {
                Filter = filter,
                MarketProjection = marketProjection
            };

            this.Params = marketCatalogueParameters;
        }

        /// <summary>
        /// Gets the id of the request (always 1).
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; } = 1;

        /// <summary>
        /// Gets the JSON RPC (always 2.0).
        /// </summary>
        [JsonProperty(PropertyName = "jsonrpc")]
        public string Jsonrpc { get; } = "2.0";

        /// <summary>
        /// Gets the method.
        /// </summary>
        [JsonProperty(PropertyName = "method")]
        public string Method { get; } = "SportsAPING/v1.0/listMarketCatalogue";

        /// <summary>
        /// Gets the market catalogue parameters.
        /// Used to define and filter the response.
        /// </summary>
        [JsonProperty(PropertyName = "params")]
        public MarketCatalogueParams Params { get; }
    }
}