using System.Text.Json.Serialization;
using MemoryPack;

namespace Betfair.Benchmarks.Responses;

[MemoryPackable]
public partial class OrderChange
{
    [JsonPropertyName("id")]
    public string? MarketId { get; init; }

    [JsonPropertyName("accountId")]
    public long? AccountId { get; init; }

    [JsonPropertyName("closed")]
    public bool? Closed { get; init; }

    [JsonPropertyName("orc")]
    public List<OrderRunnerChange>? OrderRunnerChanges { get; init; }
}