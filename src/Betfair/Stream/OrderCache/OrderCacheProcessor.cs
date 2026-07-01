using System.Diagnostics;
using Betfair.Core.Enums;
using Betfair.Stream.MarketCache;

namespace Betfair.Stream.OrderCache;

/// <summary>
/// Ultra-low-latency processor that reads raw stream bytes directly into
/// <see cref="OrderCache"/> instances using <see cref="Utf8JsonReader"/>.
/// Zero allocation on steady-state delta messages.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Private static property-name fields grouped together for readability.")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Static helpers grouped near their call sites for readability.")]
public sealed class OrderCacheProcessor
{
    private OrderCache? _singleMarket;
    private Dictionary<string, OrderCache>? _multiMarkets;

    private byte[] _clockBuffer = new byte[64];
    private int _clockLength;
    private byte[] _initialClockBuffer = new byte[64];
    private int _initialClockLength;

    // Top-level message property names
    private static ReadOnlySpan<byte> PropOp => "op"u8;

    private static ReadOnlySpan<byte> PropPt => "pt"u8;

    private static ReadOnlySpan<byte> PropClk => "clk"u8;

    private static ReadOnlySpan<byte> PropInitialClk => "initialClk"u8;

    private static ReadOnlySpan<byte> PropOc => "oc"u8;

    private static ReadOnlySpan<byte> OpOcm => "ocm"u8;

    // Order change property names
    private static ReadOnlySpan<byte> PropId => "id"u8;

    private static ReadOnlySpan<byte> PropAccountId => "accountId"u8;

    private static ReadOnlySpan<byte> PropClosed => "closed"u8;

    private static ReadOnlySpan<byte> PropOrc => "orc"u8;

    // Order runner change property names
    private static ReadOnlySpan<byte> PropHc => "hc"u8;

    private static ReadOnlySpan<byte> PropFullImage => "fullImage"u8;

    private static ReadOnlySpan<byte> PropMb => "mb"u8;

    private static ReadOnlySpan<byte> PropMl => "ml"u8;

    private static ReadOnlySpan<byte> PropUo => "uo"u8;

    private static ReadOnlySpan<byte> PropSmc => "smc"u8;

    // Unmatched order property names
    private static ReadOnlySpan<byte> PropP => "p"u8;

    private static ReadOnlySpan<byte> PropS => "s"u8;

    private static ReadOnlySpan<byte> PropBsp => "bsp"u8;

    private static ReadOnlySpan<byte> PropSide => "side"u8;

    private static ReadOnlySpan<byte> PropStatus => "status"u8;

    private static ReadOnlySpan<byte> PropPtOrder => "pt"u8;

    private static ReadOnlySpan<byte> PropOt => "ot"u8;

    private static ReadOnlySpan<byte> PropPd => "pd"u8;

    private static ReadOnlySpan<byte> PropMd => "md"u8;

    private static ReadOnlySpan<byte> PropCd => "cd"u8;

    private static ReadOnlySpan<byte> PropLd => "ld"u8;

    private static ReadOnlySpan<byte> PropLsrc => "lsrc"u8;

    private static ReadOnlySpan<byte> PropAvp => "avp"u8;

    private static ReadOnlySpan<byte> PropSm => "sm"u8;

    private static ReadOnlySpan<byte> PropSr => "sr"u8;

    private static ReadOnlySpan<byte> PropSl => "sl"u8;

    private static ReadOnlySpan<byte> PropSc => "sc"u8;

    private static ReadOnlySpan<byte> PropSv => "sv"u8;

    private static ReadOnlySpan<byte> PropRac => "rac"u8;

    private static ReadOnlySpan<byte> PropRc => "rc"u8;

    private static ReadOnlySpan<byte> PropRfo => "rfo"u8;

    private static ReadOnlySpan<byte> PropRfs => "rfs"u8;

    /// <summary>Gets the last clock token as a span (zero-allocation access).</summary>
    public ReadOnlySpan<byte> ClockBytes => _clockBuffer.AsSpan(0, _clockLength);

    /// <summary>Gets the last clock token as a string (allocates on access).</summary>
    public string? Clock => _clockLength > 0
        ? System.Text.Encoding.UTF8.GetString(_clockBuffer, 0, _clockLength)
        : null;

