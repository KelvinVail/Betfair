namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ListMarketProfitAndLoss
    {
        [JsonProperty(PropertyName = "includeBspBets")] public bool IncludeBspBets;

        [JsonProperty(PropertyName = "includeSettledBets")] public bool IncludeSettledBets;

        [JsonProperty(PropertyName = "marketIds")] public List<string> MarketIds = new List<string>();

        [JsonProperty(PropertyName = "netOfCommission")] public bool NetOfCommission;
    }
}