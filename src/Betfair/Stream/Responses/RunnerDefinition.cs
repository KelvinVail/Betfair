namespace Betfair.Stream.Responses;

[DataContract]
public class RunnerDefinition
{
    [DataMember(Name = "status", EmitDefaultValue = false)]
    public string? Status { get; init; }

    [DataMember(Name = "sortPriority", EmitDefaultValue = false)]
    public int? SortPriority { get; init; }

    [DataMember(Name = "removalDate", EmitDefaultValue = false)]
    public DateTime? RemovalDate { get; init; }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public long? SelectionId { get; init; }

    [DataMember(Name = "hc", EmitDefaultValue = false)]
    public double? Handicap { get; init; }

    [DataMember(Name = "adjustmentFactor", EmitDefaultValue = false)]
    public double? AdjustmentFactor { get; init; }

    [DataMember(Name = "bsp", EmitDefaultValue = false)]
    public double? BspLiability { get; init; }
}