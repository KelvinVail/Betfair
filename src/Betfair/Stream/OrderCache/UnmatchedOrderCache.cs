using Betfair.Core.Enums;

namespace Betfair.Stream.OrderCache;

/// <summary>
/// Maintains the live state of a single unmatched order.
/// Updated in-place from stream deltas. Uses NaN sentinels for doubles,
/// byte-sized enums for categorical fields, UTF-8 byte identity for
/// zero-allocation lookup, and cached-bytes pattern for string fields
/// to avoid re-allocation when values are unchanged.
/// </summary>
public sealed class UnmatchedOrderCache
{
    private byte[] _betIdBytes;
    private string? _betIdString;

    // Cached UTF-8 bytes for string fields — only allocate string when value changes
    private byte[]? _lsrcBytes;
    private string? _lsrcString;
    private byte[]? _racBytes;
    private string? _racString;
    private byte[]? _rcBytes;
    private string? _rcString;
    private byte[]? _rfoBytes;
    private string? _rfoString;
    private byte[]? _rfsBytes;
    private string? _rfsString;

    internal UnmatchedOrderCache(byte[] betIdBytes)
    {
        _betIdBytes = betIdBytes;
    }

    /// <summary>Gets the bet ID (lazily decoded from UTF-8 bytes on first access).</summary>
    public string BetId => _betIdString ??= System.Text.Encoding.UTF8.GetString(_betIdBytes);

    /// <summary>Gets the raw UTF-8 bytes of the bet ID for zero-allocation comparison.</summary>
    public ReadOnlySpan<byte> BetIdBytes => _betIdBytes;

    /// <summary>Gets or sets the original placed price.</summary>
    public double Price { get; set; } = double.NaN;

    /// <summary>Gets or sets the original placed size.</summary>
    public double Size { get; set; } = double.NaN;

    /// <summary>Gets or sets the BSP liability (NaN if not a BSP order).</summary>
    public double BspLiability { get; set; } = double.NaN;

    /// <summary>Gets or sets the side of the order.</summary>
    public Side Side { get; set; }

    /// <summary>Gets or sets the order status.</summary>
    public OrderStatus Status { get; set; }

    /// <summary>Gets or sets the persistence type.</summary>
    public PersistenceType PersistenceType { get; set; }

    /// <summary>Gets or sets the order type.</summary>
    public OrderType OrderType { get; set; }

    /// <summary>Gets or sets the placed date (epoch millis).</summary>
    public long PlacedDate { get; set; }

    /// <summary>Gets or sets the matched date (epoch millis, 0 if not matched).</summary>
    public long MatchedDate { get; set; }

    /// <summary>Gets or sets the cancelled date (epoch millis, 0 if not cancelled).</summary>
    public long CancelledDate { get; set; }

    /// <summary>Gets or sets the lapsed date (epoch millis, 0 if not lapsed).</summary>
    public long LapsedDate { get; set; }

    /// <summary>Gets the lapsed status reason code (lazily decoded, zero-allocation if unchanged).</summary>
    public string? LapsedStatusReasonCode => _lsrcBytes != null ? (_lsrcString ??= System.Text.Encoding.UTF8.GetString(_lsrcBytes)) : null;

    /// <summary>Gets or sets the average price matched.</summary>
    public double AveragePriceMatched { get; set; } = double.NaN;

    /// <summary>Gets or sets the size matched.</summary>
    public double SizeMatched { get; set; } = double.NaN;

    /// <summary>Gets or sets the size remaining.</summary>
    public double SizeRemaining { get; set; } = double.NaN;

    /// <summary>Gets or sets the size lapsed.</summary>
    public double SizeLapsed { get; set; } = double.NaN;

    /// <summary>Gets or sets the size cancelled.</summary>
    public double SizeCancelled { get; set; } = double.NaN;

    /// <summary>Gets or sets the size voided.</summary>
    public double SizeVoided { get; set; } = double.NaN;

    /// <summary>Gets the regulator auth code (lazily decoded, zero-allocation if unchanged).</summary>
    public string? RegulatorAuthCode => _racBytes != null ? (_racString ??= System.Text.Encoding.UTF8.GetString(_racBytes)) : null;

