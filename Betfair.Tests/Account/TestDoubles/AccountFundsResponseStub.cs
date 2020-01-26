namespace Betfair.Tests.Account.TestDoubles
{
    using Newtonsoft.Json;

    public class AccountFundsResponseStub
    {
        [JsonProperty(PropertyName = "availableToBetBalance")]
        public double AvailableToBetBalance { get; set; } = 1000;

        [JsonProperty(PropertyName = "exposure")]
        public double Exposure { get; set; } = 100;

        [JsonProperty(PropertyName = "retainedCommission")]
        public double RetainedCommission { get; set; } = 10;

        [JsonProperty(PropertyName = "exposureLimit")]
        public double ExposureLimit { get; set; } = 10000;

        [JsonProperty(PropertyName = "discountRate")]
        public double DiscountRate { get; set; } = 0.1;

        [JsonProperty(PropertyName = "pointsBalance")]
        public int PointsBalance { get; set; } = 9;
    }
}
