namespace Betfair.Api.Responses;

/// <summary>
/// Information about a market.
/// </summary>
public sealed class MarketCatalogue
{
    /// <summary>
    /// Gets the unique identifier for the market.
    /// </summary>
    [JsonPropertyName("marketId")]
    public string MarketId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the market.
    /// </summary>
    [JsonPropertyName("marketName")]
    public string MarketName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the total amount matched on the market.
    /// </summary>
    [JsonPropertyName("totalMatched")]
    public decimal TotalMatched { get; init; }

    /// <summary>
    /// Gets the start time of the market.
    /// </summary>
    [JsonPropertyName("marketStartTime")]
    public DateTimeOffset? MarketStartTime { get; init; }

    /// <summary>
    /// Gets the competition associated with the market.
    /// </summary>
    [JsonPropertyName("competition")]
    public Competition? Competition { get; init; }

    /// <summary>
    /// Gets the event associated with the market.
    /// </summary>
    [JsonPropertyName("event")]
    public MarketEvent? Event { get; init; }

    /// <summary>
    /// Gets the event type associated with the market.
    /// </summary>
    [JsonPropertyName("eventType")]
    public EventType? EventType { get; init; }

    /// <summary>
    /// Gets the market description.
    /// </summary>
    [JsonPropertyName("description")]
    public MarketDescription? Description { get; init; }

    /// <summary>
    /// Gets the runners (selections) in the market.
    /// </summary>
    [JsonPropertyName("runners")]
    public IEnumerable<RunnerResponse>? Runners { get; init; }
}
