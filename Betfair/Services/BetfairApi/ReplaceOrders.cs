namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ReplaceOrders
    {
        // If true places orders without waiting for a response
        [JsonProperty(PropertyName = "async")] public bool? Async;

        // Used to de-dup
        // 32 char limit
        [JsonProperty(PropertyName = "customerRef")] public string CustomerRef;

        [JsonProperty(PropertyName = "instructions")]
        public List<ReplaceInstruction> Instructions = new List<ReplaceInstruction>();

        [JsonProperty(PropertyName = "marketId")] public string MarketId;

        // Used to avoid placing bets into an unintended version 
        // i.e. any suspension (runner removal) will increment the version
        [JsonProperty(PropertyName = "marketVersion")] public long? MarketVersion;
    }
}