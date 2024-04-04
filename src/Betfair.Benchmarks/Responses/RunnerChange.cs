using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MemoryPack;

namespace Betfair.Benchmarks.Responses;

[MemoryPackable]
public partial class RunnerChange
{
    [JsonPropertyName("tv")]
    [DataMember(Name = "tv")]
    public double? TotalMatched { get; init; }

    [JsonPropertyName("batb")]
    [DataMember(Name = "batb")]
    public List<List<double?>>? BestAvailableToBack { get; init; }

    [JsonPropertyName("spb")]
    [DataMember(Name = "spb")]
    public List<List<double?>>? StartingPriceBack { get; init; }

    [JsonPropertyName("bdatl")]
    [DataMember(Name = "bdatl")]
    public List<List<double?>>? BestDisplayAvailableToLay { get; init; }

    [JsonPropertyName("trd")]
    [DataMember(Name = "trd")]
    public List<List<double?>>? Traded { get; init; }

    [JsonPropertyName("spf")]
    [DataMember(Name = "spf")]
    public double? StartingPriceFar { get; init; }

    [JsonPropertyName("ltp")]
    [DataMember(Name = "ltp")]
    public double? LastTradedPrice { get; init; }

    [JsonPropertyName("atb")]
    [DataMember(Name = "atb")]
    public List<List<double?>>? AvailableToBack { get; init; }

    [JsonPropertyName("spl")]
    [DataMember(Name = "spl")]
    public List<List<double?>>? StartingPriceLay { get; init; }

    [JsonPropertyName("spn")]
    [DataMember(Name = "spn")]
    public double? StartingPriceNear { get; init; }

    [JsonPropertyName("atl")]
    [DataMember(Name = "atl")]
    public List<List<double?>>? AvailableToLay { get; init; }

    [JsonPropertyName("batl")]
    [DataMember(Name = "batl")]
    public List<List<double?>>? BestAvailableToLay { get; init; }

    [JsonPropertyName("id")]
    [DataMember(Name = "id")]
    public long? SelectionId { get; init; }

    [JsonPropertyName("hc")]
    [DataMember(Name = "hc")]
    public double? Handicap { get; init; }

    [JsonPropertyName("bdatb")]
    [DataMember(Name = "bdatb")]
    public List<List<double?>>? BestDisplayAvailableToBack { get; init; }
}