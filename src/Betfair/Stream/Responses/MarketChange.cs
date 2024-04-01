namespace Betfair.Stream.Responses;

public class MarketChange
{
    [JsonPropertyName("id")]
    [DataMember(Name = "id")]
    public string? MarketId { get; init; }

    [JsonPropertyName("marketDefinition")]
    public MarketDefinition? MarketDefinition { get; init; }

    [JsonPropertyName("rc")]
    [DataMember(Name = "rc")]
    public List<RunnerChange>? RunnerChanges { get; init; }

    [JsonPropertyName("img")]
    [DataMember(Name = "img")]
    public bool? ReplaceCache { get; init; }

    [JsonPropertyName("tv")]
    [DataMember(Name = "tv")]
    public double? TotalAmountMatched { get; init; }

    [JsonPropertyName("con")]
    [DataMember(Name = "con")]
    public bool? Conflated { get; init; }
}