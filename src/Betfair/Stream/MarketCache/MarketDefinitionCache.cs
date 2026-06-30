namespace Betfair.Stream.MarketCache;

/// <summary>
/// Mutable market definition updated in-place from stream bytes.
/// No allocation on updates when string values haven't changed.
/// Uses cached UTF-8 bytes for zero-allocation comparison.
/// </summary>
public sealed class MarketDefinitionCache
{
    // Reusable runner definitions list
    private readonly List<RunnerDefinitionCache> _runners = new (16);

    // Cached UTF-8 bytes for string fields — avoids Encoding.UTF8.GetBytes allocation on comparison
    private byte[]? _statusBytes;
    private byte[]? _bettingTypeBytes;
    private byte[]? _venueBytes;
    private byte[]? _timezoneBytes;
    private byte[]? _marketTypeBytes;
    private byte[]? _countryCodeBytes;
    private byte[]? _eventIdBytes;
    private byte[]? _eventTypeIdBytes;

    public bool? BspMarket { get; private set; }

    public bool? TurnInPlayEnabled { get; private set; }

    public bool? PersistenceEnabled { get; private set; }

    public double? MarketBaseRate { get; private set; }

    public string? BettingType { get; private set; }

    public string? Status { get; private set; }

    public string? Venue { get; private set; }

    public string? Timezone { get; private set; }

    public double? EachWayDivisor { get; private set; }

    public string? MarketType { get; private set; }

    public int? NumberOfWinners { get; private set; }

    public string? CountryCode { get; private set; }

    public bool? InPlay { get; private set; }

    public int? BetDelay { get; private set; }

    public int? NumberOfActiveRunners { get; private set; }

    public string? EventId { get; private set; }

    public string? EventTypeId { get; private set; }

    public bool? CrossMatching { get; private set; }

    public bool? RunnersVoidable { get; private set; }

    public bool? DiscountAllowed { get; private set; }

    public long? Version { get; private set; }

    public bool? Complete { get; private set; }

    public bool? BspReconciled { get; private set; }

    public IReadOnlyList<RunnerDefinitionCache> Runners => _runners;

    private static ReadOnlySpan<byte> PropBspMarket => "bspMarket"u8;

    private static ReadOnlySpan<byte> PropTurnInPlayEnabled => "turnInPlayEnabled"u8;

    private static ReadOnlySpan<byte> PropPersistenceEnabled => "persistenceEnabled"u8;

    private static ReadOnlySpan<byte> PropMarketBaseRate => "marketBaseRate"u8;

    private static ReadOnlySpan<byte> PropBettingType => "bettingType"u8;

    private static ReadOnlySpan<byte> PropStatus => "status"u8;

    private static ReadOnlySpan<byte> PropVenue => "venue"u8;

    private static ReadOnlySpan<byte> PropTimezone => "timezone"u8;

    private static ReadOnlySpan<byte> PropEachWayDivisor => "eachWayDivisor"u8;

    private static ReadOnlySpan<byte> PropMarketType => "marketType"u8;

    private static ReadOnlySpan<byte> PropNumberOfWinners => "numberOfWinners"u8;

    private static ReadOnlySpan<byte> PropCountryCode => "countryCode"u8;

    private static ReadOnlySpan<byte> PropInPlay => "inPlay"u8;

    private static ReadOnlySpan<byte> PropBetDelay => "betDelay"u8;

    private static ReadOnlySpan<byte> PropNumberOfActiveRunners => "numberOfActiveRunners"u8;

    private static ReadOnlySpan<byte> PropEventId => "eventId"u8;

    private static ReadOnlySpan<byte> PropCrossMatching => "crossMatching"u8;

    private static ReadOnlySpan<byte> PropRunnersVoidable => "runnersVoidable"u8;

    private static ReadOnlySpan<byte> PropDiscountAllowed => "discountAllowed"u8;

    private static ReadOnlySpan<byte> PropRunners => "runners"u8;

    private static ReadOnlySpan<byte> PropVersion => "version"u8;

    private static ReadOnlySpan<byte> PropEventTypeId => "eventTypeId"u8;

    private static ReadOnlySpan<byte> PropComplete => "complete"u8;

    private static ReadOnlySpan<byte> PropBspReconciled => "bspReconciled"u8;

