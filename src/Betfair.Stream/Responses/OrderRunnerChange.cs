using System.Runtime.Serialization;

namespace Betfair.Stream.Responses;

[DataContract]
public class OrderRunnerChange
{
    [DataMember(Name = "mb", EmitDefaultValue = false)]
    public List<List<double?>>? MatchedBacks { get; init; }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public long? SelectionId { get; init; }

    [DataMember(Name = "hc", EmitDefaultValue = false)]
    public double? Handicap { get; init; }

    [DataMember(Name = "fullImage", EmitDefaultValue = false)]
    public bool? FullImage { get; init; }

    [DataMember(Name = "ml", EmitDefaultValue = false)]
    public List<List<double?>>? MatchedLays { get; init; }

    [DataMember(Name = "uo", EmitDefaultValue = false)]
    public List<UnmatchedOrder>? UnmatchedOrders { get; init; }
}