namespace Betfair.Api.Betting.Endpoints.ListMarketBook;

/// <summary>
/// Price projection for market book requests.
/// </summary>
public class PriceProjection
{
    /// <summary>
    /// Gets or sets the price data to include.
    /// </summary>
    [JsonPropertyName("priceData")]
    public List<string>? PriceData { get; set; }

    /// <summary>
    /// Gets or sets the exchange prices to include.
    /// </summary>
    [JsonPropertyName("exBestOffersOverrides")]
    public ExBestOffersOverrides? ExBestOffersOverrides { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include virtual prices.
    /// </summary>
    [JsonPropertyName("virtualise")]
    public bool? Virtualise { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to roll up prices.
    /// </summary>
    [JsonPropertyName("rolloverStakes")]
    public bool? RolloverStakes { get; set; }
}

/// <summary>
/// Exchange best offers overrides.
/// </summary>
public class ExBestOffersOverrides
{
    /// <summary>
    /// Gets or sets the best prices depth.
    /// </summary>
    [JsonPropertyName("bestPricesDepth")]
    public int? BestPricesDepth { get; set; }

    /// <summary>
    /// Gets or sets the rollup model.
    /// </summary>
    [JsonPropertyName("rollupModel")]
    public string? RollupModel { get; set; }

    /// <summary>
    /// Gets or sets the rollup limit.
    /// </summary>
    [JsonPropertyName("rollupLimit")]
    public int? RollupLimit { get; set; }

    /// <summary>
    /// Gets or sets the rollup liability threshold.
    /// </summary>
    [JsonPropertyName("rollupLiabilityThreshold")]
    public double? RollupLiabilityThreshold { get; set; }

    /// <summary>
    /// Gets or sets the rollup liability factor.
    /// </summary>
    [JsonPropertyName("rollupLiabilityFactor")]
    public int? RollupLiabilityFactor { get; set; }
}
