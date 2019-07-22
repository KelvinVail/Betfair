namespace Betfair.Services.BetfairApi.Orders.PlaceOrders.Request
{
    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    internal  sealed class LimitOrder
    {
        [JsonProperty(PropertyName = "size")]
        public double Size { get; set; }

        [JsonProperty(PropertyName = "price")]
        public double Price { get; set; }

        [JsonProperty(PropertyName = "persistenceType")]
        public PersistenceType PersistenceType { get; set; }

        [JsonProperty(PropertyName = "timeInForce")]
        public TimeInForce? TimeInForce { get; set; }

        [JsonProperty(PropertyName = "minFillSize")]
        public double? MinFillSize { get; set; }

        [JsonProperty(PropertyName = "betTargetType")]
        public BetTargetType? BetTargetType { get; set; }

        [JsonProperty(PropertyName = "betTargetSize")]
        public double? BetTargetSize { get; set; }
    }
}