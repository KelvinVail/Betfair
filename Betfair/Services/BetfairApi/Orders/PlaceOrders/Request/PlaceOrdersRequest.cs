namespace Betfair.Services.BetfairApi.Orders.PlaceOrders.Request
{
    using Newtonsoft.Json;

    internal sealed class PlaceOrdersRequest
    {
        /// <summary>
        /// The ID.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id => 1;

        [JsonProperty(PropertyName = "jsonrpc")]
        public string Jsonrpc => "2.0";

        [JsonProperty(PropertyName = "method")]
        public string Method => "SportsAPING/v1.0/placeOrders";

        [JsonProperty(PropertyName = "params")]
        public PlaceOrders Params = new PlaceOrders();
    }
}