namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class AccountFunds
    {
        [JsonProperty(PropertyName = "availableToBetBalance")]
        public double AvailableToBetBalance { get; set; }

        [JsonProperty(PropertyName = "exposure")]
        public double Exposure { get; set; }

        [JsonProperty(PropertyName = "retainedCommission")]
        public double RetainedCommission { get; set; }

        [JsonProperty(PropertyName = "exposureLimit")]
        public double ExposureLimit { get; set; }

        [JsonProperty(PropertyName = "discountRate")]
        public double DiscountRate { get; set; }

        [JsonProperty(PropertyName = "pointsBalance")]
        public double PointsBalance { get; set; }
    }
}