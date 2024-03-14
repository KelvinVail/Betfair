namespace Betfair.Stream.Responses;

public class MarketChange
{
    [JsonPropertyName("id")]
    public string? MarketId { get; init; }

    [JsonPropertyName("marketDefinition")]
    public MarketDefinition? MarketDefinition { get; init; }

    [JsonPropertyName("rc")]
    public List<RunnerChange>? RunnerChanges { get; init; }

    [JsonPropertyName("img")]
    public bool? ReplaceCache { get; init; }

    [JsonPropertyName("tv")]
    public double? TotalAmountMatched { get; init; }

    [JsonPropertyName("con")]
    public bool? Conflated { get; init; }
}