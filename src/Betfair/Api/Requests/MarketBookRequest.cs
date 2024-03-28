namespace Betfair.Api.Requests;

internal class MarketBookRequest
{
    [JsonPropertyName("marketIds")]
    public List<string>? MarketIds { get; set; }
}
