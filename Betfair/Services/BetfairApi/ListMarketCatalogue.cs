namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    public class ListMarketCatalogue
    {
        [JsonProperty(PropertyName = "filter")] public MarketCatalogueFilter Filter = new MarketCatalogueFilter();

        [JsonProperty(PropertyName = "marketProjection")]
        public List<MarketProjection> MarketProjection = new List<MarketProjection>();

        [JsonProperty(PropertyName = "maxResults")] public string MaxResults = "100";

        [JsonProperty(PropertyName = "sort")] public MarketSort Sort = MarketSort.FIRST_TO_START;
    }
}