namespace Betfair.Benchmarks.JsonBenchmarks.Minimal;

public class RunnerChangeMin
{
    [JsonPropertyName("batb")]
    public List<List<double?>>? BestAvailableToBack { get; init; }

    [JsonPropertyName("id")]
    public long? SelectionId { get; init; }
}
