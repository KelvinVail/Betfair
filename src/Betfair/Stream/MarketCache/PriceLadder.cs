namespace Betfair.Stream.MarketCache;

/// <summary>
/// A price ladder storing price → size mappings.
/// Uses a Dictionary for O(1) lookups and updates with minimal allocation.
/// Entries with size 0 are removed (Betfair's deletion signal).
/// </summary>
public sealed class PriceLadder
{
    private readonly Dictionary<double, double> _levels = new (64);

    /// <summary>Gets the number of active price levels.</summary>
    public int Count => _levels.Count;

    /// <summary>Gets the size at the specified price, or 0 if not present.</summary>
    /// <param name="price">The price to look up.</param>
    /// <returns>The size at the specified price, or 0 if the price is not present.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "Price values are the natural domain key for a price ladder.")]
    public double this[double price] => _levels.GetValueOrDefault(price);

    /// <summary>
    /// Applies a single price/size update. Removes the level if size is 0.
    /// </summary>
    /// <param name="price">The price level to update.</param>
    /// <param name="size">The new size; 0 removes the level.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(double price, double size)
    {
        // Betfair signals deletion with exactly 0.0 — intentional exact comparison
#pragma warning disable S1244
        if (size == 0)
#pragma warning restore S1244
            _levels.Remove(price);
        else
            _levels[price] = size;
    }

    /// <summary>Clears all price levels (used on full image).</summary>
    public void Clear() => _levels.Clear();

    /// <summary>Gets all active price levels.</summary>
    /// <returns>An enumerator over all active price/size pairs.</returns>
    public Dictionary<double, double>.Enumerator GetEnumerator() => _levels.GetEnumerator();
}

/// <summary>
/// A fixed-size position-indexed ladder for Best Available (3 levels)
/// and Best Display Available (3 levels). Uses flat arrays for O(1) access
/// with zero hashing overhead and excellent cache locality.
/// Position is the array index (0, 1, 2).
/// </summary>
public sealed class PositionLadder
{
    // Betfair best-available uses positions 0-9, but typically only 3 are populated.
    // Size to 10 to handle all possible positions without branching.
    private const int MaxPositions = 10;

    private readonly double[] _prices = new double[MaxPositions];
    private readonly double[] _sizes = new double[MaxPositions];
    private int _count;

    /// <summary>Gets the number of active positions.</summary>
    public int Count => _count;

    /// <summary>Gets the price at the specified position, or 0 if not present.</summary>
    /// <param name="position">The zero-based position index.</param>
    /// <returns>The price at the specified position, or 0 if not present.</returns>
    public double GetPrice(int position) => (uint)position < MaxPositions ? _prices[position] : 0;

    /// <summary>Gets the size at the specified position, or 0 if not present.</summary>
    /// <param name="position">The zero-based position index.</param>
    /// <returns>The size at the specified position, or 0 if not present.</returns>
    public double GetSize(int position) => (uint)position < MaxPositions ? _sizes[position] : 0;

    /// <summary>
    /// Applies a position-based update. Removes the level if size is 0.
    /// Position is the first element in the [position, price, size] tuple.
    /// </summary>
    /// <param name="position">The zero-based position index.</param>
    /// <param name="price">The price at this position.</param>
    /// <param name="size">The size; 0 removes the level.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(int position, double price, double size)
    {
        if ((uint)position >= MaxPositions) return;

        // Betfair signals deletion with exactly 0.0 — intentional exact comparison
#pragma warning disable S1244
        if (size == 0)
#pragma warning restore S1244
        {
            _prices[position] = 0;
            _sizes[position] = 0;
            RecalculateCount();
        }
        else
        {
            _prices[position] = price;
            _sizes[position] = size;
            if (position >= _count) _count = position + 1;
        }
    }

    /// <summary>Clears all positions (used on full image).</summary>
    public void Clear()
    {
        Array.Clear(_prices, 0, _count);
        Array.Clear(_sizes, 0, _count);
        _count = 0;
    }

    private void RecalculateCount()
    {
        int c = 0;
        for (int i = MaxPositions - 1; i >= 0; i--)
        {
            // Betfair signals deletion with exactly 0.0 — intentional exact comparison
#pragma warning disable S1244
            if (_sizes[i] != 0)
#pragma warning restore S1244
            {
                c = i + 1;
                break;
            }
        }

        _count = c;
    }
}
