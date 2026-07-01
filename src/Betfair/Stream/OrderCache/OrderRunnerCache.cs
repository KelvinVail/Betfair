using Betfair.Stream.MarketCache;

namespace Betfair.Stream.OrderCache;

/// <summary>
/// Maintains the live order state for a single runner (selection) in a market.
/// Supports both aggregated positions (matched backs/lays) and detailed
/// individual orders, as well as per-strategy matched data.
/// Uses a flat array with byte-based linear scan for zero-allocation order lookup.
/// </summary>
public sealed class OrderRunnerCache
{
    private UnmatchedOrderCache[] _orderArray = new UnmatchedOrderCache[8];
    private int _orderCount;

    // Pool of previously allocated but cleared order objects for reuse
    private UnmatchedOrderCache[] _pool = new UnmatchedOrderCache[8];
    private int _poolCount;

    private StrategyMatchCache[] _strategyArray = new StrategyMatchCache[4];
    private int _strategyCount;

    public OrderRunnerCache(long selectionId, double handicap = 0)
    {
        SelectionId = selectionId;
        Handicap = handicap;
    }

    /// <summary>Gets the selection (runner) ID.</summary>
    public long SelectionId { get; }

    /// <summary>Gets the handicap value for this runner.</summary>
    public double Handicap { get; }

    /// <summary>Gets the aggregated matched backs ladder (price → size).</summary>
    public PriceLadder MatchedBacks { get; } = new ();

    /// <summary>Gets the aggregated matched lays ladder (price → size).</summary>
    public PriceLadder MatchedLays { get; } = new ();

    /// <summary>Gets the number of individual orders on this runner.</summary>
    public int OrderCount => _orderCount;

    /// <summary>Gets all orders as a span for zero-allocation iteration.</summary>
    public ReadOnlySpan<UnmatchedOrderCache> OrderSpan => _orderArray.AsSpan(0, _orderCount);

    /// <summary>Gets individual unmatched orders keyed by bet ID (allocates on access).</summary>
    public IReadOnlyDictionary<string, UnmatchedOrderCache> Orders
    {
        get
        {
            var dict = new Dictionary<string, UnmatchedOrderCache>(_orderCount);
            for (int i = 0; i < _orderCount; i++)
                dict[_orderArray[i].BetId] = _orderArray[i];
            return dict;
        }
    }

    /// <summary>Gets strategy-partitioned matched data (allocates dictionary on access).</summary>
    public IReadOnlyDictionary<string, StrategyMatchCache> StrategyMatches
    {
        get
        {
            var dict = new Dictionary<string, StrategyMatchCache>(_strategyCount);
            for (int i = 0; i < _strategyCount; i++)
                dict[_strategyArray[i].StrategyRef] = _strategyArray[i];
            return dict;
        }
    }

    /// <summary>Gets an individual order by bet ID string, or null if not present.</summary>
    /// <param name="betId">The bet ID to look up.</param>
    /// <returns>The order cache if found; otherwise, null.</returns>
    public UnmatchedOrderCache? GetOrder(string betId)
    {
        var utf8 = System.Text.Encoding.UTF8.GetBytes(betId);
        return GetOrderByBytes(utf8);
    }

    /// <summary>Gets an individual order by UTF-8 bet ID bytes, or null if not present. Zero allocation.</summary>
    /// <param name="betIdBytes">The UTF-8 encoded bet ID to look up.</param>
    /// <returns>The order cache if found; otherwise, null.</returns>
    public UnmatchedOrderCache? GetOrderByBytes(ReadOnlySpan<byte> betIdBytes)
    {
        for (int i = 0; i < _orderCount; i++)
        {
            if (_orderArray[i].BetIdEquals(betIdBytes))
                return _orderArray[i];
        }

        return null;
    }

    /// <summary>Gets or creates a strategy match cache using byte comparison. Zero-allocation on hit.</summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the strategy ref value.</param>
    /// <returns>The existing or newly created strategy match cache.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal StrategyMatchCache GetOrAddStrategy(ref Utf8JsonReader reader)
    {
        for (int i = 0; i < _strategyCount; i++)
        {
            if (reader.ValueTextEquals(_strategyArray[i].StrategyRefBytes))
                return _strategyArray[i];
        }

        return AddStrategy(ref reader);
    }

