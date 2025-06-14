namespace Betfair.Api.Requests;

internal class MarketCatalogueRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter? Filter { get; set; }

    [JsonPropertyName("marketProjection")]
    public List<string>? MarketProjection { get; set; }

    [JsonPropertyName("sort")]
    public string? Sort { get; set; }

    [JsonPropertyName("maxResults")]
    public int MaxResults { get; set; } = 1000;

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }
}
