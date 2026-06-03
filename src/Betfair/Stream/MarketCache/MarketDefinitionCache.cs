namespace Betfair.Stream.MarketCache;

/// <summary>
/// Mutable market definition updated in-place from stream bytes.
/// No allocation on updates when string values haven't changed.
/// Uses cached UTF-8 bytes for zero-allocation comparison.
/// </summary>
public sealed class MarketDefinitionCache
{
    private static ReadOnlySpan<byte> PropBspMarket => "bspMarket"u8;
    private static ReadOnlySpan<byte> PropTurnInPlayEnabled => "turnInPlayEnabled"u8;
    private static ReadOnlySpan<byte> PropPersistenceEnabled => "persistenceEnabled"u8;
    private static ReadOnlySpan<byte> PropMarketBaseRate => "marketBaseRate"u8;
    private static ReadOnlySpan<byte> PropBettingType => "bettingType"u8;
    private static ReadOnlySpan<byte> PropStatus => "status"u8;
    private static ReadOnlySpan<byte> PropVenue => "venue"u8;
    private static ReadOnlySpan<byte> PropSettledTime => "settledTime"u8;
    private static ReadOnlySpan<byte> PropTimezone => "timezone"u8;
    private static ReadOnlySpan<byte> PropEachWayDivisor => "eachWayDivisor"u8;
    private static ReadOnlySpan<byte> PropRegulators => "regulators"u8;
    private static ReadOnlySpan<byte> PropMarketType => "marketType"u8;
    private static ReadOnlySpan<byte> PropNumberOfWinners => "numberOfWinners"u8;
    private static ReadOnlySpan<byte> PropCountryCode => "countryCode"u8;
    private static ReadOnlySpan<byte> PropInPlay => "inPlay"u8;
    private static ReadOnlySpan<byte> PropBetDelay => "betDelay"u8;
    private static ReadOnlySpan<byte> PropNumberOfActiveRunners => "numberOfActiveRunners"u8;
    private static ReadOnlySpan<byte> PropEventId => "eventId"u8;
    private static ReadOnlySpan<byte> PropCrossMatching => "crossMatching"u8;
    private static ReadOnlySpan<byte> PropRunnersVoidable => "runnersVoidable"u8;
    private static ReadOnlySpan<byte> PropSuspendTime => "suspendTime"u8;
    private static ReadOnlySpan<byte> PropDiscountAllowed => "discountAllowed"u8;
    private static ReadOnlySpan<byte> PropRunners => "runners"u8;
    private static ReadOnlySpan<byte> PropVersion => "version"u8;
    private static ReadOnlySpan<byte> PropEventTypeId => "eventTypeId"u8;
    private static ReadOnlySpan<byte> PropComplete => "complete"u8;
    private static ReadOnlySpan<byte> PropOpenDate => "openDate"u8;
    private static ReadOnlySpan<byte> PropMarketTime => "marketTime"u8;
    private static ReadOnlySpan<byte> PropBspReconciled => "bspReconciled"u8;

    // Runner definition property names
    private static ReadOnlySpan<byte> PropId => "id"u8;
    private static ReadOnlySpan<byte> PropSortPriority => "sortPriority"u8;
    private static ReadOnlySpan<byte> PropRemovalDate => "removalDate"u8;
    private static ReadOnlySpan<byte> PropHc => "hc"u8;
    private static ReadOnlySpan<byte> PropAdjustmentFactor => "adjustmentFactor"u8;
    private static ReadOnlySpan<byte> PropBsp => "bsp"u8;

    // Reusable runner definitions list
    private readonly List<RunnerDefinitionCache> _runners = new(16);

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

