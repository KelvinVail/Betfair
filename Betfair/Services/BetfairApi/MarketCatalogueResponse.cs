namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The market catalogue response.
    /// </summary>
    public class MarketCatalogueResponse
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the jsonRPC.
        /// </summary>
        [JsonProperty(PropertyName = "jsonrpc")]
        public string Jsonrpc { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public List<MarketCatalogue> Result { get; set; }
    }
}