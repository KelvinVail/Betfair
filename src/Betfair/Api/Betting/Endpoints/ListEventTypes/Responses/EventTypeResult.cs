namespace Betfair.Api.Betting.Endpoints.ListEventTypes.Responses;

/// <summary>
/// BetfairEvent Type Result.
/// </summary>
public class EventTypeResult
{
    /// <summary>
    /// Gets the event type.
    /// </summary>
    [JsonPropertyName("eventType")]
    public EventType? EventType { get; init; } = null;

    /// <summary>
    /// Gets the count of markets associated with this BetfairEvent Type.
    /// </summary>
    [JsonPropertyName("marketCount")]
    public int MarketCount { get; init; } = 0;
}