    /// <summary>Gets the initial clock token as a span (zero-allocation access).</summary>
    public ReadOnlySpan<byte> InitialClockBytes => _initialClockBuffer.AsSpan(0, _initialClockLength);

    /// <summary>Gets the initial clock token as a string (allocates on access).</summary>
    public string? InitialClock => _initialClockLength > 0
        ? System.Text.Encoding.UTF8.GetString(_initialClockBuffer, 0, _initialClockLength)
        : null;

    /// <summary>Gets the last publish time.</summary>
    public long PublishTime { get; private set; }

    /// <summary>Gets the processing duration of the last message.</summary>
    public TimeSpan LastProcessingTime { get; private set; }

    /// <summary>Gets all order market caches.</summary>
    public IReadOnlyDictionary<string, OrderCache> Markets
    {
        get
        {
            if (_multiMarkets != null) return _multiMarkets;
            if (_singleMarket != null)
            {
                _multiMarkets = new Dictionary<string, OrderCache>(1)
                    { [_singleMarket.MarketId] = _singleMarket };
                return _multiMarkets;
            }

            return new Dictionary<string, OrderCache>(0);
        }
    }

    /// <summary>Gets an order market cache by ID, or null if not present.</summary>
    public OrderCache? GetMarket(string marketId)
    {
        if (_singleMarket != null
            && string.Equals(_singleMarket.MarketId, marketId, StringComparison.Ordinal))
            return _singleMarket;
        return _multiMarkets?.GetValueOrDefault(marketId);
    }

