namespace Betfair.Api.Responses;

public sealed class LadderDescription
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}
