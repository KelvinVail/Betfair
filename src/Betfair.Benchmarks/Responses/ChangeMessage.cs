using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MemoryPack;

namespace Betfair.Benchmarks.Responses;

[MemoryPackable]
public partial class ChangeMessage
{
    [JsonPropertyName("op")]
    [DataMember(Name = "op")]
    public string? Operation { get; init; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("statusCode")]
    public string? StatusCode { get; init; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; init; }

    [JsonPropertyName("connectionId")]
    public string? ConnectionId { get; init; }

    [JsonPropertyName("connectionClosed")]
    public bool? ConnectionClosed { get; init; }

    [JsonPropertyName("connectionsAvailable")]
    public int? ConnectionsAvailable { get; init; }

    [JsonPropertyName("initialClk")]
    [DataMember(Name = "initialClk")]
    public string? InitialClock { get; set; }

    [JsonPropertyName("clk")]
    [DataMember(Name = "clk")]
    public string? Clock { get; init; }

    [JsonPropertyName("conflateMs")]
    public int? ConflateMs { get; init; }

    [JsonPropertyName("heartbeatMs")]
    public int? HeartbeatMs { get; init; }

    [JsonPropertyName("pt")]
    [DataMember(Name = "pt")]
    public long? PublishTime { get; init; }

    [JsonPropertyName("ct")]
    [DataMember(Name = "ct")]
    public string? ChangeType { get; init; }

    [JsonPropertyName("segmentType")]
    public string? SegmentType { get; init; }

    [JsonPropertyName("mc")]
    [DataMember(Name = "mc")]
    public List<MarketChange>? MarketChanges { get; init; }

    [JsonPropertyName("oc")]
    [DataMember(Name = "oc")]
    public List<OrderChange>? OrderChanges { get; init; }

    [JsonPropertyName("rt")]
    public long ReceivedTick { get; internal set; }

    [JsonPropertyName("dt")]
    public long DeserializedTick { get; internal set; }
}