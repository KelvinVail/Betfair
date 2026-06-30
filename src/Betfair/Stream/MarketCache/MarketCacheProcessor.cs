using System.Diagnostics;
using Betfair.Api.Betting.Enums;

namespace Betfair.Stream.MarketCache;

/// <summary>
/// Ultra-low-latency processor that reads raw stream bytes directly into
/// a <see cref="MarketCache"/> using <see cref="Utf8JsonReader"/>.
/// Zero allocation on steady-state delta messages.
/// </summary>
public sealed class MarketCacheProcessor
{
    // Single-market fast path: most subscriptions have 1 market.
    private MarketCache? _singleMarket;

    // Fallback for multi-market subscriptions
    private Dictionary<string, MarketCache>? _multiMarkets;

    // Reusable buffer for deferred ladder updates (instance-level, stays in L1 cache)
    private DeferredLadderUpdate[] _deferredBuffer = new DeferredLadderUpdate[256];

    // Reusable clock buffers to avoid string allocation on every message
    private byte[] _clockBuffer = new byte[64];
    private int _clockLength;
    private byte[] _initialClockBuffer = new byte[64];
    private int _initialClockLength;

    [JsonConverter(typeof(SnakeCaseEnumJsonConverter<LadderType>))]
    private enum LadderType : byte
    {
        Atb,
        Atl,
        Batb,
        Batl,
        Bdatb,
        Bdatl,
        Trd,
        Spb,
        Spl,
    }

    /// <summary>Gets the last clock token as a span (zero-allocation access).</summary>
    public ReadOnlySpan<byte> ClockBytes => _clockBuffer.AsSpan(0, _clockLength);

    /// <summary>Gets the last clock token as a string (allocates on access).</summary>
    public string? Clock => _clockLength > 0 ? System.Text.Encoding.UTF8.GetString(_clockBuffer, 0, _clockLength) : null;

    /// <summary>Gets the initial clock token as a span (zero-allocation access).</summary>
    public ReadOnlySpan<byte> InitialClockBytes => _initialClockBuffer.AsSpan(0, _initialClockLength);

    /// <summary>Gets the initial clock token as a string (allocates on access).</summary>
    public string? InitialClock => _initialClockLength > 0 ? System.Text.Encoding.UTF8.GetString(_initialClockBuffer, 0, _initialClockLength) : null;

    /// <summary>Gets the last publish time.</summary>
    public long PublishTime { get; private set; }

    /// <summary>Gets the processing duration of the last message (line-ready to Process complete).</summary>
    public TimeSpan LastProcessingTime { get; private set; }

    /// <summary>Gets all market caches.</summary>
    public IReadOnlyDictionary<string, MarketCache> Markets
    {
        get
        {
            if (_multiMarkets != null) return _multiMarkets;
            if (_singleMarket != null)
            {
                _multiMarkets = new Dictionary<string, MarketCache>(1) { [_singleMarket.MarketId] = _singleMarket };
                return _multiMarkets;
            }

            return new Dictionary<string, MarketCache>(0);
        }
    }

    // Property name bytes — only keep those still needed for ValueTextEquals
    private static ReadOnlySpan<byte> PropOp => "op"u8;

    private static ReadOnlySpan<byte> PropId => "id"u8;

    private static ReadOnlySpan<byte> PropClk => "clk"u8;

    private static ReadOnlySpan<byte> PropInitialClk => "initialClk"u8;

    private static ReadOnlySpan<byte> PropPt => "pt"u8;

    private static ReadOnlySpan<byte> PropCt => "ct"u8;

    private static ReadOnlySpan<byte> PropMc => "mc"u8;

    private static ReadOnlySpan<byte> PropRc => "rc"u8;

    private static ReadOnlySpan<byte> PropTv => "tv"u8;

    private static ReadOnlySpan<byte> PropImg => "img"u8;

    private static ReadOnlySpan<byte> PropMarketDefinition => "marketDefinition"u8;

    private static ReadOnlySpan<byte> PropCon => "con"u8;

    private static ReadOnlySpan<byte> PropConflateMs => "conflateMs"u8;

    private static ReadOnlySpan<byte> PropHeartbeatMs => "heartbeatMs"u8;

    private static ReadOnlySpan<byte> PropOc => "oc"u8;

