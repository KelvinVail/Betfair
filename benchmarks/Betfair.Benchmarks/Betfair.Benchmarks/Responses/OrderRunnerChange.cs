using System.Text.Json.Serialization;
using MemoryPack;

namespace Betfair.Benchmarks.Responses;

[MemoryPackable]
public partial class OrderRunnerChange
{
    [JsonPropertyName("mb")]
    public List<List<double?>>? MatchedBacks { get; init; }

    [JsonPropertyName("id")]
    public long? SelectionId { get; init; }

    [JsonPropertyName("hc")]
    public double? Handicap { get; init; }

    [JsonPropertyName("fullImage")]
    public bool? FullImage { get; init; }

    [JsonPropertyName("ml")]
    public List<List<double?>>? MatchedLays { get; init; }

    [JsonPropertyName("uo")]
    public List<UnmatchedOrder>? UnmatchedOrders { get; init; }
}