namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class CurrentOrderRequest
    {
        [JsonProperty(PropertyName = "id")] public int Id = 1;

        [JsonProperty(PropertyName = "jsonrpc")] public string Jsonrpc = "2.0";

        [JsonProperty(PropertyName = "method")] public string Method = "SportsAPING/v1.0/listCurrentOrders";

        [JsonProperty(PropertyName = "params")] public ListCurrentOrders Params = new ListCurrentOrders();
    }
}