using Betfair.Api.Betting.Endpoints.ListEventTypes;

using Betfair.Api.Betting.Endpoints.ListEventTypes.Responses;

namespace Betfair.Api.Betting.Endpoints.ListEvents.Responses;

/// <summary>
/// BetfairEvent Result.
/// </summary>
public class EventResult
{
    /// <summary>
    /// Gets the event.
    /// </summary>
    [JsonPropertyName("event")]
    public EventType? Event { get; init; } = null;

    /// <summary>
    /// Gets the count of markets associated with this BetfairEvent.
    /// </summary>
    [JsonPropertyName("marketCount")]
    public int MarketCount { get; init; } = 0;
}
