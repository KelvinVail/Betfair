namespace Betfair.Services.BetfairApi.Orders.PlaceOrders.Request
{
    using Newtonsoft.Json;

    internal sealed class MarketOnCloseOrder
    {
        [JsonProperty(PropertyName = "liability")]
        public double Liability { get; set; }
    }
}