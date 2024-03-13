namespace Betfair.Stream.Responses;

[DataContract]
public class OrderChange
{
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public string? MarketId { get; init; }

    [DataMember(Name = "accountId", EmitDefaultValue = false)]
    public long? AccountId { get; init; }

    [DataMember(Name = "closed", EmitDefaultValue = false)]
    public bool? Closed { get; init; }

    [DataMember(Name = "orc", EmitDefaultValue = false)]
    public List<OrderRunnerChange>? OrderRunnerChanges { get; init; }
}