namespace Betfair.Benchmarks.JsonBenchmarks.Minimal;

public class MarketChangeMin
{
    [JsonPropertyName("id")]
    public string? MarketId { get; init; }
}
