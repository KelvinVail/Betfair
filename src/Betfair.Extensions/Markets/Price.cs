namespace Betfair.Extensions.Markets;

/// <summary>
/// A Price value object containing properties and methods to help when dealing with Betfair prices.
/// </summary>
[JsonConverter(typeof(PriceJsonConverter))]
public sealed class Price : ValueObject
{
    private const double _highestInvalidSize = 1.59;
    private static readonly Dictionary<double, Price> _prices = new (PriceExtensions.ValidPrices.Length);
    private static readonly Dictionary<int, Price> _indexes = new (PriceExtensions.ValidPrices.Length);

    static Price()
    {
        for (int i = 0; i < PriceExtensions.ValidPrices.Length; i++)
        {
            var price = new Price(PriceExtensions.ValidPrices[i], i);
            _prices.Add(PriceExtensions.ValidPrices[i], price);
            _indexes.Add(i, price);
        }
    }

    private Price(double decimalOdds, int tick)
    {
        DecimalOdds = decimalOdds;
        Tick = tick;
        Chance = 1 / decimalOdds;
        MinimumSize = Math.Min(Math.Ceiling(10 / decimalOdds * 100) / 100, 1);
        IsValidPrice = _prices.ContainsKey(decimalOdds);
    }

    /// <summary>
    /// Gets a Null or None Price object.
    /// </summary>
    public static Price None => new (-1, -1);

    /// <summary>
    /// Gets the Betfair tick level of the price.
    /// Invalid prices will return the tick of the nearest valid price. Invalid prices can be created when prices are adjusted by withdrawals.
    /// </summary>
    public int Tick { get; }

    /// <summary>
    /// Gets the decimal odds of the price.
    /// </summary>
    public double DecimalOdds { get; }

    /// <summary>
    /// Gets the chance of the price winning.
    /// </summary>
    public double Chance { get; }

    /// <summary>
    /// Gets the minimum size that can be placed at this price.
    /// </summary>
    public double MinimumSize { get; }

    /// <summary>
    /// Gets a value indicating whether the price is a valid price.
    /// </summary>
    public bool IsValidPrice { get; }

    /// <summary>
    /// Create a price object from a decimal odds value.
    /// </summary>
    /// <param name="decimalOdds">Decimal odds to create the price object from.</param>
    /// <returns>A Price value object</returns>
    public static Price Of(double decimalOdds)
    {
        _prices.TryGetValue(decimalOdds, out var price);
        if (price is not null) return price;

        var nearestKey = NearestTick(decimalOdds);
        return new Price(decimalOdds, nearestKey);
    }

    /// <summary>
    /// Add or subtract ticks to/from the price.
    /// </summary>
    /// <param name="ticks">Ticks to add or subtract.</param>
    /// <returns>A new price object.</returns>
    public Price AddTicks(int ticks)
    {
        var newIndex = Tick + ticks;
        return newIndex switch
        {
            >= 350 => _prices[1000],
            < 1 => _prices[1.01],
            _ => _indexes[newIndex]
        };
    }

    /// <summary>
    /// The ticks between the current price and the end price.
    /// </summary>
    /// <param name="endPrice">The price to calculate the number of ticks between.</param>
    /// <returns>The tick difference between this price and the supplied end price.</returns>
    public int TicksBetween(Price? endPrice)
    {
        if (endPrice == null) return 0;

        return endPrice.Tick - Tick;
    }

    /// <summary>
    /// Reduce this price by a reduction factor.
    /// </summary>
    /// <param name="reductionFactor">The reduction factor to apply.</param>
    /// <returns>A new reduced price.</returns>
    public Price ReduceBy(double reductionFactor)
    {
        if (reductionFactor < 2.5) return this;
        var priceReduction = (DecimalOdds / 100) * reductionFactor;
        var reducedOdds = Math.Round(DecimalOdds - priceReduction, 2);
        var nearestKey = NearestTick(reducedOdds);
        return new Price(reducedOdds, _indexes[nearestKey].Tick);
    }

    /// <summary>
    /// Is this price achievable with the given size.
    /// Sizes below the minimum size are usually achievable but some price:sizes ratios are not allowed due to unfair rounding gains.
    /// This method checks if the price:size ratio is ultimately achievable.
    /// </summary>
    /// <param name="size">The desired size to place at this price.</param>
    /// <returns>Whether the size can be achieved.</returns>
    public bool IsSizeAchievable(double size) => size > _highestInvalidSize || IsProfitRatioValid(size);

    /// <summary>
    /// The size needed to achieve a profit target.
    /// </summary>
    /// <param name="profitTarget">The desired profit target.</param>
    /// <returns>The size required to achieve the profit target.</returns>
    public double SizeNeededForProfit(double profitTarget) => Math.Round(profitTarget / (DecimalOdds - 1), 2);

    public override string ToString() => $"{DecimalOdds}";

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return DecimalOdds;
    }

    private static int NearestTick(double value)
    {
        int index = 0;
        double minDifference = double.MaxValue;
        for (int i = 0; i < PriceExtensions.ValidPrices.Length; i++)
        {
            double currentDifference = Math.Abs((1 / PriceExtensions.ValidPrices[i]) - (1 / value));
            if (currentDifference < minDifference)
            {
                index = i;
                minDifference = currentDifference;
            }
        }

        return index;
    }

    private bool IsProfitRatioValid(double size)
    {
        var unRoundedProfit = (DecimalOdds * size) - size;
        var roundedProfit = Math.Round(unRoundedProfit, 2);
        var roundedGain = (roundedProfit - unRoundedProfit) / unRoundedProfit;
        return Math.Round(roundedGain, 2) is >= -0.2 and <= 0.25;
    }
}