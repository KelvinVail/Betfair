namespace Betfair.Services.BetfairApi
{
    using System.Text;

    using Newtonsoft.Json;

    public class EventResult
    {
        [JsonProperty(PropertyName = "event")]
        public Event Event { get; set; }

        [JsonProperty(PropertyName = "marketCount")]
        public int MarketCount { get; set; }

        public override string ToString()
        {
            return new StringBuilder().AppendFormat("{0}", "EventResult")
                .AppendFormat(" : {0}", this.Event)
                .AppendFormat(" : MarketCount={0}", this.MarketCount)
                .ToString();
        }
    }
}