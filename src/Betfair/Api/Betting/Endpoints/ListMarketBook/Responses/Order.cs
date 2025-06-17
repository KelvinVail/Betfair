namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;

/// <summary>
/// Order information.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets the bet ID.
    /// </summary>
    [JsonPropertyName("betId")]
    public string BetId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the BSP Order type.
    /// </summary>
    [JsonPropertyName("orderType")]
    public string OrderType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the order status. Either EXECUTABLE (an unmatched amount remains) or EXECUTION_COMPLETE (no unmatched amount remains).
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets what to do with the order at turn-in-play.
    /// </summary>
    [JsonPropertyName("persistenceType")]
    public string PersistenceType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the side of the bet.
    /// </summary>
    [JsonPropertyName("side")]
    public string Side { get; init; } = string.Empty;

    /// <summary>
    /// Gets the price of the bet.
    /// </summary>
    [JsonPropertyName("price")]
    public double Price { get; init; }

    /// <summary>
    /// Gets the size of the bet.
    /// </summary>
    [JsonPropertyName("size")]
    public double Size { get; init; }

    /// <summary>
    /// Gets the BSP liability. Not to be confused with size. This is the liability of a given BSP bet.
    /// </summary>
    [JsonPropertyName("bspLiability")]
    public double BspLiability { get; init; }

    /// <summary>
    /// Gets the date, to the second, the bet was placed.
    /// </summary>
    [JsonPropertyName("placedDate")]
    public DateTimeOffset PlacedDate { get; init; }

    /// <summary>
    /// Gets the average price matched at. Voided match fragments are removed from this average calculation.
    /// This value is not meaningful for activity on LINE markets and is not guaranteed to be returned or maintained for these markets.
    /// </summary>
    [JsonPropertyName("avgPriceMatched")]
    public double? AvgPriceMatched { get; init; }

    /// <summary>
    /// Gets the current amount of this bet that was matched.
    /// </summary>
    [JsonPropertyName("sizeMatched")]
    public double? SizeMatched { get; init; }

    /// <summary>
    /// Gets the current amount of this bet that is unmatched.
    /// </summary>
    [JsonPropertyName("sizeRemaining")]
    public double? SizeRemaining { get; init; }

    /// <summary>
    /// Gets the current amount of this bet that was lapsed.
    /// </summary>
    [JsonPropertyName("sizeLapsed")]
    public double? SizeLapsed { get; init; }

    /// <summary>
    /// Gets the current amount of this bet that was cancelled.
    /// </summary>
    [JsonPropertyName("sizeCancelled")]
    public double? SizeCancelled { get; init; }

    /// <summary>
    /// Gets the current amount of this bet that was voided.
    /// </summary>
    [JsonPropertyName("sizeVoided")]
    public double? SizeVoided { get; init; }

    /// <summary>
    /// Gets the customer order reference sent for this bet.
    /// </summary>
    [JsonPropertyName("customerOrderRef")]
    public string? CustomerOrderRef { get; init; }

    /// <summary>
    /// Gets the customer strategy reference sent for this bet.
    /// </summary>
    [JsonPropertyName("customerStrategyRef")]
    public string? CustomerStrategyRef { get; init; }
}