    /// <summary>
    /// Processes a single line of raw stream bytes, updating order caches in-place.
    /// Zero allocation for typical delta messages.
    /// </summary>
    [SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
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

    [SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private bool TryProcessProperty(ref Utf8JsonReader reader, ref long pt)
    {
        if (reader.ValueTextEquals(PropOp))
        {
            reader.Read();
            return reader.TokenType != JsonTokenType.String || reader.ValueTextEquals(OpOcm);
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
        else if (reader.ValueTextEquals(PropOc))
        {
            ReadOrderChanges(ref reader, pt);
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
    private void ReadOrderChanges(ref Utf8JsonReader reader, long publishTime)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
        {
            ReadSingleOrderChange(ref reader, publishTime);
        }
    }

    [SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    [SuppressMessage("Major Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private void ReadSingleOrderChange(ref Utf8JsonReader reader, long publishTime)
    {
        OrderCache? cache = null;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(PropId))
            {
                reader.Read();
                cache = ResolveMarket(ref reader);
                if (cache != null) cache.PublishTime = publishTime;
            }
            else if (reader.ValueTextEquals(PropAccountId))
            {
                reader.Read();
                if (cache != null) cache.AccountId = reader.GetInt64();
            }
            else if (reader.ValueTextEquals(PropClosed))
            {
                reader.Read();
                if (cache != null) cache.Closed = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropOrc))
            {
                ReadOrderRunnerChanges(ref reader, cache);
            }
            else
            {
                SkipValue(ref reader);
            }
        }
    }

    [SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Fast-path market lookup — branches are necessary for performance.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private OrderCache ResolveMarket(ref Utf8JsonReader reader)
    {
        if (_singleMarket != null && reader.ValueTextEquals(_singleMarket.MarketIdBytes))
            return _singleMarket;

        var marketId = reader.GetString() ?? string.Empty;

        if (_singleMarket == null)
        {
            _singleMarket = new OrderCache(marketId);
            return _singleMarket;
        }

        if (string.Equals(_singleMarket.MarketId, marketId, StringComparison.Ordinal))
            return _singleMarket;

        _multiMarkets ??= new Dictionary<string, OrderCache>(4)
            { [_singleMarket.MarketId] = _singleMarket };

        if (_multiMarkets.TryGetValue(marketId, out var market))
            return market;

        market = new OrderCache(marketId);
        _multiMarkets[marketId] = market;
        return market;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReadOrderRunnerChanges(ref Utf8JsonReader reader, OrderCache? cache)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
        {
            ReadSingleOrderRunnerChange(ref reader, cache);
        }
    }

    [SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    [SuppressMessage("Major Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private static void ReadSingleOrderRunnerChange(ref Utf8JsonReader reader, OrderCache? cache)
    {
        long selectionId = 0;
        double handicap = 0;
        bool fullImage = false;
        OrderRunnerCache? runner = null;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(PropId))
            {
                reader.Read();
                selectionId = reader.GetInt64();
                if (cache != null)
                {
                    runner = cache.GetOrAddRunner(selectionId, handicap);
                    if (fullImage) runner.Clear();
                }
            }
            else if (reader.ValueTextEquals(PropHc))
            {
                reader.Read();
                handicap = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropFullImage))
            {
                reader.Read();
                fullImage = reader.GetBoolean();
                if (fullImage && runner != null)
                    runner.Clear();
            }
            else if (reader.ValueTextEquals(PropMb))
            {
                ReadPriceSizePairsIntoLadder(ref reader, runner?.MatchedBacks);
            }
            else if (reader.ValueTextEquals(PropMl))
            {
                ReadPriceSizePairsIntoLadder(ref reader, runner?.MatchedLays);
            }
            else if (reader.ValueTextEquals(PropUo))
            {
                ReadUnmatchedOrders(ref reader, runner);
            }
            else if (reader.ValueTextEquals(PropSmc))
            {
                ReadStrategyMatchChanges(ref reader, runner);
            }
            else
            {
                SkipValue(ref reader);
            }
        }
    }

    [SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Simple array parsing loop.")]
    private static void ReadPriceSizePairsIntoLadder(ref Utf8JsonReader reader, PriceLadder? ladder)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
        {
            reader.Read();
            double price = reader.GetDouble();
            reader.Read();
            double size = reader.GetDouble();

            // Consume any trailing elements in the inner array
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                // Intentionally empty — consume remaining tokens in inner array
            }

            ladder?.Update(price, size);
        }
    }

    private static void ReadUnmatchedOrders(ref Utf8JsonReader reader, OrderRunnerCache? runner)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
        {
            ReadSingleUnmatchedOrder(ref reader, runner);
        }
    }

    [SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch for order fields.")]
    [SuppressMessage("Major Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private static void ReadSingleUnmatchedOrder(ref Utf8JsonReader reader, OrderRunnerCache? runner)
    {
        UnmatchedOrderCache? order = null;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(PropId))
            {
                reader.Read();
                if (runner != null)
                    order = runner.GetOrAddOrder(ref reader);
            }
            else if (reader.ValueTextEquals(PropP))
            {
                reader.Read();
                if (order != null) order.Price = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropS))
            {
                reader.Read();
                if (order != null) order.Size = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropBsp))
            {
                reader.Read();
                if (order != null) order.BspLiability = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropSide))
            {
                reader.Read();
                if (order != null) order.Side = ParseSide(ref reader);
            }
            else if (reader.ValueTextEquals(PropStatus))
            {
                reader.Read();
                if (order != null) order.Status = ParseStatus(ref reader);
            }
            else if (reader.ValueTextEquals(PropPtOrder))
            {
                reader.Read();
                if (order != null) order.PersistenceType = ParsePersistenceType(ref reader);
            }
            else if (reader.ValueTextEquals(PropOt))
            {
                reader.Read();
                if (order != null) order.OrderType = ParseOrderType(ref reader);
            }
            else
            {
                ReadRemainingOrderProperties(ref reader, order);
            }
        }
    }

    [SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch for order date/size fields.")]
    [SuppressMessage("Major Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private static void ReadRemainingOrderProperties(ref Utf8JsonReader reader, UnmatchedOrderCache? order)
    {
        if (reader.ValueTextEquals(PropPd))
        {
            reader.Read();
            if (order != null) order.PlacedDate = reader.GetInt64();
        }
        else if (reader.ValueTextEquals(PropMd))
        {
            reader.Read();
            if (order != null) order.MatchedDate = reader.GetInt64();
        }
        else if (reader.ValueTextEquals(PropCd))
        {
            reader.Read();
            if (order != null) order.CancelledDate = reader.GetInt64();
        }
        else if (reader.ValueTextEquals(PropLd))
        {
            reader.Read();
            if (order != null) order.LapsedDate = reader.GetInt64();
        }
        else if (reader.ValueTextEquals(PropLsrc))
        {
            reader.Read();
            if (order != null) order.SetLapsedStatusReasonCode(ref reader);
        }
        else if (reader.ValueTextEquals(PropAvp))
        {
            reader.Read();
            if (order != null) order.AveragePriceMatched = reader.GetDouble();
        }
        else if (reader.ValueTextEquals(PropSm))
        {
            reader.Read();
            if (order != null) order.SizeMatched = reader.GetDouble();
        }
        else if (reader.ValueTextEquals(PropSr))
        {
            reader.Read();
            if (order != null) order.SizeRemaining = reader.GetDouble();
        }
        else if (reader.ValueTextEquals(PropSl))
        {
            reader.Read();
            if (order != null) order.SizeLapsed = reader.GetDouble();
        }
        else if (reader.ValueTextEquals(PropSc))
        {
            reader.Read();
            if (order != null) order.SizeCancelled = reader.GetDouble();
        }
        else if (reader.ValueTextEquals(PropSv))
        {
            reader.Read();
            if (order != null) order.SizeVoided = reader.GetDouble();
        }
        else if (reader.ValueTextEquals(PropRac))
        {
            reader.Read();
            if (order != null) order.SetRegulatorAuthCode(ref reader);
        }
        else if (reader.ValueTextEquals(PropRc))
        {
            reader.Read();
            if (order != null) order.SetRegulatorCode(ref reader);
        }
        else if (reader.ValueTextEquals(PropRfo))
        {
            reader.Read();
            if (order != null) order.SetOrderReference(ref reader);
        }
        else if (reader.ValueTextEquals(PropRfs))
        {
            reader.Read();
            if (order != null) order.SetStrategyReference(ref reader);
        }
        else
        {
            SkipValue(ref reader);
        }
    }

    [SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch for strategy match changes.")]
    [SuppressMessage("Major Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private static void ReadStrategyMatchChanges(ref Utf8JsonReader reader, OrderRunnerCache? runner)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            var strategyCache = runner?.GetOrAddStrategy(ref reader);

            // Read the strategy object { "mb": [...], "ml": [...] }
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            {
                if (reader.TokenType is JsonTokenType.StartArray)
                    reader.Skip();
                continue;
            }

            while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
            {
                if (reader.ValueTextEquals(PropMb))
                {
                    ReadPriceSizePairsIntoLadder(ref reader, strategyCache?.MatchedBacks);
                }
                else if (reader.ValueTextEquals(PropMl))
                {
                    ReadPriceSizePairsIntoLadder(ref reader, strategyCache?.MatchedLays);
                }
                else
                {
                    SkipValue(ref reader);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Side ParseSide(ref Utf8JsonReader reader)
    {
        if (reader.ValueTextEquals("B"u8)) return Side.Back;
        if (reader.ValueTextEquals("L"u8)) return Side.Lay;
        return Side.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static OrderStatus ParseStatus(ref Utf8JsonReader reader)
    {
        if (reader.ValueTextEquals("E"u8)) return OrderStatus.Executable;
        if (reader.ValueTextEquals("EC"u8)) return OrderStatus.ExecutionComplete;
        return OrderStatus.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PersistenceType ParsePersistenceType(ref Utf8JsonReader reader)
    {
        if (reader.ValueTextEquals("L"u8)) return PersistenceType.Lapse;
        if (reader.ValueTextEquals("P"u8)) return PersistenceType.Persist;
        if (reader.ValueTextEquals("MOC"u8)) return PersistenceType.MarketOnClose;
        return PersistenceType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static OrderType ParseOrderType(ref Utf8JsonReader reader)
    {
        if (reader.ValueTextEquals("L"u8)) return OrderType.Limit;
        if (reader.ValueTextEquals("LOC"u8)) return OrderType.LimitOnClose;
        if (reader.ValueTextEquals("MOC"u8)) return OrderType.MarketOnClose;
        return OrderType.Unknown;
    }
}
