namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class ReplaceOrdersRequest
    {
        [JsonProperty(PropertyName = "id")] public int Id = 1;

        [JsonProperty(PropertyName = "jsonrpc")] public string Jsonrpc = "2.0";

        [JsonProperty(PropertyName = "method")] public string Method = "SportsAPING/v1.0/replaceOrders";

        [JsonProperty(PropertyName = "params")] public ReplaceOrders Params = new ReplaceOrders();
    }
}