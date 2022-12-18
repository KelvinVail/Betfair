#nullable enable
using System.Runtime.Serialization;
using Utf8Json;
using Utf8Json.Resolvers;

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
    public string ConnectionId { get; set; }

    [DataMember(Name = "connectionClosed", EmitDefaultValue = false)]
    public bool? ConnectionClosed { get; set; }

    [DataMember(Name = "initialClk", EmitDefaultValue = false)]
    public string InitialClock { get; set; }

    [DataMember(Name = "clk", EmitDefaultValue = false)]
    public string Clock { get; set; }

    [DataMember(Name = "conflateMs", EmitDefaultValue = false)]
    public int? ConflateMs { get; set; }

    [DataMember(Name = "heartbeatMs", EmitDefaultValue = false)]
    public int? HeartbeatMs { get; set; }

    [DataMember(Name = "pt", EmitDefaultValue = false)]
    public long? PublishTime { get; set; }

    [DataMember(Name = "ct", EmitDefaultValue = false)]
    public string ChangeType { get; set; }

    [DataMember(Name = "segmentType", EmitDefaultValue = false)]
    public string SegmentType { get; set; }

    [DataMember(Name = "mc", EmitDefaultValue = false)]
    public List<MarketChange> MarketChanges { get; set; }

    [DataMember(Name = "oc", EmitDefaultValue = false)]
    public List<OrderChange> OrderChanges { get; set; }

    [DataMember(Name = "arrivalTick", EmitDefaultValue = false)]
    public long ArrivalTick { get; init; } = DateTime.UtcNow.Ticks;
}