namespace Betfair.Stream.Responses;

[DataContract]
public class MarketChange
{
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public string? MarketId { get; init; }

    [DataMember(Name = "marketDefinition", EmitDefaultValue = false)]
    public MarketDefinition? MarketDefinition { get; init; }

    [DataMember(Name = "rc", EmitDefaultValue = false)]
    public List<RunnerChange>? RunnerChanges { get; init; }

    [DataMember(Name = "img", EmitDefaultValue = false)]
    public bool? ReplaceCache { get; init; }

    [DataMember(Name = "tv", EmitDefaultValue = false)]
    public double? TotalAmountMatched { get; init; }

    [DataMember(Name = "con", EmitDefaultValue = false)]
    public bool? Conflated { get; init; }
}