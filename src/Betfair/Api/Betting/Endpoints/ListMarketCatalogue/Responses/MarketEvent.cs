namespace Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Responses;

public sealed class MarketEvent
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("countryCode")]
    public string CountryCode { get; init; } = string.Empty;

    [JsonPropertyName("timezone")]
    public string Timezone { get; init; } = string.Empty;

    [JsonPropertyName("venue")]
    public string Venue { get; init; } = string.Empty;

    [JsonPropertyName("openDate")]
    public DateTimeOffset OpenDate { get; init; }
}
