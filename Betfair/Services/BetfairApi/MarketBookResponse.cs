namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The market book response.
    /// </summary>
    public class MarketBookResponse
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the json RPC.
        /// </summary>
        [JsonProperty(PropertyName = "jsonrpc")]
        public string Jsonrpc { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public List<MarketBook> Result { get; set; }
    }
}