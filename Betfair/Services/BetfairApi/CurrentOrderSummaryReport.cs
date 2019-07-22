namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class CurrentOrderSummaryReport
    {
        [JsonProperty(PropertyName = "currentOrders")]
        public IList<CurrentOrderSummary> CurrentOrders { get; set; }

        [JsonProperty(PropertyName = "moreAvailable")]
        public bool MoreAvailable { get; set; }
    }
}