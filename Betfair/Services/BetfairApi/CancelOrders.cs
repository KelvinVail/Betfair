namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class CancelOrders
    {
        // Used to de-dup
        // 32 char limit
        [JsonProperty(PropertyName = "customerRef")] public string CustomerRef;

        [JsonProperty(PropertyName = "instructions")]
        public List<CancelInstruction> Instructions = new List<CancelInstruction>();

        [JsonProperty(PropertyName = "marketId")] public string MarketId;
    }
}