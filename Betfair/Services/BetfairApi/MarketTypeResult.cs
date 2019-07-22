namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class MarketTypeResult
    {
        [JsonProperty(PropertyName = "marketType")]
        public string MarketType { get; set; }

        [JsonProperty(PropertyName = "marketCount")]
        public int MarketCount { get; set; }
    }
}