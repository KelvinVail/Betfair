namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class ClearedOrderResponse
    {
        [JsonProperty(PropertyName = "id")] public int Id;

        [JsonProperty(PropertyName = "jsonrpc")] public string Jsonrpc;

        [JsonProperty(PropertyName = "result")] public ClearedOrderSummaryReport Result;
    }
}