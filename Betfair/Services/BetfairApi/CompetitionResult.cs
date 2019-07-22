namespace Betfair.Services.BetfairApi
{
    using System.Text;

    using Newtonsoft.Json;

    public class CompetitionResult
    {
        [JsonProperty(PropertyName = "competition")]
        public Competition Competition { get; set; }

        [JsonProperty(PropertyName = "marketCount")]
        public int MarketCount { get; set; }

        public override string ToString()
        {
            return new StringBuilder().AppendFormat("{0}", "CompetitionResult")
                .AppendFormat(" : {0}", this.Competition)
                .AppendFormat(" : MarketCount={0}", this.MarketCount)
                .ToString();
        }
    }
}