namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class TimeRangeResult
    {
        [JsonProperty(PropertyName = "timeRange")]
        public TimeRange TimeRange { get; set; }

        [JsonProperty(PropertyName = "marketCount")]
        public int MarketCount { get; set; }
    }
}