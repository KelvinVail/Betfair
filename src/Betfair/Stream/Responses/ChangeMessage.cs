namespace Betfair.Stream.Responses;

public sealed class ChangeMessage
{
    [JsonPropertyName("op")]
    [DataMember(Name = "op")]
    public string? Operation { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("statusCode")]
    public string? StatusCode { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("connectionId")]
    public string? ConnectionId { get; set; }

    [JsonPropertyName("connectionClosed")]
    public bool? ConnectionClosed { get; set; }

    [JsonPropertyName("connectionsAvailable")]
    public int? ConnectionsAvailable { get; set; }

    [JsonPropertyName("initialClk")]
    [DataMember(Name = "initialClk")]
    public string? InitialClock { get; set; }

    [JsonPropertyName("clk")]
    [DataMember(Name = "clk")]
    public string? Clock { get; set; }

    [JsonPropertyName("conflateMs")]
    public int? ConflateMs { get; set; }

    [JsonPropertyName("heartbeatMs")]
    public int? HeartbeatMs { get; set; }

    [JsonPropertyName("pt")]
    [DataMember(Name = "pt")]
    public long? PublishTime { get; set; }

    [JsonPropertyName("ct")]
    [DataMember(Name = "ct")]
    public string? ChangeType { get; set; }

    [JsonPropertyName("segmentType")]
    public string? SegmentType { get; set; }

    [JsonPropertyName("mc")]
    [DataMember(Name = "mc")]
    public List<MarketChange>? MarketChanges { get; set; }

    [JsonPropertyName("oc")]
    [DataMember(Name = "oc")]
    public List<OrderChange>? OrderChanges { get; set; }

    [JsonPropertyName("rt")]
    public long ReceivedTick { get; internal set; }

    [JsonPropertyName("dt")]
    public long DeserializedTick { get; internal set; }
}