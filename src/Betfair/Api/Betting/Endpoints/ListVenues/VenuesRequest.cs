namespace Betfair.Api.Betting.Endpoints.ListVenues;

internal class VenuesRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new();

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }
}
