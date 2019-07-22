namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ListClearedOrders
    {
        [JsonProperty(PropertyName = "betStatus")] public BetStatus BetStatus;

        [JsonProperty(PropertyName = "fromRecord")] public int FromRecord;

        [JsonProperty(PropertyName = "marketIds")] public List<string> MarketIds = new List<string>();

        [JsonProperty(PropertyName = "recordCount")] public int RecordCount;
    }
}