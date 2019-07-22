namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class ClearedOrderRequest
    {
        [JsonProperty(PropertyName = "id")] public int Id = 1;

        [JsonProperty(PropertyName = "jsonrpc")] public string Jsonrpc = "2.0";

        [JsonProperty(PropertyName = "method")] public string Method = "SportsAPING/v1.0/listClearedOrders";

        [JsonProperty(PropertyName = "params")] public ListClearedOrders Params = new ListClearedOrders();
    }
}