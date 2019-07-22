namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    internal class ReplaceOrdersResponse
    {
        [JsonProperty(PropertyName = "id")] public int Id;

        [JsonProperty(PropertyName = "jsonrpc")] public string Jsonrpc;

        [JsonProperty(PropertyName = "result")] public ReplaceExecutionReport Result;
    }
}