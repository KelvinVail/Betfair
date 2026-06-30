namespace Betfair.Stream.MarketCache;

/// <summary>
/// Maintains the live state for a single market.
/// Updated directly from raw stream bytes via <see cref="MarketCacheProcessor"/>.
/// Uses a flat array indexed by runner ordinal for O(1) lookup.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1724:The type name MarketCache conflicts in whole or in part with the namespace name", Justification = "MarketCache is the canonical domain name; renaming would harm API clarity.")]
public sealed class MarketCache
{
    // Map selection ID → ordinal index for O(1) lookup after first encounter
    private readonly Dictionary<long, int> _selectionIdToIndex = new (16);

    // Start with capacity for typical horse race (16 runners), grows if needed
    private RunnerCache[] _runnerArray = new RunnerCache[16];
    private int _runnerCount;

    public MarketCache(string marketId)
    {
        MarketId = marketId;
        MarketIdBytes = System.Text.Encoding.UTF8.GetBytes(marketId);
    }

    /// <summary>Gets the market ID (e.g. "1.241629436").</summary>
    public string MarketId { get; }

    /// <summary>Gets the total matched volume on this market.</summary>
    public double? TotalMatched { get; internal set; }

    /// <summary>Gets a value indicating whether the last update was a full image.</summary>
    public bool IsImage { get; internal set; }

    /// <summary>Gets the last publish time from the stream.</summary>
    public long PublishTime { get; internal set; }

    /// <summary>Gets the market definition, updated in-place from stream bytes.</summary>
    public MarketDefinitionCache Definition { get; } = new ();

    /// <summary>Gets the number of runners in this market.</summary>
    public int RunnerCount => _runnerCount;

    /// <summary>Gets all runners as a span for zero-allocation iteration.</summary>
    public ReadOnlySpan<RunnerCache> RunnerSpan => _runnerArray.AsSpan(0, _runnerCount);

    /// <summary>Gets all runners as a dictionary (allocates wrapper on first access).</summary>
    public IReadOnlyDictionary<long, RunnerCache> Runners
    {
        get
        {
            var dict = new Dictionary<long, RunnerCache>(_runnerCount);
            for (int i = 0; i < _runnerCount; i++)
            {
                dict[_runnerArray[i].SelectionId] = _runnerArray[i];
            }

            return dict;
        }
    }

    /// <summary>Gets the pre-computed UTF-8 bytes of the market ID for zero-allocation comparison.</summary>
    internal byte[] MarketIdBytes { get; }

    /// <summary>Gets a runner by selection ID.</summary>
    /// <param name="selectionId">The selection (runner) ID to look up.</param>
    /// <returns>The runner cache if found; otherwise, null.</returns>
    public RunnerCache? GetRunner(long selectionId)
    {
        if (_selectionIdToIndex.TryGetValue(selectionId, out var idx))
            return _runnerArray[idx];
        return null;
    }

    /// <summary>Gets or creates a runner cache for the given selection ID. O(1) after first encounter.</summary>
    /// <param name="selectionId">The selection (runner) ID.</param>
    /// <param name="handicap">The handicap value for the runner.</param>
    /// <returns>The existing or newly created runner cache.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal RunnerCache GetOrAddRunner(long selectionId, double handicap = 0)
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
    private RunnerCache AddRunner(long selectionId, double handicap)
    {
        var runner = new RunnerCache(selectionId, handicap);
        var idx = _runnerCount;

        if (idx >= _runnerArray.Length)
            Array.Resize(ref _runnerArray, _runnerArray.Length * 2);

        _runnerArray[idx] = runner;
        _selectionIdToIndex[selectionId] = idx;
        _runnerCount++;
        return runner;
    }
}
