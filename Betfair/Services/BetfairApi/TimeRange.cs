namespace Betfair.Services.BetfairApi
{
    using System;
    using System.Text;

    using Newtonsoft.Json;

    public class TimeRange
    {
        [JsonProperty(PropertyName = "from")]
        public DateTime From { get; set; }

        [JsonProperty(PropertyName = "to")]
        public DateTime To { get; set; }

        public override string ToString()
        {
            return new StringBuilder().AppendFormat("{0}", "TimeRange")
                .AppendFormat(" : From={0}", this.From)
                .AppendFormat(" : To={0}", this.To)
                .ToString();
        }
    }
}