namespace Betfair.Stream.Responses;

public class UnmatchedOrder
{
    /// <summary>
    /// Gets the id of the order.
    /// </summary>
    [JsonPropertyName("id")]
    public string BetId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the original placed price of the order.
    /// </summary>
    [JsonPropertyName("p")]
    public double? Price { get; init; }

    /// <summary>
    /// Gets the original placed size of the order.
    /// </summary>
    [JsonPropertyName("s")]
    public double? Size { get; init; }

    /// <summary>
    /// Gets the BSP liability of the order (null if the order is not a BSP order).
    /// </summary>
    [JsonPropertyName("bsp")]
    public double? BspLiability { get; init; }

    /// <summary>
    /// Gets the side of the order.
    /// </summary>
    [JsonPropertyName("side")]
    public string? Side { get; init; }

    /// <summary>
    /// Gets the status of the order (E = EXECUTABLE, EC = EXECUTION_COMPLETE).
    /// </summary>
    [JsonPropertyName("status")]
    public string? OrderStatus { get; init; }

    /// <summary>
    /// Gets whether the order will persist at in play or not (L = LAPSE, P = PERSIST, MOC = Market On Close).
    /// </summary>
    [JsonPropertyName("pt")]
    public string? PersistenceType { get; init; }

    /// <summary>
    /// Gets the type of the order (L = LIMIT, MOC = MARKET_ON_CLOSE, LOC = LIMIT_ON_CLOSE).
    /// </summary>
    [JsonPropertyName("ot")]
    public string? OrderType { get; init; }

    /// <summary>
    /// Gets the date the order was placed.
    /// </summary>
    [JsonPropertyName("pd")]
    public long? PlacedDate { get; init; }

    /// <summary>
    /// Gets the date the order was matched (null if the order is not matched).
    /// </summary>
    [JsonPropertyName("md")]
    public long? MatchedDate { get; init; }

    /// <summary>
    /// Gets the date the order was cancelled (null if the order is not cancelled)
    /// </summary>
    [JsonPropertyName("cd")]
    public long? CancelledDate { get; init; }

    /// <summary>
    /// Gets the date the order was lapsed (null if the order is not lapsed).
    /// </summary>
    [JsonPropertyName("ld")]
    public long? LapsedDate { get; init; }

    /// <summary>
    /// Gets the reason that some or all of this order has been lapsed (null if no portion of the order is lapsed).
    /// </summary>
    [JsonPropertyName("lsrc")]
    public string? LapsedStatusReasonCode { get; init; }

    /// <summary>
    /// Gets the average price the order was matched at (null if the order is not matched).
    /// </summary>
    [JsonPropertyName("avp")]
    public double? AveragePriceMatched { get; init; }

    /// <summary>
    /// Gets the amount of the order that has been matched.
    /// </summary>
    [JsonPropertyName("sm")]
    public double? SizeMatched { get; init; }

    /// <summary>
    /// Gets the amount of the order that is remaining unmatched.
    /// </summary>
    [JsonPropertyName("sr")]
    public double? SizeRemaining { get; init; }

    /// <summary>
    /// Gets the amount of the order that has been lapsed.
    /// </summary>
    [JsonPropertyName("sl")]
    public double? SizeLapsed { get; init; }

    /// <summary>
    /// Gets the amount of the order that has been cancelled.
    /// </summary>
    [JsonPropertyName("sc")]
    public double? SizeCancelled { get; init; }

    /// <summary>
    /// Gets the amount of the order that has been voided.
    /// </summary>
    [JsonPropertyName("sv")]
    public double? SizeVoided { get; init; }

    /// <summary>
    /// Gets the auth code returned by the regulator.
    /// </summary>
    [JsonPropertyName("rac")]
    public string? RegulatorAuthCode { get; init; }

    /// <summary>
    /// Gets the regulator of the order.
    /// </summary>
    [JsonPropertyName("rc")]
    public string? RegulatorCode { get; init; }

    /// <summary>
    /// Gets the customer supplied order reference.
    /// </summary>
    [JsonPropertyName("rfo")]
    public string? ReferenceOrder { get; init; }

    /// <summary>
    /// Gets the customer-supplied strategy reference used to group orders together.
    /// </summary>
    [JsonPropertyName("rfs")]
    public string? ReferenceStrategy { get; init; }
}