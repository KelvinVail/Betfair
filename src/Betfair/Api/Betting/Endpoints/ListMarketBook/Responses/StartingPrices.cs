namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;

/// <summary>
/// Information about the Betfair Starting Price. Only available in BSP markets
/// </summary>
public class StartingPrices
{
    /// <summary>
    /// Gets what the starting price would be if the market was reconciled now,
    /// taking into account the SP bets as well as unmatched exchange bets on the same selection in the exchange.
    /// This data is cached and update every 60 seconds.
    /// Please note: Type Double may contain numbers, INF, -INF, and NaN.
    /// </summary>
    [JsonPropertyName("nearPrice")]
    public double? NearPrice { get; init; }

    /// <summary>
    /// Gets what the starting price would be if the market was reconciled now,
    /// taking into account only the currently placed SP bets.
    /// The Far Price is not as complicated but not as accurate and only accounts for money on the exchange at SP.
    /// This data is cached and updated every 60 seconds.
    /// Please note: Type Double may contain numbers, INF, -INF, and NaN.
    /// </summary>
    [JsonPropertyName("farPrice")]
    public double? FarPrice { get; init; }

    /// <summary>
    /// Gets the total amount of back bets matched at the actual Betfair Starting Price.
    /// Pre-reconciliation, this field is zero for all prices except 1.01 (for Market on Close bets) and at the limit price for any Limit on Close bets.
    /// </summary>
    [JsonPropertyName("backStakeTaken")]
    public List<PriceSize>? BackStakeTaken { get; init; }

    /// <summary>
    /// Gets the lay amount matched at the actual Betfair Starting Price.
    /// Pre-reconciliation, this field is zero for all prices except 1000 (for Market on Close bets) and at the limit price for any Limit on Close bets.
    /// </summary>
    [JsonPropertyName("layLiabilityTaken")]
    public List<PriceSize>? LayLiabilityTaken { get; init; }

    /// <summary>
    /// Gets the final BSP price for this runner. Only available for a BSP market that has been reconciled.
    /// Please note: for REMOVED runners the actualSP will be returned as 'NaN. Value may be returned as 'Infinity' if no BSP can be calculated.
    /// </summary>
    [JsonPropertyName("actualSP")]
    public double? ActualSp { get; init; }
}
