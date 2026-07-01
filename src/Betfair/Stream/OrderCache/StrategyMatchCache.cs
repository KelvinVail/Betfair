using Betfair.Stream.MarketCache;

namespace Betfair.Stream.OrderCache;

/// <summary>
/// Maintains matched back/lay ladders for a single strategy reference.
/// Updated in-place from stream deltas. Uses byte-based identity with
/// lazy string decoding.
/// </summary>
public sealed class StrategyMatchCache
{
    private readonly byte[] _strategyRefBytes;
    private string? _strategyRefString;

    internal StrategyMatchCache(byte[] strategyRefBytes)
    {
        _strategyRefBytes = strategyRefBytes;
    }

    /// <summary>Gets the strategy reference (lazily decoded from UTF-8 bytes).</summary>
    public string StrategyRef => _strategyRefString ??= System.Text.Encoding.UTF8.GetString(_strategyRefBytes);

    /// <summary>Gets the raw UTF-8 bytes of the strategy reference.</summary>
    public ReadOnlySpan<byte> StrategyRefBytes => _strategyRefBytes;

    /// <summary>Gets the matched backs ladder for this strategy (price → size).</summary>
    public PriceLadder MatchedBacks { get; } = new ();

    /// <summary>Gets the matched lays ladder for this strategy (price → size).</summary>
    public PriceLadder MatchedLays { get; } = new ();

    /// <summary>Clears all ladder data.</summary>
    internal void Clear()
    {
        MatchedBacks.Clear();
        MatchedLays.Clear();
    }

    /// <summary>Returns true if this strategy's ref matches the given UTF-8 bytes.</summary>
    /// <param name="utf8Ref">The UTF-8 bytes to compare against.</param>
    /// <returns>True if the bytes match this strategy's reference.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool StrategyRefEquals(ReadOnlySpan<byte> utf8Ref) =>
        utf8Ref.SequenceEqual(_strategyRefBytes);
}
