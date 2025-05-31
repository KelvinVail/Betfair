namespace Betfair.Stream.Responses;

public class MarketChange
{
    [JsonPropertyName("id")]
    [DataMember(Name = "id")]
    public string? MarketId { get; set; }

    [JsonPropertyName("marketDefinition")]
    public MarketDefinition? MarketDefinition { get; set; }

    [JsonPropertyName("rc")]
    [DataMember(Name = "rc")]
    public List<RunnerChange>? RunnerChanges { get; set; }

    [JsonPropertyName("img")]
    [DataMember(Name = "img")]
    public bool? ReplaceCache { get; set; }

    [JsonPropertyName("tv")]
    [DataMember(Name = "tv")]
    public double? TotalAmountMatched { get; set; }

    [JsonPropertyName("con")]
    [DataMember(Name = "con")]
    public bool? Conflated { get; set; }
}