namespace Betfair.Api.Responses;

/// <summary>
/// Event.
/// </summary>
public class Event
{
    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the event.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the ISO-2 code for the event country.
    /// </summary>
    [JsonPropertyName("countryCode")]
    public string CountryCode { get; init; } = string.Empty;

    /// <summary>
    /// Gets the timezone in which the event is taking place.
    /// </summary>
    [JsonPropertyName("timezone")]
    public string Timezone { get; init; } = string.Empty;

    /// <summary>
    /// Gets the venue where the event is taking place.
    /// </summary>
    [JsonPropertyName("venue")]
    public string Venue { get; init; } = string.Empty;

    /// <summary>
    /// Gets the scheduled start date and time of the event.
    /// </summary>
    [JsonPropertyName("openDate")]
    public DateTimeOffset OpenDate { get; init; } = default;
}