    /// <summary>Gets the regulator code (lazily decoded, zero-allocation if unchanged).</summary>
    public string? RegulatorCode => _rcBytes != null ? (_rcString ??= System.Text.Encoding.UTF8.GetString(_rcBytes)) : null;

    /// <summary>Gets the customer-supplied order reference (lazily decoded, zero-allocation if unchanged).</summary>
    public string? OrderReference => _rfoBytes != null ? (_rfoString ??= System.Text.Encoding.UTF8.GetString(_rfoBytes)) : null;

    /// <summary>Gets the customer-supplied strategy reference (lazily decoded, zero-allocation if unchanged).</summary>
    public string? StrategyReference => _rfsBytes != null ? (_rfsString ??= System.Text.Encoding.UTF8.GetString(_rfsBytes)) : null;

    /// <summary>
    /// Updates the lapsed status reason code from raw stream bytes.
    /// Only allocates if the value has actually changed.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetLapsedStatusReasonCode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            _lsrcBytes = null;
            _lsrcString = null;
            return;
        }

        if (_lsrcBytes != null && reader.ValueTextEquals(_lsrcBytes)) return;
        _lsrcBytes = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();
        _lsrcString = null;
    }

    /// <summary>Updates the regulator auth code from raw stream bytes. Zero-allocation if unchanged.</summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetRegulatorAuthCode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            _racBytes = null;
            _racString = null;
            return;
        }

        if (_racBytes != null && reader.ValueTextEquals(_racBytes)) return;
        _racBytes = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();
        _racString = null;
    }

    /// <summary>Updates the regulator code from raw stream bytes. Zero-allocation if unchanged.</summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetRegulatorCode(ref Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            _rcBytes = null;
            _rcString = null;
            return;
        }

        if (_rcBytes != null && reader.ValueTextEquals(_rcBytes)) return;
        _rcBytes = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();
        _rcString = null;
    }

    /// <summary>Updates the order reference from raw stream bytes. Zero-allocation if unchanged.</summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetOrderReference(ref Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            _rfoBytes = null;
            _rfoString = null;
            return;
        }

        if (_rfoBytes != null && reader.ValueTextEquals(_rfoBytes)) return;
        _rfoBytes = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();
        _rfoString = null;
    }

    /// <summary>Updates the strategy reference from raw stream bytes. Zero-allocation if unchanged.</summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetStrategyReference(ref Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            _rfsBytes = null;
            _rfsString = null;
            return;
        }

        if (_rfsBytes != null && reader.ValueTextEquals(_rfsBytes)) return;
        _rfsBytes = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();
        _rfsString = null;
    }

    /// <summary>Resets this instance for reuse with a new BetId.</summary>
    /// <param name="betIdBytes">The UTF-8 bytes of the new BetId.</param>
    internal void Reset(byte[] betIdBytes)
    {
        _betIdBytes = betIdBytes;
        _betIdString = null;
        Price = double.NaN;
        Size = double.NaN;
        BspLiability = double.NaN;
        Side = Side.Unknown;
        Status = OrderStatus.Unknown;
        PersistenceType = PersistenceType.Unknown;
        OrderType = OrderType.Unknown;
        PlacedDate = 0;
        MatchedDate = 0;
        CancelledDate = 0;
        LapsedDate = 0;
        _lsrcBytes = null;
        _lsrcString = null;
        AveragePriceMatched = double.NaN;
        SizeMatched = double.NaN;
        SizeRemaining = double.NaN;
        SizeLapsed = double.NaN;
        SizeCancelled = double.NaN;
        SizeVoided = double.NaN;
        _racBytes = null;
        _racString = null;
        _rcBytes = null;
        _rcString = null;
        _rfoBytes = null;
        _rfoString = null;
        _rfsBytes = null;
        _rfsString = null;
    }

    /// <summary>Returns true if this order's BetId matches the given UTF-8 bytes.</summary>
    /// <param name="utf8BetId">The UTF-8 bytes to compare against.</param>
    /// <returns>True if the bytes match this order's BetId.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool BetIdEquals(ReadOnlySpan<byte> utf8BetId) =>
        utf8BetId.SequenceEqual(_betIdBytes);
}
