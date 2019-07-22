namespace Betfair.Services.BetfairApi
{
    using System;
    using System.Collections.Generic;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    public class ListMarketBook
    {
        // If you ask for orders, restricts the results to orders with the specified bet IDs. 
        // Omitting this parameter means that all bets will be included in the response. 
        // Please note: A maximum of 250 betId's can be provided at a time.
        [JsonProperty(PropertyName = "betIds")] public List<string> BetIds;

        // A Betfair standard currency code. If not specified, the default currency code is used.
        [JsonProperty(PropertyName = "currencyCode")] public string CurrencyCode;

        // If you ask for orders, restricts the results to orders matching any of the specified set of customer defined strategies. 
        // Also filters which matches by strategy for selections are returned, if partitionMatchedByStrategyRef is true. 
        // An empty set will be treated as if the parameter has been omitted(or null passed).
        [JsonProperty(PropertyName = "customerStrategyRefs")] public List<string> CustomerStrategyRefs;

        // If you ask for orders, returns matches for each selection. Defaults to true if unspecified.
        [JsonProperty(PropertyName = "includeOverallPosition")] public bool? IncludeOverallPosition;

        // The language used for the response. If not specified, the default is returned.
        [JsonProperty(PropertyName = "locale")] public string Locale;

        [JsonProperty(PropertyName = "marketIds")] public List<string> MarketIds = new List<string>();

        // If you ask for orders, restricts the results to orders that have at least one fragment matched since 
        // the specified date(all matched fragments of such an order will be returned even if some were matched before the specified date). 
        // All EXECUTABLE orders will be returned regardless of matched date.
        [JsonProperty(PropertyName = "matchedSince")] public DateTime? MatchedSince;

        [JsonProperty(PropertyName = "matchProjection")] public MatchProjection? MatchProjection;

        [JsonProperty(PropertyName = "orderProjection")] public OrderProjection? OrderProjection;

        // If you ask for orders, returns the breakdown of matches by strategy for each selection. Defaults to false if unspecified.
        [JsonProperty(PropertyName = "partitionMatchedByStrategyRef")] public bool? PartitionMatchedByStrategyRef;

        [JsonProperty(PropertyName = "priceProjection")] public PriceProjection PriceProjection;
    }
}