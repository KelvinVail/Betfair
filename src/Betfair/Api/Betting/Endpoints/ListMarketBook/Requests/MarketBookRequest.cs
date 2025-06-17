using Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;
using Betfair.Api.Betting.Enums;

using Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;

namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Requests;

internal class MarketBookRequest
{
    [JsonPropertyName("marketIds")]
    public List<string>? MarketIds { get; set; }

    [JsonPropertyName("priceProjection")]
    public PriceProjection? PriceProjection { get; set; }

    [JsonPropertyName("orderProjection")]
    public OrderProjection? OrderProjection { get; set; }

    [JsonPropertyName("matchProjection")]
    public MatchProjection? MatchProjection { get; set; }

    [JsonPropertyName("includeOverallPosition")]
    public bool? IncludeOverallPosition { get; set; }

    [JsonPropertyName("partitionMatchedByStrategyRef")]
    public bool? PartitionMatchedByStrategyRef { get; set; }

    [JsonPropertyName("customerStrategyRefs")]
    public List<string>? CustomerStrategyRefs { get; set; }

    [JsonPropertyName("currencyCode")]
    public string? CurrencyCode { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("matchedSince")]
    public DateTime? MatchedSince { get; set; }

    [JsonPropertyName("betIds")]
    public List<string>? BetIds { get; set; }
}

