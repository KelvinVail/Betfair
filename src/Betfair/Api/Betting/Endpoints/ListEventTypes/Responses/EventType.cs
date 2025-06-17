namespace Betfair.Api.Betting.Endpoints.ListEventTypes.Responses;

/// <summary>
/// BetfairEvent Type.
/// </summary>
public class EventType
{
    /// <summary>
    /// Gets the unique identifier for the event type.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Gets the name of the event type.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
