namespace Betfair.Stream.Responses;

public class RunnerChange
{
    [JsonPropertyName("tv")]
    [DataMember(Name = "tv")]
    public double? TotalMatched { get; set; }

    [JsonPropertyName("batb")]
    [DataMember(Name = "batb")]
    public List<List<double>>? BestAvailableToBack { get; set; }

    [JsonPropertyName("spb")]
    [DataMember(Name = "spb")]
    public List<List<double>>? StartingPriceBack { get; set; }

    [JsonPropertyName("bdatl")]
    [DataMember(Name = "bdatl")]
    public List<List<double>>? BestDisplayAvailableToLay { get; set; }

    [JsonPropertyName("trd")]
    [DataMember(Name = "trd")]
    public List<List<double>>? Traded { get; set; }

    [JsonPropertyName("spf")]
    [DataMember(Name = "spf")]
    public double? StartingPriceFar { get; set; }

    [JsonPropertyName("ltp")]
    [DataMember(Name = "ltp")]
    public double? LastTradedPrice { get; set; }

    [JsonPropertyName("atb")]
    [DataMember(Name = "atb")]
    public List<List<double>>? AvailableToBack { get; set; }

    [JsonPropertyName("spl")]
    [DataMember(Name = "spl")]
    public List<List<double>>? StartingPriceLay { get; set; }

    [JsonPropertyName("spn")]
    [DataMember(Name = "spn")]
    public double? StartingPriceNear { get; set; }

    [JsonPropertyName("atl")]
    [DataMember(Name = "atl")]
    public List<List<double>>? AvailableToLay { get; set; }

    [JsonPropertyName("batl")]
    [DataMember(Name = "batl")]
    public List<List<double>>? BestAvailableToLay { get; set; }

    [JsonPropertyName("id")]
    [DataMember(Name = "id")]
    public long? SelectionId { get; set; }

    [JsonPropertyName("hc")]
    [DataMember(Name = "hc")]
    public double? Handicap { get; set; }

    [JsonPropertyName("bdatb")]
    [DataMember(Name = "bdatb")]
    public List<List<double>>? BestDisplayAvailableToBack { get; set; }
}