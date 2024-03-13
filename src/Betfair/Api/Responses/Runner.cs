namespace Betfair.Api.Responses;

[JsonSerializable(typeof(Runner))]
public sealed class Runner
{
    [JsonPropertyName("selectionId")]
    public long SelectionId { get; init; }

    [JsonPropertyName("runnerName")]
    public string RunnerName { get; init; } = string.Empty;

    [JsonPropertyName("handicap")]
    public decimal Handicap { get; init; }

    [JsonPropertyName("sortPriority")]
    public int SortPriority { get; init; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; init; }
}