    /// <summary>
    /// Reads and applies a market definition update from a Utf8JsonReader.
    /// The reader should be positioned at the StartObject token.
    /// Only allocates strings when values actually change.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader positioned at the start of the market definition object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Minimal dispatch loop — cannot be simplified further.")]
    internal void ReadFrom(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (!TryReadProperty(ref reader))
            {
                reader.Read();
                if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
                    reader.Skip();
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private static bool ReadBool(ref Utf8JsonReader reader, out bool value)
    {
        reader.Read();
        value = reader.GetBoolean();
        return true;
    }

    private static bool ReadInt(ref Utf8JsonReader reader, out int value)
    {
        reader.Read();
        value = reader.GetInt32();
        return true;
    }

    private static bool ReadLong(ref Utf8JsonReader reader, out long value)
    {
        reader.Read();
        value = reader.GetInt64();
        return true;
    }

    private static bool ReadDouble(ref Utf8JsonReader reader, out double value)
    {
        reader.Read();
        value = reader.GetDouble();
        return true;
    }

    private static bool ReadCachedStringField(ref Utf8JsonReader reader, ref byte[]? cachedBytes, Action<string> setter)
    {
        reader.Read();
        ReadCachedString(ref reader, ref cachedBytes, out var val);
        if (val != null) setter(val);
        return true;
    }

    /// <summary>
    /// Reads a string value using cached UTF-8 bytes for comparison.
    /// If the value matches the cached bytes, returns null (meaning "unchanged").
    /// If it differs, allocates a new string and updates the cache.
    /// </summary>
    private static void ReadCachedString(ref Utf8JsonReader reader, ref byte[]? cachedBytes, out string? newValue)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            cachedBytes = null;
            newValue = null;
            return;
        }

        // If we have cached bytes and they match, the value is unchanged — no allocation
        if (cachedBytes != null && reader.ValueTextEquals(cachedBytes))
        {
            newValue = null; // Signal: unchanged
            return;
        }

        // Value changed — allocate new string and cache its bytes
        newValue = reader.GetString();
        cachedBytes = newValue != null ? System.Text.Encoding.UTF8.GetBytes(newValue) : null;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private bool TryReadProperty(ref Utf8JsonReader reader)
    {
        return TryReadFrequentProperty(ref reader)
            || TryReadBoolProperty(ref reader)
            || TryReadNumericProperty(ref reader)
            || TryReadStringProperty(ref reader)
            || TryReadRunners(ref reader);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private bool TryReadFrequentProperty(ref Utf8JsonReader reader)
    {
        if (reader.ValueTextEquals(PropStatus))
            return ReadStatus(ref reader);
        if (reader.ValueTextEquals(PropInPlay))
            return ReadBool(ref reader, out var v) && SetInPlay(v);
        if (reader.ValueTextEquals(PropBetDelay))
            return ReadInt(ref reader, out var v) && SetBetDelay(v);
        if (reader.ValueTextEquals(PropNumberOfActiveRunners))
            return ReadInt(ref reader, out var v) && SetNumberOfActiveRunners(v);
        if (reader.ValueTextEquals(PropVersion))
            return ReadLong(ref reader, out var v) && SetVersion(v);
        if (reader.ValueTextEquals(PropComplete))
            return ReadBool(ref reader, out var v) && SetComplete(v);

        return false;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private bool TryReadBoolProperty(ref Utf8JsonReader reader)
    {
        if (reader.ValueTextEquals(PropBspReconciled))
            return ReadBool(ref reader, out var v) && SetBspReconciled(v);
        if (reader.ValueTextEquals(PropCrossMatching))
            return ReadBool(ref reader, out var v) && SetCrossMatching(v);
        if (reader.ValueTextEquals(PropRunnersVoidable))
            return ReadBool(ref reader, out var v) && SetRunnersVoidable(v);
        if (reader.ValueTextEquals(PropBspMarket))
            return ReadBool(ref reader, out var v) && SetBspMarket(v);
        if (reader.ValueTextEquals(PropTurnInPlayEnabled))
            return ReadBool(ref reader, out var v) && SetTurnInPlayEnabled(v);
        if (reader.ValueTextEquals(PropPersistenceEnabled))
            return ReadBool(ref reader, out var v) && SetPersistenceEnabled(v);
        if (reader.ValueTextEquals(PropDiscountAllowed))
            return ReadBool(ref reader, out var v) && SetDiscountAllowed(v);

        return false;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private bool TryReadNumericProperty(ref Utf8JsonReader reader)
    {
        if (reader.ValueTextEquals(PropMarketBaseRate))
            return ReadDouble(ref reader, out var v) && SetMarketBaseRate(v);
        if (reader.ValueTextEquals(PropEachWayDivisor))
            return ReadDouble(ref reader, out var v) && SetEachWayDivisor(v);
        if (reader.ValueTextEquals(PropNumberOfWinners))
            return ReadInt(ref reader, out var v) && SetNumberOfWinners(v);

        return false;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    private bool TryReadStringProperty(ref Utf8JsonReader reader)
    {
        if (reader.ValueTextEquals(PropBettingType))
            return ReadCachedStringField(ref reader, ref _bettingTypeBytes, v => BettingType = v);
        if (reader.ValueTextEquals(PropMarketType))
            return ReadCachedStringField(ref reader, ref _marketTypeBytes, v => MarketType = v);
        if (reader.ValueTextEquals(PropVenue))
            return ReadCachedStringField(ref reader, ref _venueBytes, v => Venue = v);
        if (reader.ValueTextEquals(PropTimezone))
            return ReadCachedStringField(ref reader, ref _timezoneBytes, v => Timezone = v);
        if (reader.ValueTextEquals(PropCountryCode))
            return ReadCachedStringField(ref reader, ref _countryCodeBytes, v => CountryCode = v);
        if (reader.ValueTextEquals(PropEventId))
            return ReadCachedStringField(ref reader, ref _eventIdBytes, v => EventId = v);
        if (reader.ValueTextEquals(PropEventTypeId))
            return ReadCachedStringField(ref reader, ref _eventTypeIdBytes, v => EventTypeId = v);

        return false;
    }

    private bool TryReadRunners(ref Utf8JsonReader reader)
    {
        if (reader.ValueTextEquals(PropRunners))
        {
            ReadRunners(ref reader);
            return true;
        }

        return false;
    }

    private bool ReadStatus(ref Utf8JsonReader reader)
    {
        reader.Read();
        ReadCachedString(ref reader, ref _statusBytes, out var val);
        if (val != null) Status = val;
        return true;
    }

    private bool SetInPlay(bool v)
    {
        InPlay = v;
        return true;
    }

    private bool SetBetDelay(int v)
    {
        BetDelay = v;
        return true;
    }

    private bool SetNumberOfActiveRunners(int v)
    {
        NumberOfActiveRunners = v;
        return true;
    }

    private bool SetVersion(long v)
    {
        Version = v;
        return true;
    }

    private bool SetComplete(bool v)
    {
        Complete = v;
        return true;
    }

    private bool SetBspReconciled(bool v)
    {
        BspReconciled = v;
        return true;
    }

    private bool SetCrossMatching(bool v)
    {
        CrossMatching = v;
        return true;
    }

    private bool SetRunnersVoidable(bool v)
    {
        RunnersVoidable = v;
        return true;
    }

    private bool SetBspMarket(bool v)
    {
        BspMarket = v;
        return true;
    }

    private bool SetTurnInPlayEnabled(bool v)
    {
        TurnInPlayEnabled = v;
        return true;
    }

    private bool SetPersistenceEnabled(bool v)
    {
        PersistenceEnabled = v;
        return true;
    }

    private bool SetDiscountAllowed(bool v)
    {
        DiscountAllowed = v;
        return true;
    }

    private bool SetMarketBaseRate(double v)
    {
        MarketBaseRate = v;
        return true;
    }

    private bool SetEachWayDivisor(double v)
    {
        EachWayDivisor = v;
        return true;
    }

    private bool SetNumberOfWinners(int v)
    {
        NumberOfWinners = v;
        return true;
    }

    private void ReadRunners(ref Utf8JsonReader reader)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return;

        _runners.Clear();

        while (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
        {
            var def = new RunnerDefinitionCache();
            def.ReadFrom(ref reader);
            _runners.Add(def);
        }
    }
}

/// <summary>
/// Mutable runner definition within a market definition.
/// </summary>
public sealed class RunnerDefinitionCache
{
    public long SelectionId { get; private set; }

    public string? Status { get; private set; }

    public int? SortPriority { get; private set; }

    public double? Handicap { get; private set; }

    public double? AdjustmentFactor { get; private set; }

    public double? BspLiability { get; private set; }

    private static ReadOnlySpan<byte> PropId => "id"u8;

    private static ReadOnlySpan<byte> PropStatus => "status"u8;

    private static ReadOnlySpan<byte> PropSortPriority => "sortPriority"u8;

    private static ReadOnlySpan<byte> PropHc => "hc"u8;

    private static ReadOnlySpan<byte> PropAdjustmentFactor => "adjustmentFactor"u8;

    private static ReadOnlySpan<byte> PropBsp => "bsp"u8;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1502:Avoid excessive complexity", Justification = "Sequential JSON property dispatch — complexity is inherent to the protocol.")]
    internal void ReadFrom(ref Utf8JsonReader reader)
    {
        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(PropId))
            {
                reader.Read();
                SelectionId = reader.GetInt64();
            }
            else if (reader.ValueTextEquals(PropStatus))
            {
                reader.Read();
                Status = reader.GetString();
            }
            else if (reader.ValueTextEquals(PropSortPriority))
            {
                reader.Read();
                SortPriority = reader.GetInt32();
            }
            else if (reader.ValueTextEquals(PropHc))
            {
                reader.Read();
                Handicap = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropAdjustmentFactor))
            {
                reader.Read();
                AdjustmentFactor = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropBsp))
            {
                reader.Read();
                BspLiability = reader.GetDouble();
            }
            else
            {
                reader.Read();
                if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
                    reader.Skip();
            }
        }
    }
}
