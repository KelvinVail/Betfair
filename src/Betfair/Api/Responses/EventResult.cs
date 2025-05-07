namespace Betfair.Api.Responses;

public class EventResult
{
    [JsonPropertyName("event")]
    public EventType? Event { get; init; } = null;

    [JsonPropertyName("marketCount")]
    public int MarketCount { get; init; } = 0;
}
