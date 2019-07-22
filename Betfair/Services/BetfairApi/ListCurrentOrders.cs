namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    public class ListCurrentOrders
    {
        [JsonProperty(PropertyName = "betIds")] public List<string> BetIds = new List<string>();

        [JsonProperty(PropertyName = "customerOrderRefs")] public List<string> CustomerOrderRefs = new List<string>();

        [JsonProperty(PropertyName = "customerStrategyRefs")]
        public List<string> CustomerStrategyRefs = new List<string>();

        [JsonProperty(PropertyName = "dateRange")] public DateRange DateRange = new DateRange();

        [JsonProperty(PropertyName = "fromRecord")] public int? FromRecord;

        [JsonProperty(PropertyName = "marketIds")] public List<string> MarketIds = new List<string>();

        [JsonProperty(PropertyName = "orderBy")] public OrderBy? OrderBy;

        [JsonProperty(PropertyName = "orderProjection")] public OrderProjection? OrderProjection;

        [JsonProperty(PropertyName = "recordCount")] public int? RecordCount;

        [JsonProperty(PropertyName = "sortDir")] public SortDir? SortDir;
    }
}