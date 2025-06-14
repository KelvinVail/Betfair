using Betfair.Api.Responses.Orders;

namespace Betfair.Api.Responses.Markets;

/// <summary>
/// The dynamic data about runners in a market.
/// </summary>
public class Runner
{
    /// <summary>
    /// Gets the unique id of the runner (selection).
    /// </summary>
    [JsonPropertyName("selectionId")]
    public long SelectionId { get; init; }

    /// <summary>
    /// Gets the handicap.
    /// </summary>
    [JsonPropertyName("handicap")]
    public double Handicap { get; init; }

    /// <summary>
    /// Gets the status of the selection (i.e., ACTIVE, REMOVED, WINNER, LOSER).
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets the adjustment factor applied if the selection is removed.
    /// </summary>
    [JsonPropertyName("adjustmentFactor")]
    public double AdjustmentFactor { get; init; }

    /// <summary>
    /// Gets the price of the most recent bet matched on this selection.
    /// </summary>
    [JsonPropertyName("lastPriceTraded")]
    public double? LastPriceTraded { get; init; }

    /// <summary>
    /// Gets the total amount matched on this runner. This value is truncated at 2dp.
    /// </summary>
    [JsonPropertyName("totalMatched")]
    public double? TotalMatched { get; init; }

    /// <summary>
    /// Gets the date and time the runner was removed.
    /// </summary>
    [JsonPropertyName("removalDate")]
    public DateTimeOffset? RemovalDate { get; init; }

    /// <summary>
    /// Gets the BSP related prices for this runner.
    /// </summary>
    [JsonPropertyName("sp")]
    public StartingPrices? StartingPrices { get; init; }

    /// <summary>
    /// Gets the Exchange prices available for this runner.
    /// </summary>
    [JsonPropertyName("ex")]
    public ExchangePrices? ExchangePrices { get; init; }

    /// <summary>
    /// Gets the list of orders in the market.
    /// </summary>
    [JsonPropertyName("orders")]
    public List<Order>? Orders { get; init; }

    /// <summary>
    /// Gets the list of matches (i.e, orders that have been fully or partially executed).
    /// </summary>
    [JsonPropertyName("matches")]
    public List<Match>? Matches { get; init; }

    /// <summary>
    /// Gets the list of matches keyed by strategy, ordered by matched data.
    /// </summary>
    [JsonPropertyName("matchesByStrategy")]
    public Dictionary<string, Matches>? MatchesByStrategy { get; init; }
}
