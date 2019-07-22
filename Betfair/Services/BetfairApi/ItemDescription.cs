namespace Betfair.Services.BetfairApi
{
    using System;

    using Newtonsoft.Json;

    public class ItemDescription
    {
        [JsonProperty(PropertyName = "eventTypeDesc")]
        public string EventTypeDesc { get; set; }

        [JsonProperty(PropertyName = "eventDesc")]
        public string EventDesc { get; set; }

        [JsonProperty(PropertyName = "marketDesc")]
        public string MarketDesc { get; set; }

        [JsonProperty(PropertyName = "marketType")]
        public string MarketType { get; set; }

        [JsonProperty(PropertyName = "marketStartTime")]
        public DateTime MarketStartTime { get; set; }

        [JsonProperty(PropertyName = "runnerDesc")]
        public string RunnerDesc { get; set; }

        [JsonProperty(PropertyName = "numberOfWinners")]
        public int NumberOfWinners { get; set; }

        [JsonProperty(PropertyName = "eachWayDivisor")]
        public double EachWayDivisor { get; set; }
    }
}