namespace Betfair.Api.Betting.Endpoints.ListMarketTypes.Requests;

internal class MarketTypesRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new ();

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }
}
