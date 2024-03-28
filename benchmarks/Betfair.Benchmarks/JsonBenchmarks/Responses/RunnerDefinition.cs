namespace Betfair.Benchmarks.JsonBenchmarks.Responses;

public struct RunnerDefinition
{
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("sortPriority")]
    public int? SortPriority { get; init; }

    [JsonPropertyName("removalDate")]
    public DateTime? RemovalDate { get; init; }

    [JsonPropertyName("id")]
    public long? SelectionId { get; init; }

    [JsonPropertyName("hc")]
    public double? Handicap { get; init; }

    [JsonPropertyName("adjustmentFactor")]
    public double? AdjustmentFactor { get; init; }

    [JsonPropertyName("bsp")]
    public double? BspLiability { get; init; }
}