namespace Betfair.Api.Responses;

[JsonSerializable(typeof(LadderDescription))]
public sealed class LadderDescription
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}
