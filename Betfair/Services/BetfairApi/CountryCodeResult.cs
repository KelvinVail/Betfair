namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class CountryCodeResult
    {
        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "marketCount")]
        public int MarketCount { get; set; }
    }
}