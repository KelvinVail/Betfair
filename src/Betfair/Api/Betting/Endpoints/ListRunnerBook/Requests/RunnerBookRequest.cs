using Betfair.Api.Betting.Endpoints.ListMarketBook;

using Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;

namespace Betfair.Api.Betting.Endpoints.ListRunnerBook.Requests;

internal class RunnerBookRequest
{
    [JsonPropertyName("marketId")]
    public string? MarketId { get; set; }

    [JsonPropertyName("selectionId")]
    public long SelectionId { get; set; }

    [JsonPropertyName("handicap")]
    public double? Handicap { get; set; }

    [JsonPropertyName("priceProjection")]
    public PriceProjection? PriceProjection { get; set; }

    [JsonPropertyName("orderProjection")]
    public string? OrderProjection { get; set; }

    [JsonPropertyName("matchProjection")]
    public string? MatchProjection { get; set; }

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

