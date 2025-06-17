using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Enums;

namespace Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Requests;

internal class MarketCatalogueRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter? Filter { get; set; }

    [JsonPropertyName("marketProjection")]
    public List<MarketProjection>? MarketProjection { get; set; }

    [JsonPropertyName("sort")]
    public MarketSort? Sort { get; set; }

    [JsonPropertyName("maxResults")]
    public int MaxResults { get; set; } = 1000;

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }
}
