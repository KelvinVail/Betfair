namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class RunnerId
    {
        [JsonProperty(PropertyName = "marketId")]
        public string MarketId { get; set; }

        [JsonProperty(PropertyName = "selectionId")]
        public long SelectionId { get; set; }

        [JsonProperty(PropertyName = "handicap")]
        public double Handicap { get; set; }
    }
}