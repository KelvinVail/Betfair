namespace Betfair.Stream.OrderCache;

/// <summary>
/// Maintains the live order state for a single market.
/// Updated directly from raw stream bytes via <see cref="OrderCacheProcessor"/>.
/// Uses a flat array indexed by runner ordinal for O(1) lookup.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1724:The type name OrderCache conflicts in whole or in part with the namespace name", Justification = "OrderCache is the canonical domain name; renaming would harm API clarity.")]
public sealed class OrderCache
{
    private readonly Dictionary<long, int> _selectionIdToIndex = new (16);
    private OrderRunnerCache[] _runnerArray = new OrderRunnerCache[16];
    private int _runnerCount;

    public OrderCache(string marketId)
    {
        MarketId = marketId;
        MarketIdBytes = System.Text.Encoding.UTF8.GetBytes(marketId);
    }

    /// <summary>Gets the market ID (e.g. "1.241629436").</summary>
    public string MarketId { get; }

    /// <summary>Gets the account ID associated with this order market.</summary>
    public long? AccountId { get; internal set; }

    /// <summary>Gets a value indicating whether this market is closed for orders.</summary>
    public bool? Closed { get; internal set; }

    /// <summary>Gets a value indicating whether the last update was a full image.</summary>
    public bool IsImage { get; internal set; }

    /// <summary>Gets the last publish time from the stream.</summary>
    public long PublishTime { get; internal set; }

    /// <summary>Gets the number of runners with order data in this market.</summary>
    public int RunnerCount => _runnerCount;

    /// <summary>Gets all runners as a span for zero-allocation iteration.</summary>
    public ReadOnlySpan<OrderRunnerCache> RunnerSpan => _runnerArray.AsSpan(0, _runnerCount);

    /// <summary>Gets the pre-computed UTF-8 bytes of the market ID for zero-allocation comparison.</summary>
    internal byte[] MarketIdBytes { get; }

    /// <summary>Gets a runner by selection ID.</summary>
    /// <param name="selectionId">The selection (runner) ID to look up.</param>
    /// <returns>The order runner cache if found; otherwise, null.</returns>
    public OrderRunnerCache? GetRunner(long selectionId)
    {
        if (_selectionIdToIndex.TryGetValue(selectionId, out var idx))
            return _runnerArray[idx];
        return null;
    }

    /// <summary>Gets or creates an order runner cache for the given selection ID. O(1) after first encounter.</summary>
    /// <param name="selectionId">The selection (runner) ID.</param>
    /// <param name="handicap">The handicap value for the runner.</param>
    /// <returns>The existing or newly created order runner cache.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal OrderRunnerCache GetOrAddRunner(long selectionId, double handicap = 0)
    {
        if (_selectionIdToIndex.TryGetValue(selectionId, out var idx))
            return _runnerArray[idx];

        return AddRunner(selectionId, handicap);
    }

    /// <summary>Clears all runners (used before processing a full image).</summary>
    internal void ClearRunners()
    {
        Array.Clear(_runnerArray, 0, _runnerCount);
        _selectionIdToIndex.Clear();
        _runnerCount = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private OrderRunnerCache AddRunner(long selectionId, double handicap)
    {
        var runner = new OrderRunnerCache(selectionId, handicap);
        var idx = _runnerCount;

        if (idx >= _runnerArray.Length)
            Array.Resize(ref _runnerArray, _runnerArray.Length * 2);

        _runnerArray[idx] = runner;
        _selectionIdToIndex[selectionId] = idx;
        _runnerCount++;
        return runner;
    }
}
