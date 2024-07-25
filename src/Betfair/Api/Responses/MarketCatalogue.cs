namespace Betfair.Api.Responses;

public sealed class MarketCatalogue
{
    [JsonPropertyName("marketId")]
    public string MarketId { get; init; } = string.Empty;

    [JsonPropertyName("marketName")]
    public string MarketName { get; init; } = string.Empty;

    [JsonPropertyName("totalMatched")]
    public decimal TotalMatched { get; init; }

    [JsonPropertyName("marketStartTime")]
    public DateTimeOffset? MarketStartTime { get; init; }

    [JsonPropertyName("competition")]
    public Competition? Competition { get; init; }

    [JsonPropertyName("event")]
    public MarketEvent? Event { get; init; }

    [JsonPropertyName("eventType")]
    public MarketEventType? EventType { get; init; }

    [JsonPropertyName("description")]
    public MarketDescription? Description { get; init; }

    [JsonPropertyName("runners")]
    public IEnumerable<Runner>? Runners { get; init; }
}
