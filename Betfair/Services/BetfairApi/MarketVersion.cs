namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class MarketVersion
    {
        [JsonProperty(PropertyName = "version")]
        public long Version { get; set; }
    }
}