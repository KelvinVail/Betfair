namespace Betfair.Stream.Responses;

[JsonSerializable(typeof(RunnerChange))]
public class RunnerChange
{
    [JsonPropertyName("tv")]
    public double? TotalMatched { get; init; }

    [JsonPropertyName("batb")]
    public List<List<double?>>? BestAvailableToBack { get; init; }

    [JsonPropertyName("spb")]
    public List<List<double?>>? StartingPriceBack { get; init; }

    [JsonPropertyName("bdatl")]
    public List<List<double?>>? BestDisplayAvailableToLay { get; init; }

    [JsonPropertyName("trd")]
    public List<List<double?>>? Traded { get; init; }

    [JsonPropertyName("spf")]
    public double? StartingPriceFar { get; init; }

    [JsonPropertyName("ltp")]
    public double? LastTradedPrice { get; init; }

    [JsonPropertyName("atb")]
    public List<List<double?>>? AvailableToBack { get; init; }

    [JsonPropertyName("spl")]
    public List<List<double?>>? StartingPriceLay { get; init; }

    [JsonPropertyName("spn")]
    public double? StartingPriceNear { get; init; }

    [JsonPropertyName("atl")]
    public List<List<double?>>? AvailableToLay { get; init; }

    [JsonPropertyName("batl")]
    public List<List<double?>>? BestAvailableToLay { get; init; }

    [JsonPropertyName("id")]
    public long? SelectionId { get; init; }

    [JsonPropertyName("hc")]
    public double? Handicap { get; init; }

    [JsonPropertyName("bdatb")]
    public List<List<double?>>? BestDisplayAvailableToBack { get; init; }
}