    /// <summary>Gets or creates a strategy match cache by string. Convenience for tests.</summary>
    /// <param name="strategyRef">The strategy reference string.</param>
    /// <returns>The existing or newly created strategy match cache.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal StrategyMatchCache GetOrAddStrategy(string strategyRef)
    {
        var utf8 = System.Text.Encoding.UTF8.GetBytes(strategyRef);
        for (int i = 0; i < _strategyCount; i++)
        {
            if (_strategyArray[i].StrategyRefEquals(utf8))
                return _strategyArray[i];
        }

        return AddStrategyFromBytes(utf8);
    }

    /// <summary>
    /// Gets or creates an order cache using the Utf8JsonReader for zero-allocation comparison.
    /// Only allocates a byte[] on first encounter of a new BetId.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the BetId value.</param>
    /// <returns>The existing or newly created order cache.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal UnmatchedOrderCache GetOrAddOrder(ref Utf8JsonReader reader)
    {
        // Linear scan comparing reader bytes against cached bet ID bytes — zero allocation on hit
        for (int i = 0; i < _orderCount; i++)
        {
            if (reader.ValueTextEquals(_orderArray[i].BetIdBytes))
                return _orderArray[i];
        }

        return AddOrder(ref reader);
    }

    /// <summary>Gets or creates an order cache for the given bet ID string. Allocates on first call.</summary>
    /// <param name="betId">The bet ID string.</param>
    /// <returns>The existing or newly created order cache.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal UnmatchedOrderCache GetOrAddOrder(string betId)
    {
        var utf8 = System.Text.Encoding.UTF8.GetBytes(betId);
        for (int i = 0; i < _orderCount; i++)
        {
            if (_orderArray[i].BetIdEquals(utf8))
                return _orderArray[i];
        }

        return AddOrderFromBytes(utf8);
    }

    /// <summary>Removes an order by bet ID.</summary>
    /// <param name="betId">The bet ID to remove.</param>
    internal void RemoveOrder(string betId)
    {
        var utf8 = System.Text.Encoding.UTF8.GetBytes(betId);
        for (int i = 0; i < _orderCount; i++)
        {
            if (_orderArray[i].BetIdEquals(utf8))
            {
                // Swap with last element and decrement count
                _orderCount--;
                if (i < _orderCount)
                    _orderArray[i] = _orderArray[_orderCount];
                _orderArray[_orderCount] = null!;
                return;
            }
        }
    }

    /// <summary>Clears all orders and ladders (used on full image). Orders are pooled for reuse.</summary>
    internal void Clear()
    {
        MatchedBacks.Clear();
        MatchedLays.Clear();

        // Move current orders to pool for reuse
        for (int i = 0; i < _orderCount; i++)
        {
            if (_poolCount >= _pool.Length)
                Array.Resize(ref _pool, _pool.Length * 2);
            _pool[_poolCount++] = _orderArray[i];
        }

        Array.Clear(_orderArray, 0, _orderCount);
        _orderCount = 0;
        Array.Clear(_strategyArray, 0, _strategyCount);
        _strategyCount = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private UnmatchedOrderCache AddOrder(ref Utf8JsonReader reader)
    {
        // Only allocates bytes on first encounter of a new BetId
        byte[] betIdBytes;
        if (reader.HasValueSequence)
        {
            betIdBytes = reader.ValueSequence.ToArray();
        }
        else
        {
            betIdBytes = reader.ValueSpan.ToArray();
        }

        return AddOrderFromBytes(betIdBytes);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private UnmatchedOrderCache AddOrderFromBytes(byte[] betIdBytes)
    {
        UnmatchedOrderCache order;

        if (_poolCount > 0)
        {
            order = _pool[--_poolCount];
            _pool[_poolCount] = null!;
            order.Reset(betIdBytes);
        }
        else
        {
            order = new UnmatchedOrderCache(betIdBytes);
        }

        var idx = _orderCount;

        if (idx >= _orderArray.Length)
            Array.Resize(ref _orderArray, _orderArray.Length * 2);

        _orderArray[idx] = order;
        _orderCount++;
        return order;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private StrategyMatchCache AddStrategy(ref Utf8JsonReader reader)
    {
        byte[] bytes = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();
        return AddStrategyFromBytes(bytes);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private StrategyMatchCache AddStrategyFromBytes(byte[] strategyRefBytes)
    {
        var cache = new StrategyMatchCache(strategyRefBytes);
        if (_strategyCount >= _strategyArray.Length)
            Array.Resize(ref _strategyArray, _strategyArray.Length * 2);
        _strategyArray[_strategyCount++] = cache;
        return cache;
    }
}
