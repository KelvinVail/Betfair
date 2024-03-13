namespace Betfair.Stream.Responses;

[DataContract]
public class UnmatchedOrder
{
    [DataMember(Name = "side", EmitDefaultValue = false)]
    public string? Side { get; init; }

    [DataMember(Name = "pt", EmitDefaultValue = false)]
    public string? PersistenceType { get; init; }

    [DataMember(Name = "ot", EmitDefaultValue = false)]
    public string? OrderType { get; init; }

    [DataMember(Name = "status", EmitDefaultValue = false)]
    public string? OrderStatus { get; init; }

    [DataMember(Name = "sv", EmitDefaultValue = false)]
    public double? SizeVoided { get; init; }

    [DataMember(Name = "p", EmitDefaultValue = false)]
    public double? Price { get; init; }

    [DataMember(Name = "sc", EmitDefaultValue = false)]
    public double? SizeCancelled { get; init; }

    [DataMember(Name = "rc", EmitDefaultValue = false)]
    public string? RegulatorCode { get; init; }

    [DataMember(Name = "s", EmitDefaultValue = false)]
    public double? Size { get; init; }

    [DataMember(Name = "pd", EmitDefaultValue = false)]
    public long? PlacedDate { get; init; }

    [DataMember(Name = "rac", EmitDefaultValue = false)]
    public string? RegulatorAuthCode { get; init; }

    [DataMember(Name = "md", EmitDefaultValue = false)]
    public long? MatchedDate { get; init; }

    [DataMember(Name = "sl", EmitDefaultValue = false)]
    public double? SizeLapsed { get; init; }

    [DataMember(Name = "avp", EmitDefaultValue = false)]
    public double? AveragePriceMatched { get; init; }

    [DataMember(Name = "sm", EmitDefaultValue = false)]
    public double? SizeMatched { get; init; }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public string? BetId { get; init; }

    [DataMember(Name = "bsp", EmitDefaultValue = false)]
    public double? BspLiability { get; init; }

    [DataMember(Name = "sr", EmitDefaultValue = false)]
    public double? SizeRemaining { get; init; }
}