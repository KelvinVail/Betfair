using System.Runtime.CompilerServices;

namespace Betfair.Stream.MarketCache;

/// <summary>
/// Maintains the live state for a single market.
/// Updated directly from raw stream bytes via <see cref="MarketCacheProcessor"/>.
/// </summary>
public sealed class MarketCache
{
    private readonly Dictionary<long, RunnerCache> _runners = new(16);

    public MarketCache(string marketId)
    {
        MarketId = marketId;
        MarketIdBytes = System.Text.Encoding.UTF8.GetBytes(marketId);
    }

    /// <summary>Gets the market ID (e.g. "1.241629436").</summary>
    public string MarketId { get; }

    /// <summary>Gets the pre-computed UTF-8 bytes of the market ID for zero-allocation comparison.</summary>
    internal byte[] MarketIdBytes { get; }

    /// <summary>Gets the total matched volume on this market.</summary>
    public double? TotalMatched { get; internal set; }

    /// <summary>Gets whether the last update was a full image.</summary>
    public bool IsImage { get; internal set; }

    /// <summary>Gets the last publish time from the stream.</summary>
    public long PublishTime { get; internal set; }

    /// <summary>Gets the market definition, updated in-place from stream bytes.</summary>
    public MarketDefinitionCache Definition { get; } = new();

    /// <summary>Gets the number of runners in this market.</summary>
    public int RunnerCount => _runners.Count;

    /// <summary>Gets a runner by selection ID.</summary>
    public RunnerCache? GetRunner(long selectionId) =>
        _runners.GetValueOrDefault(selectionId);

    /// <summary>Gets all runners.</summary>
    public IReadOnlyDictionary<long, RunnerCache> Runners => _runners;

    /// <summary>Gets or creates a runner cache for the given selection ID.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal RunnerCache GetOrAddRunner(long selectionId, double handicap = 0)
    {
        if (_runners.TryGetValue(selectionId, out var runner))
            return runner;

        runner = new RunnerCache(selectionId, handicap);
        _runners[selectionId] = runner;
        return runner;
    }

    /// <summary>Clears all runners (used before processing a full image).</summary>
    internal void ClearRunners() => _runners.Clear();
}
