using System.Runtime.Serialization;

namespace Betfair.Stream.Responses;

[DataContract]
public sealed class ChangeMessage
{
    [DataMember(Name = "op", EmitDefaultValue = false)]
    public string Operation { get; init; } = string.Empty;

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public int Id { get; init; }

    [DataMember(Name = "statusCode", EmitDefaultValue = false)]
    public string? StatusCode { get; init; }

    [DataMember(Name = "errorCode", EmitDefaultValue = false)]
    public string? ErrorCode { get; init; }

    [DataMember(Name = "connectionId", EmitDefaultValue = false)]
    public string? ConnectionId { get; init; }

    [DataMember(Name = "connectionClosed", EmitDefaultValue = false)]
    public bool? ConnectionClosed { get; init; }

    [DataMember(Name = "initialClk", EmitDefaultValue = false)]
    public string? InitialClock { get; init; }

    [DataMember(Name = "clk", EmitDefaultValue = false)]
    public string? Clock { get; init; }

    [DataMember(Name = "conflateMs", EmitDefaultValue = false)]
    public int? ConflateMs { get; init; }

    [DataMember(Name = "heartbeatMs", EmitDefaultValue = false)]
    public int? HeartbeatMs { get; init; }

    [DataMember(Name = "pt", EmitDefaultValue = false)]
    public long? PublishTime { get; init; }

    [DataMember(Name = "ct", EmitDefaultValue = false)]
    public string? ChangeType { get; init; }

    [DataMember(Name = "segmentType", EmitDefaultValue = false)]
    public string? SegmentType { get; init; }

    [DataMember(Name = "mc", EmitDefaultValue = false)]
    public List<MarketChange>? MarketChanges { get; init; }

    [DataMember(Name = "oc", EmitDefaultValue = false)]
    public List<OrderChange>? OrderChanges { get; init; }

    [DataMember(Name = "rt", EmitDefaultValue = false)]
    public long ReceivedTick { get; internal set; }

    [DataMember(Name = "dt", EmitDefaultValue = false)]
    public long DeserializedTick { get; internal set; }
}