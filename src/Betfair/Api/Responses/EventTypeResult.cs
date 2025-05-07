namespace Betfair.Api.Responses;

public class EventTypeResult
{
    [JsonPropertyName("eventType")]
    public EventType? EventType { get; init; } = null;

    [JsonPropertyName("marketCount")]
    public int MarketCount { get; init; } = 0;
}
