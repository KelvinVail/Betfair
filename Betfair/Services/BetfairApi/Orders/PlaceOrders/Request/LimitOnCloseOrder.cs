namespace Betfair.Services.BetfairApi.Orders.PlaceOrders.Request
{
    using Newtonsoft.Json;

    internal sealed class LimitOnCloseOrder
    {
        [JsonProperty(PropertyName = "size")]
        public double Size { get; set; }

        [JsonProperty(PropertyName = "liability")]
        public double Liability { get; set; }
    }
}