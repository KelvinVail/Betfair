namespace Betfair.Api.Responses;

/// <summary>
/// Information about a runner (selection) in a market.
/// </summary>
public sealed class RunnerResponse
{
    /// <summary>
    /// Gets the unique identifier for the selection.
    /// </summary>
    [JsonPropertyName("selectionId")]
    public long SelectionId { get; init; }

    /// <summary>
    /// Gets the name of the runner.
    /// </summary>
    [JsonPropertyName("runnerName")]
    public string RunnerName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the handicap associated with the runner in case of Asian handicap markets.
    /// </summary>
    [JsonPropertyName("handicap")]
    public decimal Handicap { get; init; }

    /// <summary>
    /// Gets the sort priority of this runner.
    /// </summary>
    [JsonPropertyName("sortPriority")]
    public int SortPriority { get; init; }

    /// <summary>
    /// Gets the metadata associated with the runner.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; init; }
}
