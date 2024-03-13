namespace Betfair.Api.Responses;

[JsonSerializable(typeof(MarketStatus))]
internal class MarketStatus
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