    private static ReadOnlySpan<byte> OpMcm => "mcm"u8;

    /// <summary>Gets a market cache by ID, or null if not present.</summary>
    public MarketCache? GetMarket(string marketId)
    {
        if (_singleMarket != null && string.Equals(_singleMarket.MarketId, marketId, StringComparison.Ordinal))
            return _singleMarket;
        return _multiMarkets?.GetValueOrDefault(marketId);
    }

    /// <summary>
    /// Processes a single line of raw stream bytes, updating the market caches in-place.
    /// Zero allocation for typical delta messages.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    public void Process(ReadOnlySpan<byte> data)
    {
        long start = Stopwatch.GetTimestamp();

        var reader = new Utf8JsonReader(data);

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
        {
            LastProcessingTime = Stopwatch.GetElapsedTime(start);
            return;
        }

        long pt = 0;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(PropOp))
            {
                reader.Read();
                if (reader.TokenType == JsonTokenType.String && !reader.ValueTextEquals(OpMcm))
                    return;
            }
            else if (reader.ValueTextEquals(PropPt))
            {
                reader.Read();
                pt = reader.GetInt64();
                PublishTime = pt;
            }
            else if (reader.ValueTextEquals(PropClk))
            {
                reader.Read();
                CopyTokenToBuffer(ref reader, ref _clockBuffer, out _clockLength);
            }
            else if (reader.ValueTextEquals(PropMc))
            {
                ReadMarketChanges(ref reader, pt);
            }
            else if (reader.ValueTextEquals(PropInitialClk))
            {
                reader.Read();
                CopyTokenToBuffer(ref reader, ref _initialClockBuffer, out _initialClockLength);
            }
            else if (reader.ValueTextEquals(PropId))
            {
                reader.Read();
            }
            else if (reader.ValueTextEquals(PropCt))
            {
                reader.Read();
            }
            else if (reader.ValueTextEquals(PropConflateMs))
            {
                reader.Read();
            }
            else if (reader.ValueTextEquals(PropHeartbeatMs))
            {
                reader.Read();
            }
            else if (reader.ValueTextEquals(PropOc))
            {
                SkipValue(ref reader);
            }
            else
            {
                SkipValue(ref reader);
            }
        }

        LastProcessingTime = Stopwatch.GetElapsedTime(start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SkipValue(ref Utf8JsonReader reader)
    {
        reader.Read();
        if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
            reader.Skip();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CopyTokenToBuffer(ref Utf8JsonReader reader, ref byte[] buffer, out int length)
    {
        if (reader.HasValueSequence)
        {
            var seq = reader.ValueSequence;
            length = (int)seq.Length;
            if (length > buffer.Length)
                buffer = new byte[length];
            seq.CopyTo(buffer);
        }
        else
        {
            var span = reader.ValueSpan;
            length = span.Length;
            if (length > buffer.Length)
                buffer = new byte[length];
            span.CopyTo(buffer);
        }
    }

    private static void ReadMarketDefinition(ref Utf8JsonReader reader, MarketCache? cache)
    {
        reader.Read();
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            if (cache != null)
            {
                cache.Definition.ReadFrom(ref reader);
            }
            else
            {
                reader.Skip();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadMarketChanges(ref Utf8JsonReader reader, long publishTime)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
        {
            ReadSingleMarketChange(ref reader, publishTime);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private void ReadSingleMarketChange(ref Utf8JsonReader reader, long publishTime)
    {
        MarketCache? cache = null;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(PropId))
            {
                reader.Read();
                cache = ResolveMarket(ref reader);
                if (cache != null) cache.PublishTime = publishTime;
            }
            else if (reader.ValueTextEquals(PropRc))
            {
                ReadRunnerChanges(ref reader, cache);
            }
            else if (reader.ValueTextEquals(PropTv))
            {
                reader.Read();
                if (cache != null)
                    cache.TotalMatched = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropImg))
            {
                reader.Read();
                if (cache != null && reader.GetBoolean())
                {
                    cache.IsImage = true;
                    cache.ClearRunners();
                }
            }
            else if (reader.ValueTextEquals(PropCon))
            {
                reader.Read();
            }
            else if (reader.ValueTextEquals(PropMarketDefinition))
            {
                ReadMarketDefinition(ref reader, cache);
            }
            else
            {
                SkipValue(ref reader);
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Fast-path market lookup — branches are necessary for performance.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MarketCache ResolveMarket(ref Utf8JsonReader reader)
    {
        // Fast path: single market — compare against pre-computed bytes, no allocation
        if (_singleMarket != null && reader.ValueTextEquals(_singleMarket.MarketIdBytes))
            return _singleMarket;

        // Need the string for multi-market lookup or first-time creation
        var marketId = reader.GetString() ?? string.Empty;

        if (_singleMarket == null)
        {
            _singleMarket = new MarketCache(marketId);
            return _singleMarket;
        }

        if (string.Equals(_singleMarket.MarketId, marketId, StringComparison.Ordinal))
            return _singleMarket;

        // Multi-market path
        _multiMarkets ??= new Dictionary<string, MarketCache>(4) { [_singleMarket.MarketId] = _singleMarket };

        if (_multiMarkets.TryGetValue(marketId, out var market))
            return market;

        market = new MarketCache(marketId);
        _multiMarkets[marketId] = market;
        return market;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadRunnerChanges(ref Utf8JsonReader reader, MarketCache? cache)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
        {
            ReadSingleRunnerChange(ref reader, cache);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "First-byte dispatch for runner properties — performance-critical hot path.")]
    private void ReadSingleRunnerChange(ref Utf8JsonReader reader, MarketCache? cache)
    {
        long selectionId = 0;
        double handicap = 0;
        double ltp = double.NaN;
        double tv = double.NaN;
        double spn = double.NaN;
        double spf = double.NaN;
        int deferredCount = 0;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            // First-byte dispatch: reduces average comparisons from ~5 to ~1-2
            var propSpan = reader.ValueSpan;
            if (propSpan.Length == 0)
            {
                SkipValue(ref reader);
                continue;
            }

            switch (propSpan[0])
            {
                case (byte)'a': // atb, atl (highest frequency on deltas)
                    if (propSpan.Length == 3)
                    {
                        if (propSpan[2] == (byte)'b')
                            ReadPriceSizePairs(ref reader, LadderType.Atb, ref deferredCount);
                        else
                            ReadPriceSizePairs(ref reader, LadderType.Atl, ref deferredCount);
                    }
                    else
                    {
                        SkipValue(ref reader);
                    }

                    break;

                case (byte)'i': // id
                    reader.Read();
                    selectionId = reader.GetInt64();
                    break;

                case (byte)'l': // ltp
                    reader.Read();
                    ltp = reader.GetDouble();
                    break;

                case (byte)'t': // tv, trd
                    if (propSpan.Length == 2)
                    {
                        reader.Read();
                        tv = reader.GetDouble();
                    }
                    else
                    {
                        ReadPriceSizePairs(ref reader, LadderType.Trd, ref deferredCount);
                    }

                    break;

                case (byte)'b': // batb, batl, bdatb, bdatl
                    if (propSpan.Length == 4)
                    {
                        // batb or batl — distinguish by last byte
                        if (propSpan[3] == (byte)'b')
                            ReadPositionPriceSizeTriples(ref reader, LadderType.Batb, ref deferredCount);
                        else
                            ReadPositionPriceSizeTriples(ref reader, LadderType.Batl, ref deferredCount);
                    }
                    else
                    {
                        // bdatb or bdatl
                        if (propSpan[4] == (byte)'b')
                            ReadPositionPriceSizeTriples(ref reader, LadderType.Bdatb, ref deferredCount);
                        else
                            ReadPositionPriceSizeTriples(ref reader, LadderType.Bdatl, ref deferredCount);
                    }

                    break;

                case (byte)'s': // spb, spl, spn, spf
                    if (propSpan.Length == 3 && propSpan[2] == (byte)'n')
                    {
                        reader.Read();
                        spn = reader.GetDouble();
                    }
                    else if (propSpan.Length == 3 && propSpan[2] == (byte)'f')
                    {
                        reader.Read();
                        spf = reader.GetDouble();
                    }
                    else if (propSpan.Length == 3 && propSpan[2] == (byte)'b')
                    {
                        ReadPriceSizePairs(ref reader, LadderType.Spb, ref deferredCount);
                    }
                    else if (propSpan.Length == 3 && propSpan[2] == (byte)'l')
                    {
                        ReadPriceSizePairs(ref reader, LadderType.Spl, ref deferredCount);
                    }
                    else
                    {
                        SkipValue(ref reader);
                    }

                    break;

                case (byte)'h': // hc
                    reader.Read();
                    handicap = reader.GetDouble();
                    break;

                default:
                    SkipValue(ref reader);
                    break;
            }
        }

        if (cache == null || selectionId == 0)
            return;

        var runner = cache.GetOrAddRunner(selectionId, handicap);

        if (!double.IsNaN(ltp)) runner.LastTradedPrice = ltp;
        if (!double.IsNaN(tv)) runner.TotalMatched = tv;
        if (!double.IsNaN(spn)) runner.StartingPriceNear = spn;
        if (!double.IsNaN(spf)) runner.StartingPriceFar = spf;

        ApplyDeferredUpdates(runner, deferredCount);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Flat switch dispatch over ladder types — linear and readable.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ApplyDeferredUpdates(RunnerCache runner, int count)
    {
        for (int i = 0; i < count; i++)
        {
            ref var u = ref _deferredBuffer[i];
            switch (u.Type)
            {
                case LadderType.Atb:
                    runner.AvailableToBack.Update(u.Price, u.Size);
                    break;
                case LadderType.Atl:
                    runner.AvailableToLay.Update(u.Price, u.Size);
                    break;
                case LadderType.Batb:
                    runner.BestAvailableToBack.Update((int)u.Position, u.Price, u.Size);
                    break;
                case LadderType.Batl:
                    runner.BestAvailableToLay.Update((int)u.Position, u.Price, u.Size);
                    break;
                case LadderType.Bdatb:
                    runner.BestDisplayAvailableToBack.Update((int)u.Position, u.Price, u.Size);
                    break;
                case LadderType.Bdatl:
                    runner.BestDisplayAvailableToLay.Update((int)u.Position, u.Price, u.Size);
                    break;
                case LadderType.Trd:
                    runner.Traded.Update(u.Price, u.Size);
                    break;
                case LadderType.Spb:
                    runner.StartingPriceBack.Update(u.Price, u.Size);
                    break;
                default: // Spl
                    runner.StartingPriceLay.Update(u.Price, u.Size);
                    break;
            }
        }
    }

    /// <summary>
    /// Reads a [[price, size], [price, size], ...] array directly into the deferred buffer.
    /// Inlined tight loop — no method calls per element.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Simple array parsing loop with bounds growth.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadPriceSizePairs(ref Utf8JsonReader reader, LadderType type, ref int deferredCount)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Read();
            double price = reader.GetDouble();
            reader.Read();
            double size = reader.GetDouble();

            // Skip any trailing elements in the inner array
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                // Intentionally empty — consume remaining tokens in inner array
            }

            if (deferredCount >= _deferredBuffer.Length)
                Array.Resize(ref _deferredBuffer, _deferredBuffer.Length * 2);

            ref var slot = ref _deferredBuffer[deferredCount];
            slot.Type = type;
            slot.Price = price;
            slot.Size = size;
            deferredCount++;
        }
    }

    /// <summary>
    /// Reads a [[position, price, size], ...] array directly into the deferred buffer.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Simple array parsing loop with bounds growth.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadPositionPriceSizeTriples(ref Utf8JsonReader reader, LadderType type, ref int deferredCount)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Read();
            double position = reader.GetDouble();
            reader.Read();
            double price = reader.GetDouble();
            reader.Read();
            double size = reader.GetDouble();

            // Skip any trailing elements in the inner array
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                // Intentionally empty — consume remaining tokens in inner array
            }

            if (deferredCount >= _deferredBuffer.Length)
                Array.Resize(ref _deferredBuffer, _deferredBuffer.Length * 2);

            ref var slot = ref _deferredBuffer[deferredCount];
            slot.Type = type;
            slot.Price = price;
            slot.Size = size;
            slot.Position = position;
            deferredCount++;
        }
    }

    private struct DeferredLadderUpdate
    {
        public LadderType Type;
        public double Price;
        public double Size;
        public double Position;
    }
}
