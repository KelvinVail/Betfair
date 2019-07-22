namespace Betfair.Services.BetfairApi
{
    using System;
    using System.Collections.Generic;

    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    /// <summary>
    /// The market book parameters.
    /// </summary>
    public class MarketBookParams
    {
        /// <summary>
        /// Gets or sets the bet ids.
        /// If you ask for orders, restricts the results to orders with the specified bet IDs. 
        /// Omitting this parameter means that all bets will be included in the response. 
        /// Please note: A maximum of 250 betId's can be provided at a time.
        /// </summary>
        [JsonProperty(PropertyName = "betIds")]
        public List<string> BetIds { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// A standard currency code. If not specified, the default currency code is used.
        /// </summary>
        [JsonProperty(PropertyName = "currencyCode")]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the customer strategy refs.
        /// If you ask for orders, restricts the results to orders matching any of the specified set of customer defined strategies. 
        /// Also filters which matches by strategy for selections are returned, if partitionMatchedByStrategyRef is true. 
        /// An empty set will be treated as if the parameter has been omitted(or null passed).
        /// </summary>
        [JsonProperty(PropertyName = "customerStrategyRefs")]
        public List<string> CustomerStrategyRefs { get; set; }

        /// <summary>
        /// Gets or sets the include overall position.
        /// If you ask for orders, returns matches for each selection. Defaults to true if unspecified.
        /// </summary>
        [JsonProperty(PropertyName = "includeOverallPosition")]
        public bool? IncludeOverallPosition { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// The language used for the response. If not specified, the default is returned.
        /// </summary>
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the market ids.
        /// </summary>
        [JsonProperty(PropertyName = "marketIds")]
        public List<string> MarketIds { get; set; }

        /// <summary>
        /// Gets or sets the matched since.
        /// If you ask for orders, restricts the results to orders that have at least one fragment matched since 
        /// the specified date(all matched fragments of such an order will be returned even if some were matched before the specified date). 
        /// All EXECUTABLE orders will be returned regardless of matched date.
        /// </summary>
        [JsonProperty(PropertyName = "matchedSince")]
        public DateTime? MatchedSince { get; set; }

        /// <summary>
        /// Gets or sets the match projection.
        /// </summary>
        [JsonProperty(PropertyName = "matchProjection")]
        public MatchProjection? MatchProjection { get; set; }

        /// <summary>
        /// Gets or sets the order projection.
        /// </summary>
        [JsonProperty(PropertyName = "orderProjection")]
        public OrderProjection? OrderProjection { get; set; }

        /// <summary>
        /// Gets or sets the partition matched by strategy ref.
        /// If you ask for orders, returns the breakdown of matches by strategy for each selection. Defaults to false if unspecified.
        /// </summary>
        [JsonProperty(PropertyName = "partitionMatchedByStrategyRef")]
        public bool? PartitionMatchedByStrategyRef { get; set; }

        /// <summary>
        /// Gets or sets the price projection.
        /// </summary>
        [JsonProperty(PropertyName = "priceProjection")]
        public PriceProjection PriceProjection { get; set; }
    }
}