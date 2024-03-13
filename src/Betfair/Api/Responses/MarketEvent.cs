namespace Betfair.Api.Responses;

[JsonSerializable(typeof(MarketEvent))]
public sealed class MarketEvent
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("timezone")]
    public string Timezone { get; init; } = string.Empty;

    [JsonPropertyName("openDate")]
    public DateTimeOffset OpenDate { get; init; }
}
