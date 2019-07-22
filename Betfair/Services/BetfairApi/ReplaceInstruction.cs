namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class ReplaceInstruction
    {
        [JsonProperty(PropertyName = "betId")]
        public string BetId { get; set; }

        [JsonProperty(PropertyName = "newPrice")]
        public double NewPrice { get; set; }
    }
}