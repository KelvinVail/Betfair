namespace Betfair.Services.BetfairApi.Orders.PlaceOrders.Response
{
    using Newtonsoft.Json;

    internal sealed class PlaceOrdersResponse
    {
        [JsonProperty(PropertyName = "id")] public int Id;

        [JsonProperty(PropertyName = "jsonrpc")] public string Jsonrpc;

        [JsonProperty(PropertyName = "result")] public PlaceExecutionReport Result;
    }
}