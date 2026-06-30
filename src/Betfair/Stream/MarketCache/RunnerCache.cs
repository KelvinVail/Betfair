namespace Betfair.Stream.MarketCache;

/// <summary>
/// Maintains the live state for a single runner (selection) in a market.
/// All price ladders are updated in-place from stream deltas.
/// Uses NaN sentinels instead of Nullable&lt;double&gt; to eliminate wrapping overhead.
/// </summary>
public sealed class RunnerCache
{
    public RunnerCache(long selectionId, double handicap = 0)
    {
        SelectionId = selectionId;
        Handicap = handicap;
    }

    /// <summary>Gets the selection (runner) ID.</summary>
    public long SelectionId { get; }

    /// <summary>Gets the handicap value for this runner.</summary>
    public double Handicap { get; }

    /// <summary>Gets or sets the last traded price. NaN means not set.</summary>
    public double LastTradedPrice { get; set; } = double.NaN;

    /// <summary>Gets or sets the total volume matched on this runner. NaN means not set.</summary>
    public double TotalMatched { get; set; } = double.NaN;

    /// <summary>Gets or sets the starting price near. NaN means not set.</summary>
    public double StartingPriceNear { get; set; } = double.NaN;

    /// <summary>Gets or sets the starting price far. NaN means not set.</summary>
    public double StartingPriceFar { get; set; } = double.NaN;

    /// <summary>Gets the full depth available to back ladder (price → size).</summary>
    public PriceLadder AvailableToBack { get; } = new ();

    /// <summary>Gets the full depth available to lay ladder (price → size).</summary>
    public PriceLadder AvailableToLay { get; } = new ();

    /// <summary>Gets the best available to back ladder (position → price/size).</summary>
    public PositionLadder BestAvailableToBack { get; } = new ();

    /// <summary>Gets the best available to lay ladder (position → price/size).</summary>
    public PositionLadder BestAvailableToLay { get; } = new ();

    /// <summary>Gets the best display available to back ladder.</summary>
    public PositionLadder BestDisplayAvailableToBack { get; } = new ();

    /// <summary>Gets the best display available to lay ladder.</summary>
    public PositionLadder BestDisplayAvailableToLay { get; } = new ();

    /// <summary>Gets the traded volume ladder (price → cumulative size).</summary>
    public PriceLadder Traded { get; } = new ();

    /// <summary>Gets the starting price back ladder.</summary>
    public PriceLadder StartingPriceBack { get; } = new ();

    /// <summary>Gets the starting price lay ladder.</summary>
    public PriceLadder StartingPriceLay { get; } = new ();

    /// <summary>Returns true if LastTradedPrice has been set.</summary>
    public bool HasLastTradedPrice => !double.IsNaN(LastTradedPrice);

    /// <summary>Returns true if TotalMatched has been set.</summary>
    public bool HasTotalMatched => !double.IsNaN(TotalMatched);

    /// <summary>Returns true if StartingPriceNear has been set.</summary>
    public bool HasStartingPriceNear => !double.IsNaN(StartingPriceNear);

    /// <summary>Returns true if StartingPriceFar has been set.</summary>
    public bool HasStartingPriceFar => !double.IsNaN(StartingPriceFar);

    /// <summary>Clears all ladders (used on full image before repopulating).</summary>
    internal void ClearLadders()
    {
        AvailableToBack.Clear();
        AvailableToLay.Clear();
        BestAvailableToBack.Clear();
        BestAvailableToLay.Clear();
        BestDisplayAvailableToBack.Clear();
        BestDisplayAvailableToLay.Clear();
        Traded.Clear();
        StartingPriceBack.Clear();
        StartingPriceLay.Clear();
        LastTradedPrice = double.NaN;
        TotalMatched = double.NaN;
        StartingPriceNear = double.NaN;
        StartingPriceFar = double.NaN;
    }
}
