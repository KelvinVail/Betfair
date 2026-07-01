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

    private static ReadOnlySpan<byte> PropMc => "mc"u8;

    private static ReadOnlySpan<byte> PropRc => "rc"u8;

    private static ReadOnlySpan<byte> PropTv => "tv"u8;

    private static ReadOnlySpan<byte> PropImg => "img"u8;

    private static ReadOnlySpan<byte> PropMarketDefinition => "marketDefinition"u8;

    private static ReadOnlySpan<byte> PropCon => "con"u8;

    private static ReadOnlySpan<byte> OpMcm => "mcm"u8;

    /// <summary>Gets a market cache by ID, or null if not present.</summary>
    /// <param name="marketId">The market identifier to look up.</param>
    /// <returns>The <see cref="MarketCache"/> for the given market, or null if not found.</returns>
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
    /// <param name="data">The raw bytes of a single stream message line.</param>
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
            if (!TryProcessProperty(ref reader, ref pt))
            {
                LastProcessingTime = Stopwatch.GetElapsedTime(start);
                return;
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

    private static void ReadTotalVolume(ref Utf8JsonReader reader, MarketCache? cache)
    {
        reader.Read();
        if (cache != null)
            cache.TotalMatched = reader.GetDouble();
    }

    private static bool ReadImageFlag(ref Utf8JsonReader reader, MarketCache? cache, bool runnersProcessed)
    {
        reader.Read();
        if (!reader.GetBoolean()) return false;

        if (cache != null)
        {
            cache.IsImage = true;
            if (!runnersProcessed)
                cache.ClearRunners();
        }

        return true;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private bool TryProcessProperty(ref Utf8JsonReader reader, ref long pt)
    {
        if (reader.ValueTextEquals(PropOp))
        {
            reader.Read();
            return reader.TokenType != JsonTokenType.String || reader.ValueTextEquals(OpMcm);
        }

        if (reader.ValueTextEquals(PropPt))
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
        else
        {
            SkipValue(ref reader);
        }

        return true;
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
        bool isImage = false;
        bool runnersProcessed = false;

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
                ProcessRunnerChanges(ref reader, cache, isImage, ref runnersProcessed);
            }
            else if (reader.ValueTextEquals(PropTv))
            {
                ReadTotalVolume(ref reader, cache);
            }
            else if (reader.ValueTextEquals(PropImg))
            {
                isImage = ReadImageFlag(ref reader, cache, runnersProcessed);
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

    private void ProcessRunnerChanges(ref Utf8JsonReader reader, MarketCache? cache, bool isImage, ref bool runnersProcessed)
    {
        if (isImage && cache != null)
            cache.ClearRunners();

        ReadRunnerChanges(ref reader, cache);
        runnersProcessed = true;
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
        var state = RunnerChangeState.Create();

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            var propSpan = reader.ValueSpan;
            if (propSpan.Length == 0)
            {
                SkipValue(ref reader);
                continue;
            }

            DispatchRunnerProperty(ref reader, propSpan, ref state);
        }

        if (cache == null || state.SelectionId == 0)
            return;

        var runner = cache.GetOrAddRunner(state.SelectionId, state.Handicap);

        if (!double.IsNaN(state.Ltp)) runner.LastTradedPrice = state.Ltp;
        if (!double.IsNaN(state.Tv)) runner.TotalMatched = state.Tv;
        if (!double.IsNaN(state.Spn)) runner.StartingPriceNear = state.Spn;
        if (!double.IsNaN(state.Spf)) runner.StartingPriceFar = state.Spf;

        ApplyDeferredUpdates(runner, state.DeferredCount);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "First-byte dispatch for runner properties — performance-critical hot path.")]
    private void DispatchRunnerProperty(
        ref Utf8JsonReader reader,
        ReadOnlySpan<byte> propSpan,
        ref RunnerChangeState state)
    {
        switch (propSpan[0])
        {
            case (byte)'a':
                ReadAtbOrAtl(ref reader, propSpan, ref state.DeferredCount);
                break;
            case (byte)'i':
                reader.Read();
                state.SelectionId = reader.GetInt64();
                break;
            case (byte)'l':
                reader.Read();
                state.Ltp = reader.GetDouble();
                break;
            case (byte)'t':
                ReadTvOrTrd(ref reader, propSpan, ref state.Tv, ref state.DeferredCount);
                break;
            case (byte)'b':
                ReadBestAvailable(ref reader, propSpan, ref state.DeferredCount);
                break;
            case (byte)'s':
                ReadStartingPrice(ref reader, propSpan, ref state.Spn, ref state.Spf, ref state.DeferredCount);
                break;
            case (byte)'h':
                reader.Read();
                state.Handicap = reader.GetDouble();
                break;
            default:
                SkipValue(ref reader);
                break;
        }
    }

    private void ReadAtbOrAtl(ref Utf8JsonReader reader, ReadOnlySpan<byte> propSpan, ref int deferredCount)
    {
        if (propSpan.Length == 3)
        {
            var type = propSpan[2] == (byte)'b' ? LadderType.Atb : LadderType.Atl;
            ReadPriceSizePairs(ref reader, type, ref deferredCount);
        }
        else
        {
            SkipValue(ref reader);
        }
    }

    private void ReadTvOrTrd(ref Utf8JsonReader reader, ReadOnlySpan<byte> propSpan, ref double tv, ref int deferredCount)
    {
        if (propSpan.Length == 2)
        {
            reader.Read();
            tv = reader.GetDouble();
        }
        else
        {
            ReadPriceSizePairs(ref reader, LadderType.Trd, ref deferredCount);
        }
    }

    private void ReadBestAvailable(ref Utf8JsonReader reader, ReadOnlySpan<byte> propSpan, ref int deferredCount)
    {
        if (propSpan.Length == 4)
        {
            var type = propSpan[3] == (byte)'b' ? LadderType.Batb : LadderType.Batl;
            ReadPositionPriceSizeTriples(ref reader, type, ref deferredCount);
        }
        else
        {
            var type = propSpan[4] == (byte)'b' ? LadderType.Bdatb : LadderType.Bdatl;
            ReadPositionPriceSizeTriples(ref reader, type, ref deferredCount);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Flat switch over starting price properties.")]
    private void ReadStartingPrice(ref Utf8JsonReader reader, ReadOnlySpan<byte> propSpan, ref double spn, ref double spf, ref int deferredCount)
    {
        if (propSpan.Length != 3)
        {
            SkipValue(ref reader);
            return;
        }

        switch (propSpan[2])
        {
            case (byte)'n':
                reader.Read();
                spn = reader.GetDouble();
                break;
            case (byte)'f':
                reader.Read();
                spf = reader.GetDouble();
                break;
            case (byte)'b':
                ReadPriceSizePairs(ref reader, LadderType.Spb, ref deferredCount);
                break;
            case (byte)'l':
                ReadPriceSizePairs(ref reader, LadderType.Spl, ref deferredCount);
                break;
            default:
                SkipValue(ref reader);
                break;
        }
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

    /// <summary>
    /// Groups the mutable state accumulated while parsing a single runner change,
    /// reducing the parameter count of <see cref="DispatchRunnerProperty"/>.
    /// </summary>
    private struct RunnerChangeState
    {
        public long SelectionId;
        public double Handicap;
        public double Ltp;
        public double Tv;
        public double Spn;
        public double Spf;
        public int DeferredCount;

        public static RunnerChangeState Create() => new RunnerChangeState
        {
            SelectionId = 0,
            Handicap = 0,
            Ltp = double.NaN,
            Tv = double.NaN,
            Spn = double.NaN,
            Spf = double.NaN,
            DeferredCount = 0,
        };
    }
}
