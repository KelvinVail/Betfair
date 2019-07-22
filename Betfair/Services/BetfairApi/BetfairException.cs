namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class BetfairException
    {
        // exception in json-rpc format
        [JsonProperty(PropertyName = "data")]
        public JObject Data { get; set; } // actual exception details

        // exception in rescript format
        [JsonProperty(PropertyName = "detail")]
        public JObject Detail { get; set; } // actual exception details
    }
}