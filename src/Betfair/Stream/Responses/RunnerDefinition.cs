namespace Betfair.Stream.Responses;

public class RunnerDefinition
{
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("sortPriority")]
    public int? SortPriority { get; init; }

    [JsonPropertyName("removalDate")]
    public DateTime? RemovalDate { get; init; }

    [JsonPropertyName("id")]
    [DataMember(Name = "id")]
    public long? SelectionId { get; init; }

    [JsonPropertyName("hc")]
    [DataMember(Name = "hc")]
    public double? Handicap { get; init; }

    [JsonPropertyName("adjustmentFactor")]
    public double? AdjustmentFactor { get; init; }

    [JsonPropertyName("bsp")]
    [DataMember(Name = "bsp")]
    public double? BspLiability { get; init; }
}