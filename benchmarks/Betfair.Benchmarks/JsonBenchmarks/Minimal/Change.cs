namespace Betfair.Benchmarks.JsonBenchmarks.Minimal;

public class Change
{
    [JsonPropertyName("op")]
    public string? Operation { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("clk")]
    public string? Clock { get; init; }

    [JsonPropertyName("pt")]
    public long? PublishTime { get; init; }

    [JsonPropertyName("mc")]
    public List<MarketChangeMin>? MarketChanges { get; init; }
}
