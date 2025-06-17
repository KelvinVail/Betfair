namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;

/// <summary>
/// Fluent builder for creating PriceProjection objects.
/// </summary>
public class PriceProjectionBuilder
{
    private readonly HashSet<string> _priceData = new ();
    private ExBestOffersOverrides? _exBestOffersOverrides;
    private bool? _virtualise;
    private bool? _rolloverStakes;

    /// <summary>
    /// Includes best prices in the projection.
    /// </summary>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder IncludeBestPrices()
    {
        _priceData.Add("EX_BEST_OFFERS");
        return this;
    }

    /// <summary>
    /// Includes all available prices in the projection.
    /// </summary>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder IncludeAllPrices()
    {
        _priceData.Add("EX_ALL_OFFERS");
        return this;
    }

    /// <summary>
    /// Includes traded prices in the projection.
    /// </summary>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder IncludeTradedPrices()
    {
        _priceData.Add("EX_TRADED");
        return this;
    }

    /// <summary>
    /// Includes starting price back prices in the projection.
    /// </summary>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder IncludeStartingPriceBack()
    {
        _priceData.Add("SP_AVAILABLE");
        return this;
    }

    /// <summary>
    /// Includes starting price lay prices in the projection.
    /// </summary>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder IncludeStartingPriceLay()
    {
        _priceData.Add("SP_TRADED");
        return this;
    }

    /// <summary>
    /// Enables virtual prices in the projection.
    /// </summary>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder WithVirtualPrices()
    {
        _virtualise = true;
        return this;
    }

    /// <summary>
    /// Enables rollover stakes in the projection.
    /// </summary>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder WithRolloverStakes()
    {
        _rolloverStakes = true;
        return this;
    }

    /// <summary>
    /// Sets the best prices depth.
    /// </summary>
    /// <param name="depth">The depth of best prices to include.</param>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder WithBestPricesDepth(int depth)
    {
        _exBestOffersOverrides ??= new ExBestOffersOverrides();
        _exBestOffersOverrides.BestPricesDepth = Math.Max(1, Math.Min(10, depth));
        return this;
    }

    /// <summary>
    /// Sets the rollup model for best offers.
    /// </summary>
    /// <param name="rollupModel">The rollup model to use.</param>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder WithRollupModel(string rollupModel)
    {
        _exBestOffersOverrides ??= new ExBestOffersOverrides();
        _exBestOffersOverrides.RollupModel = rollupModel;
        return this;
    }

    /// <summary>
    /// Sets the rollup limit for best offers.
    /// </summary>
    /// <param name="rollupLimit">The rollup limit to use.</param>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder WithRollupLimit(int rollupLimit)
    {
        _exBestOffersOverrides ??= new ExBestOffersOverrides();
        _exBestOffersOverrides.RollupLimit = Math.Max(0, rollupLimit);
        return this;
    }

    /// <summary>
    /// Sets the rollup liability threshold for best offers.
    /// </summary>
    /// <param name="threshold">The liability threshold to use.</param>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder WithRollupLiabilityThreshold(double threshold)
    {
        _exBestOffersOverrides ??= new ExBestOffersOverrides();
        _exBestOffersOverrides.RollupLiabilityThreshold = Math.Max(0, threshold);
        return this;
    }

    /// <summary>
    /// Configures the projection for basic price data (best prices only).
    /// </summary>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder BasicPrices()
    {
        _priceData.Clear();
        _priceData.Add("EX_BEST_OFFERS");
        return this;
    }

    /// <summary>
    /// Configures the projection for comprehensive price data.
    /// </summary>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder ComprehensivePrices()
    {
        _priceData.Clear();
        _priceData.Add("EX_BEST_OFFERS");
        _priceData.Add("EX_ALL_OFFERS");
        _priceData.Add("EX_TRADED");
        return this;
    }

    /// <summary>
    /// Configures the projection for starting price data only.
    /// </summary>
    /// <returns>This <see cref="PriceProjectionBuilder"/>.</returns>
    public PriceProjectionBuilder StartingPricesOnly()
    {
        _priceData.Clear();
        _priceData.Add("SP_AVAILABLE");
        _priceData.Add("SP_TRADED");
        return this;
    }

    /// <summary>
    /// Builds the PriceProjection object.
    /// </summary>
    /// <returns>A configured <see cref="PriceProjection"/>.</returns>
    public PriceProjection Build()
    {
        return new PriceProjection
        {
            PriceData = _priceData.Count > 0 ? _priceData.ToList() : null,
            ExBestOffersOverrides = _exBestOffersOverrides,
            Virtualise = _virtualise,
            RolloverStakes = _rolloverStakes,
        };
    }

    /// <summary>
    /// Converts the builder to a PriceProjection.
    /// </summary>
    /// <returns>A configured <see cref="PriceProjection"/>.</returns>
    public PriceProjection ToPriceProjection()
    {
        return Build();
    }

    /// <summary>
    /// Creates a new PriceProjectionBuilder.
    /// </summary>
    /// <returns>A new <see cref="PriceProjectionBuilder"/>.</returns>
    public static PriceProjectionBuilder Create()
    {
        return new PriceProjectionBuilder();
    }

    /// <summary>
    /// Implicitly converts the builder to a PriceProjection.
    /// </summary>
    /// <param name="builder">The builder to convert.</param>
    /// <returns>A configured <see cref="PriceProjection"/>.</returns>
    public static implicit operator PriceProjection(PriceProjectionBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.Build();
    }
}