    /// <summary>
    /// Reads and applies a market definition update from a Utf8JsonReader.
    /// The reader should be positioned at the StartObject token.
    /// Only allocates strings when values actually change.
    /// </summary>
    internal void ReadFrom(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            return;

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(PropStatus))
            {
                reader.Read();
                ReadCachedString(ref reader, ref _statusBytes, out var val);
                Status = val ?? Status;
                if (val != null) Status = val;
            }
            else if (reader.ValueTextEquals(PropInPlay))
            {
                reader.Read();
                InPlay = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropBetDelay))
            {
                reader.Read();
                BetDelay = reader.GetInt32();
            }
            else if (reader.ValueTextEquals(PropNumberOfActiveRunners))
            {
                reader.Read();
                NumberOfActiveRunners = reader.GetInt32();
            }
            else if (reader.ValueTextEquals(PropVersion))
            {
                reader.Read();
                Version = reader.GetInt64();
            }
            else if (reader.ValueTextEquals(PropComplete))
            {
                reader.Read();
                Complete = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropBspReconciled))
            {
                reader.Read();
                BspReconciled = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropCrossMatching))
            {
                reader.Read();
                CrossMatching = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropRunnersVoidable))
            {
                reader.Read();
                RunnersVoidable = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropBspMarket))
            {
                reader.Read();
                BspMarket = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropTurnInPlayEnabled))
            {
                reader.Read();
                TurnInPlayEnabled = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropPersistenceEnabled))
            {
                reader.Read();
                PersistenceEnabled = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropDiscountAllowed))
            {
                reader.Read();
                DiscountAllowed = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals(PropMarketBaseRate))
            {
                reader.Read();
                MarketBaseRate = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropEachWayDivisor))
            {
                reader.Read();
                EachWayDivisor = reader.GetDouble();
            }
            else if (reader.ValueTextEquals(PropNumberOfWinners))
            {
                reader.Read();
                NumberOfWinners = reader.GetInt32();
            }
            else if (reader.ValueTextEquals(PropBettingType))
            {
                reader.Read();
                ReadCachedString(ref reader, ref _bettingTypeBytes, out var val);
                if (val != null) BettingType = val;
            }
            else if (reader.ValueTextEquals(PropMarketType))
            {
                reader.Read();
                ReadCachedString(ref reader, ref _marketTypeBytes, out var val);
                if (val != null) MarketType = val;
            }
            else if (reader.ValueTextEquals(PropVenue))
            {
                reader.Read();
                ReadCachedString(ref reader, ref _venueBytes, out var val);
                if (val != null) Venue = val;
            }
            else if (reader.ValueTextEquals(PropTimezone))
            {
                reader.Read();
                ReadCachedString(ref reader, ref _timezoneBytes, out var val);
                if (val != null) Timezone = val;
            }
            else if (reader.ValueTextEquals(PropCountryCode))
            {
                reader.Read();
                ReadCachedString(ref reader, ref _countryCodeBytes, out var val);
                if (val != null) CountryCode = val;
            }
            else if (reader.ValueTextEquals(PropEventId))
            {
                reader.Read();
                ReadCachedString(ref reader, ref _eventIdBytes, out var val);
                if (val != null) EventId = val;
            }
            else if (reader.ValueTextEquals(PropEventTypeId))
            {
                reader.Read();
                ReadCachedString(ref reader, ref _eventTypeIdBytes, out var val);
                if (val != null) EventTypeId = val;
            }
            else if (reader.ValueTextEquals(PropRunners))
            {
                ReadRunners(ref reader);
            }
            else
            {
                // Skip settledTime, suspendTime, openDate, marketTime, regulators, and unknown fields
                reader.Read();
                if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
                    reader.Skip();
            }
        }
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
}

/// <summary>
/// Mutable runner definition within a market definition.
/// </summary>
public sealed class RunnerDefinitionCache
{
    private static ReadOnlySpan<byte> PropId => "id"u8;
    private static ReadOnlySpan<byte> PropStatus => "status"u8;
    private static ReadOnlySpan<byte> PropSortPriority => "sortPriority"u8;
    private static ReadOnlySpan<byte> PropHc => "hc"u8;
    private static ReadOnlySpan<byte> PropAdjustmentFactor => "adjustmentFactor"u8;
    private static ReadOnlySpan<byte> PropBsp => "bsp"u8;

    public long SelectionId { get; private set; }

    public string? Status { get; private set; }

    public int? SortPriority { get; private set; }

    public double? Handicap { get; private set; }

    public double? AdjustmentFactor { get; private set; }

    public double? BspLiability { get; private set; }

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
