using System.Runtime.CompilerServices;

namespace Betfair.Stream.MarketCache;

/// <summary>
/// A price ladder storing price → size mappings.
/// Uses a Dictionary for O(1) lookups and updates with minimal allocation.
/// Entries with size 0 are removed (Betfair's deletion signal).
/// </summary>
public sealed class PriceLadder
{
    private readonly Dictionary<double, double> _levels = new(64);

    /// <summary>Gets the number of active price levels.</summary>
    public int Count => _levels.Count;

    /// <summary>Gets the size at the specified price, or 0 if not present.</summary>
    public double this[double price] => _levels.GetValueOrDefault(price);

    /// <summary>
    /// Applies a single price/size update. Removes the level if size is 0.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(double price, double size)
    {
        if (size == 0)
            _levels.Remove(price);
        else
            _levels[price] = size;
    }

    /// <summary>Clears all price levels (used on full image).</summary>
    public void Clear() => _levels.Clear();

    /// <summary>Gets all active price levels.</summary>
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
    public double GetPrice(int position) => (uint)position < MaxPositions ? _prices[position] : 0;

    /// <summary>Gets the size at the specified position, or 0 if not present.</summary>
    public double GetSize(int position) => (uint)position < MaxPositions ? _sizes[position] : 0;

    /// <summary>
    /// Applies a position-based update. Removes the level if size is 0.
    /// Position is the first element in the [position, price, size] tuple.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(int position, double price, double size)
    {
        if ((uint)position >= MaxPositions) return;

        if (size == 0)
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
            if (_sizes[i] != 0) { c = i + 1; break; }
        }

        _count = c;
    }
}
