namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class MarketProfitAndLoss
    {
        [JsonProperty(PropertyName = "marketId")]
        public string MarketId { get; set; }

        [JsonProperty(PropertyName = "commissionApplied")]
        public double CommissionApplied { get; set; }

        [JsonProperty(PropertyName = "profitAndLosses")]
        public List<RunnerProfitAndLoss> ProfitAndLosses { get; set; }
    }
}