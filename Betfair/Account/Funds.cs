namespace Betfair.Account
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public sealed class Funds : BetfairAccountBase
    {
        public Funds(ISession session)
            : base(session)
        {
        }

        public double AvailableToBetBalance { get; private set; }

        public double Exposure { get; private set; }

        public double RetainedCommission { get; private set; }

        public double ExposureLimit { get; private set; }

        public double DiscountRate { get; private set; }

        public int PointsBalance { get; private set; }

        public new Funds WithHandler(HttpClientHandler handler)
        {
            base.WithHandler(handler);
            return this;
        }

        public async Task RefreshAsync()
        {
            var response = await this.SendAsync<AccountFundsResponse>("getAccountFunds");
            this.AvailableToBetBalance = response.AvailableToBetBalance;
            this.Exposure = response.Exposure;
            this.RetainedCommission = response.RetainedCommission;
            this.ExposureLimit = response.ExposureLimit;
            this.DiscountRate = response.DiscountRate;
            this.PointsBalance = response.PointsBalance;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize SendAsync response.")]
        private sealed class AccountFundsResponse
        {
            [JsonProperty(PropertyName = "availableToBetBalance")]
            internal double AvailableToBetBalance { get; set; }

            [JsonProperty(PropertyName = "exposure")]
            internal double Exposure { get; set; }

            [JsonProperty(PropertyName = "retainedCommission")]
            internal double RetainedCommission { get; set; }

            [JsonProperty(PropertyName = "exposureLimit")]
            internal double ExposureLimit { get; set; }

            [JsonProperty(PropertyName = "discountRate")]
            internal double DiscountRate { get; set; }

            [JsonProperty(PropertyName = "pointsBalance")]
            internal int PointsBalance { get; set; }
        }
    }
}
