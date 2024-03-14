namespace Betfair.Stream.Responses;

[JsonSerializable(typeof(UnmatchedOrder))]
public class UnmatchedOrder
{
    [JsonPropertyName("side")]
    public string? Side { get; init; }

    [JsonPropertyName("pt")]
    public string? PersistenceType { get; init; }

    [JsonPropertyName("ot")]
    public string? OrderType { get; init; }

    [JsonPropertyName("status")]
    public string? OrderStatus { get; init; }

    [JsonPropertyName("sv")]
    public double? SizeVoided { get; init; }

    [JsonPropertyName("p")]
    public double? Price { get; init; }

    [JsonPropertyName("sc")]
    public double? SizeCancelled { get; init; }

    [JsonPropertyName("rc")]
    public string? RegulatorCode { get; init; }

    [JsonPropertyName("s")]
    public double? Size { get; init; }

    [JsonPropertyName("pd")]
    public long? PlacedDate { get; init; }

    [JsonPropertyName("rac")]
    public string? RegulatorAuthCode { get; init; }

    [JsonPropertyName("md")]
    public long? MatchedDate { get; init; }

    [JsonPropertyName("sl")]
    public double? SizeLapsed { get; init; }

    [JsonPropertyName("avp")]
    public double? AveragePriceMatched { get; init; }

    [JsonPropertyName("sm")]
    public double? SizeMatched { get; init; }

    [JsonPropertyName("id")]
    public string? BetId { get; init; }

    [JsonPropertyName("bsp")]
    public double? BspLiability { get; init; }

    [JsonPropertyName("sr")]
    public double? SizeRemaining { get; init; }
}