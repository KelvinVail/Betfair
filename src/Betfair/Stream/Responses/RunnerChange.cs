namespace Betfair.Stream.Responses;

[DataContract]
public class RunnerChange
{
    [DataMember(Name = "tv", EmitDefaultValue = false)]
    public double? TotalMatched { get; init; }

    [DataMember(Name = "batb", EmitDefaultValue = false)]
    public List<List<double?>>? BestAvailableToBack { get; init; }

    [DataMember(Name = "spb", EmitDefaultValue = false)]
    public List<List<double?>>? StartingPriceBack { get; init; }

    [DataMember(Name = "bdatl", EmitDefaultValue = false)]
    public List<List<double?>>? BestDisplayAvailableToLay { get; init; }

    [DataMember(Name = "trd", EmitDefaultValue = false)]
    public List<List<double?>>? Traded { get; init; }

    [DataMember(Name = "spf", EmitDefaultValue = false)]
    public double? StartingPriceFar { get; init; }

    [DataMember(Name = "ltp", EmitDefaultValue = false)]
    public double? LastTradedPrice { get; init; }

    [DataMember(Name = "atb", EmitDefaultValue = false)]
    public List<List<double?>>? AvailableToBack { get; init; }

    [DataMember(Name = "spl", EmitDefaultValue = false)]
    public List<List<double?>>? StartingPriceLay { get; init; }

    [DataMember(Name = "spn", EmitDefaultValue = false)]
    public double? StartingPriceNear { get; init; }

    [DataMember(Name = "atl", EmitDefaultValue = false)]
    public List<List<double?>>? AvailableToLay { get; init; }

    [DataMember(Name = "batl", EmitDefaultValue = false)]
    public List<List<double?>>? BestAvailableToLay { get; init; }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public long? SelectionId { get; init; }

    [DataMember(Name = "hc", EmitDefaultValue = false)]
    public double? Handicap { get; init; }

    [DataMember(Name = "bdatb", EmitDefaultValue = false)]
    public List<List<double?>>? BestDisplayAvailableToBack { get; init; }
}