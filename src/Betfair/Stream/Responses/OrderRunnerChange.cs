namespace Betfair.Stream.Responses;

/// <summary>
/// Represents matched data for a specific strategy.
/// </summary>
public class StrategyMatchedData
{
    /// <summary>
    /// Gets the matched backs for this strategy.
    /// </summary>
    [JsonPropertyName("mb")]
    public List<List<double>>? MatchedBacks { get; init; }

    /// <summary>
    /// Gets the matched lays for this strategy.
    /// </summary>
    [JsonPropertyName("ml")]
    public List<List<double>>? MatchedLays { get; init; }
}

public class OrderRunnerChange
{
    [JsonPropertyName("mb")]
    public List<List<double>>? MatchedBacks { get; init; }

    [JsonPropertyName("id")]
    public long SelectionId { get; init; }

    [JsonPropertyName("hc")]
    public double? Handicap { get; init; }

    [JsonPropertyName("fullImage")]
    public bool? FullImage { get; init; }

    [JsonPropertyName("ml")]
    public List<List<double>>? MatchedLays { get; init; }

    [JsonPropertyName("uo")]
    public List<UnmatchedOrder>? UnmatchedOrders { get; init; }

    [JsonPropertyName("smc")]
    public Dictionary<string, StrategyMatchedData>? StrategyMatchedChange { get; init; }
}