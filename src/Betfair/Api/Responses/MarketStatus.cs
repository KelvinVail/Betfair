namespace Betfair.Api.Responses;

internal class MarketStatus
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
