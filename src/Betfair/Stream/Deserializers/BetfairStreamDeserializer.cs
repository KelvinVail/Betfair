using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

internal sealed class BetfairStreamDeserializer
{
    private static readonly byte[] _opProperty = "op"u8.ToArray();
    private static readonly byte[] _idProperty = "id"u8.ToArray();
    private static readonly byte[] _initialClkProperty = "initialClk"u8.ToArray();
    private static readonly byte[] _clkProperty = "clk"u8.ToArray();
    private static readonly byte[] _ptProperty = "pt"u8.ToArray();
    private static readonly byte[] _ctProperty = "ct"u8.ToArray();
    private static readonly byte[] _mcProperty = "mc"u8.ToArray();
    private static readonly byte[] _ocProperty = "oc"u8.ToArray();
    private static readonly byte[] _statusCodeProperty = "statusCode"u8.ToArray();
    private static readonly byte[] _errorCodeProperty = "errorCode"u8.ToArray();
    private static readonly byte[] _connectionIdProperty = "connectionId"u8.ToArray();
    private static readonly byte[] _connectionClosedProperty = "connectionClosed"u8.ToArray();
    private static readonly byte[] _connectionsAvailableProperty = "connectionsAvailable"u8.ToArray();
    private static readonly byte[] _conflateProperty = "conflateMs"u8.ToArray();
    private static readonly byte[] _heartbeatProperty = "heartbeatMs"u8.ToArray();
    private static readonly byte[] _segmentTypeProperty = "segmentType"u8.ToArray();

    private static readonly byte[] _marketDefinitionProperty = "marketDefinition"u8.ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChangeMessage? DeserializeChangeMessage(byte[] lineBytes)
    {
        if (lineBytes == null || lineBytes.Length == 0)
            return null;

        return DeserializeChangeMessage(lineBytes.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChangeMessage? DeserializeChangeMessage(ReadOnlySpan<byte> lineSpan)
    {
        try
        {
            var reader = new Utf8JsonReader(lineSpan);
            return ReadChangeMessage(ref reader);
        }
        catch (JsonException)
        {
            // Skip invalid JSON messages
            return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ChangeMessage? ReadChangeMessage(ref Utf8JsonReader reader)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return null;

        // Collect all properties first
        string? operation = null;
        int id = 0;
        string? initialClock = null;
        string? clock = null;
        long? publishTime = null;
        string? changeType = null;
        string? statusCode = null;
        string? errorCode = null;
        string? connectionId = null;
        bool? connectionClosed = null;
        int? connectionsAvailable = null;
        int? conflateMs = null;
        int? heartbeatMs = null;
        string? segmentType = null;
        List<MarketChange>? marketChanges = null;
        List<OrderChange>? orderChanges = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.ValueSpan;

                if (propertyName.SequenceEqual(_opProperty))
                {
                    reader.Read();
                    operation = reader.GetString();
                }
                else if (propertyName.SequenceEqual(_idProperty))
                {
                    reader.Read();
                    id = reader.GetInt32();
                }
                else if (propertyName.SequenceEqual(_initialClkProperty))
                {
                    reader.Read();
                    initialClock = reader.GetString();
                }
                else if (propertyName.SequenceEqual(_clkProperty))
                {
                    reader.Read();
                    clock = reader.GetString();
                }
                else if (propertyName.SequenceEqual(_ptProperty))
                {
                    reader.Read();
                    publishTime = reader.GetInt64();
                }
                else if (propertyName.SequenceEqual(_ctProperty))
                {
                    reader.Read();
                    changeType = reader.GetString();
                }
                else if (propertyName.SequenceEqual(_statusCodeProperty))
                {
                    reader.Read();
                    statusCode = reader.GetString();
                }
                else if (propertyName.SequenceEqual(_errorCodeProperty))
                {
                    reader.Read();
                    errorCode = reader.GetString();
                }
                else if (propertyName.SequenceEqual(_connectionIdProperty))
                {
                    reader.Read();
                    connectionId = reader.GetString();
                }
                else if (propertyName.SequenceEqual(_connectionClosedProperty))
                {
                    reader.Read();
                    connectionClosed = reader.GetBoolean();
                }
                else if (propertyName.SequenceEqual(_connectionsAvailableProperty))
                {
                    reader.Read();
                    connectionsAvailable = reader.GetInt32();
                }
                else if (propertyName.SequenceEqual(_conflateProperty))
                {
                    reader.Read();
                    conflateMs = reader.GetInt32();
                }
                else if (propertyName.SequenceEqual(_heartbeatProperty))
                {
                    reader.Read();
                    heartbeatMs = reader.GetInt32();
                }
                else if (propertyName.SequenceEqual(_segmentTypeProperty))
                {
                    reader.Read();
                    segmentType = reader.GetString();
                }
                else if (propertyName.SequenceEqual(_mcProperty))
                {
                    marketChanges = ReadMarketChanges(ref reader);
                }
                else if (propertyName.SequenceEqual(_ocProperty))
                {
                    orderChanges = ReadOrderChanges(ref reader);
                }
                else
                {
                    // Skip unknown properties
                    reader.Read();
                    reader.Skip();
                }
            }
        }

        // Create the object with all properties
        return new ChangeMessage
        {
            Operation = operation,
            Id = id,
            InitialClock = initialClock,
            Clock = clock,
            PublishTime = publishTime,
            ChangeType = changeType,
            StatusCode = statusCode,
            ErrorCode = errorCode,
            ConnectionId = connectionId,
            ConnectionClosed = connectionClosed,
            ConnectionsAvailable = connectionsAvailable,
            ConflateMs = conflateMs,
            HeartbeatMs = heartbeatMs,
            SegmentType = segmentType,
            MarketChanges = marketChanges,
            OrderChanges = orderChanges
        };
    }

    /// <summary>
    /// Reads market changes array with optimized performance.
    /// </summary>
    private List<MarketChange>? ReadMarketChanges(ref Utf8JsonReader reader)
    {
        reader.Read(); // Read the array start
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var marketChanges = new List<MarketChange>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var marketChange = ReadMarketChange(ref reader);
                if (marketChange != null)
                    marketChanges.Add(marketChange);
            }
        }

        return marketChanges;
    }

    /// <summary>
    /// Reads order changes array with optimized performance.
    /// </summary>
    private List<OrderChange>? ReadOrderChanges(ref Utf8JsonReader reader)
    {
        reader.Read(); // Read the array start
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var orderChanges = new List<OrderChange>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var orderChange = ReadOrderChange(ref reader);
                if (orderChange != null)
                    orderChanges.Add(orderChange);
            }
        }

        return orderChanges;
    }

    /// <summary>
    /// Reads a MarketChange using basic parsing for now.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MarketChange? ReadMarketChange(ref Utf8JsonReader reader)
    {
        string? marketId = null;
        double? totalAmountMatched = null;
        List<RunnerChange>? runnerChanges = null;
        bool? replaceCache = null;
        bool? conflated = null;
        MarketDefinition? marketDefinition = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var propertyName = reader.ValueSpan;

            if (propertyName.SequenceEqual("id"u8))
            {
                reader.Read();
                marketId = reader.GetString();
            }
            else if (propertyName.SequenceEqual("tv"u8))
            {
                reader.Read();
                totalAmountMatched = reader.GetDouble();
            }
            else if (propertyName.SequenceEqual("rc"u8))
            {
                runnerChanges = ReadRunnerChanges(ref reader);
            }
            else if (propertyName.SequenceEqual("img"u8))
            {
                reader.Read();
                replaceCache = reader.GetBoolean();
            }
            else if (propertyName.SequenceEqual("con"u8))
            {
                reader.Read();
                conflated = reader.GetBoolean();
            }
            else if (propertyName.SequenceEqual(_marketDefinitionProperty))
            {
                marketDefinition = ReadMarketDefinition(ref reader);
            }
            else
            {
                // Skip unknown properties
                reader.Read();
                reader.Skip();
            }
        }

        return new MarketChange
        {
            MarketId = marketId,
            TotalAmountMatched = totalAmountMatched,
            RunnerChanges = runnerChanges,
            ReplaceCache = replaceCache,
            Conflated = conflated,
            MarketDefinition = marketDefinition
        };
    }

    /// <summary>
    /// Reads an OrderChange using basic parsing for now.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private OrderChange? ReadOrderChange(ref Utf8JsonReader reader)
    {
        string? marketId = null;
        long? accountId = null;
        bool? closed = null;
        List<OrderRunnerChange>? orderRunnerChanges = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var propertyName = reader.ValueSpan;

            if (propertyName.SequenceEqual("id"u8))
            {
                reader.Read();
                marketId = reader.GetString();
            }
            else if (propertyName.SequenceEqual("accountId"u8))
            {
                reader.Read();
                accountId = reader.GetInt64();
            }
            else if (propertyName.SequenceEqual("closed"u8))
            {
                reader.Read();
                closed = reader.GetBoolean();
            }
            else
            {
                // Skip unknown properties
                reader.Read();
                reader.Skip();
            }
        }

        return new OrderChange
        {
            MarketId = marketId,
            AccountId = accountId,
            Closed = closed,
            OrderRunnerChanges = orderRunnerChanges
        };
    }

    /// <summary>
    /// Reads runner changes array with basic parsing.
    /// </summary>
    private List<RunnerChange>? ReadRunnerChanges(ref Utf8JsonReader reader)
    {
        reader.Read(); // Read the array start
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var runnerChanges = new List<RunnerChange>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var runnerChange = ReadRunnerChange(ref reader);
                if (runnerChange != null)
                    runnerChanges.Add(runnerChange);
            }
        }

        return runnerChanges;
    }

    /// <summary>
    /// Reads arrays of double arrays (e.g., [[1.5, 100], [2.0, 200]]) with optimized performance.
    /// </summary>
    private List<List<double>>? ReadDoubleArrays(ref Utf8JsonReader reader)
    {
        reader.Read(); // Read the array start
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var result = new List<List<double>>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var innerArray = ReadDoubleArray(ref reader);
                if (innerArray != null && innerArray.Count > 0)
                    result.Add(innerArray);
            }
        }

        return result;
    }

    /// <summary>
    /// Reads a single array of doubles with optimized performance.
    /// </summary>
    private List<double>? ReadDoubleArray(ref Utf8JsonReader reader)
    {
        var result = new List<double>(4); // Most arrays have 2-3 elements

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.Number)
            {
                result.Add(reader.GetDouble());
            }
        }

        return result.Count > 0 ? result : null;
    }

    /// <summary>
    /// Reads a RunnerChange using basic parsing.
    /// </summary>
    private RunnerChange? ReadRunnerChange(ref Utf8JsonReader reader)
    {
        long? selectionId = null;
        double? totalMatched = null;
        double? lastTradedPrice = null;
        double? startingPriceFar = null;
        double? startingPriceNear = null;
        double? handicap = null;
        List<List<double>>? bestAvailableToBack = null;
        List<List<double>>? startingPriceBack = null;
        List<List<double>>? bestDisplayAvailableToLay = null;
        List<List<double>>? traded = null;
        List<List<double>>? availableToBack = null;
        List<List<double>>? startingPriceLay = null;
        List<List<double>>? availableToLay = null;
        List<List<double>>? bestAvailableToLay = null;
        List<List<double>>? bestDisplayAvailableToBack = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var propertyName = reader.ValueSpan;

            if (propertyName.SequenceEqual("id"u8))
            {
                reader.Read();
                selectionId = reader.GetInt64();
            }
            else if (propertyName.SequenceEqual("tv"u8))
            {
                reader.Read();
                totalMatched = reader.GetDouble();
            }
            else if (propertyName.SequenceEqual("ltp"u8))
            {
                reader.Read();
                lastTradedPrice = reader.GetDouble();
            }
            else if (propertyName.SequenceEqual("spf"u8))
            {
                reader.Read();
                startingPriceFar = reader.GetDouble();
            }
            else if (propertyName.SequenceEqual("spn"u8))
            {
                reader.Read();
                startingPriceNear = reader.GetDouble();
            }
            else if (propertyName.SequenceEqual("hc"u8))
            {
                reader.Read();
                handicap = reader.GetDouble();
            }
            else if (propertyName.SequenceEqual("batb"u8))
            {
                bestAvailableToBack = ReadDoubleArrays(ref reader);
            }
            else if (propertyName.SequenceEqual("spb"u8))
            {
                startingPriceBack = ReadDoubleArrays(ref reader);
            }
            else if (propertyName.SequenceEqual("bdatl"u8))
            {
                bestDisplayAvailableToLay = ReadDoubleArrays(ref reader);
            }
            else if (propertyName.SequenceEqual("trd"u8))
            {
                traded = ReadDoubleArrays(ref reader);
            }
            else if (propertyName.SequenceEqual("atb"u8))
            {
                availableToBack = ReadDoubleArrays(ref reader);
            }
            else if (propertyName.SequenceEqual("spl"u8))
            {
                startingPriceLay = ReadDoubleArrays(ref reader);
            }
            else if (propertyName.SequenceEqual("atl"u8))
            {
                availableToLay = ReadDoubleArrays(ref reader);
            }
            else if (propertyName.SequenceEqual("batl"u8))
            {
                bestAvailableToLay = ReadDoubleArrays(ref reader);
            }
            else if (propertyName.SequenceEqual("bdatb"u8))
            {
                bestDisplayAvailableToBack = ReadDoubleArrays(ref reader);
            }
            else
            {
                // Skip unknown properties
                reader.Read();
                reader.Skip();
            }
        }

        return new RunnerChange
        {
            SelectionId = selectionId,
            TotalMatched = totalMatched,
            LastTradedPrice = lastTradedPrice,
            StartingPriceFar = startingPriceFar,
            StartingPriceNear = startingPriceNear,
            Handicap = handicap,
            BestAvailableToBack = bestAvailableToBack,
            StartingPriceBack = startingPriceBack,
            BestDisplayAvailableToLay = bestDisplayAvailableToLay,
            Traded = traded,
            AvailableToBack = availableToBack,
            StartingPriceLay = startingPriceLay,
            AvailableToLay = availableToLay,
            BestAvailableToLay = bestAvailableToLay,
            BestDisplayAvailableToBack = bestDisplayAvailableToBack
        };
    }

    /// <summary>
    /// Reads a MarketDefinition using optimized parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MarketDefinition? ReadMarketDefinition(ref Utf8JsonReader reader)
    {
        reader.Read(); // Read the object start
        if (reader.TokenType != JsonTokenType.StartObject)
            return null;

        bool? bspMarket = null;
        bool? turnInPlayEnabled = null;
        bool? persistenceEnabled = null;
        double? marketBaseRate = null;
        string? bettingType = null;
        string? status = null;
        string? venue = null;
        DateTime? settledTime = null;
        string? timezone = null;
        double? eachWayDivisor = null;
        List<string>? regulators = null;
        string? marketType = null;
        int? numberOfWinners = null;
        string? countryCode = null;
        bool? inPlay = null;
        int? betDelay = null;
        int? numberOfActiveRunners = null;
        string? eventId = null;
        bool? crossMatching = null;
        bool? runnersVoidable = null;
        DateTime? suspendTime = null;
        bool? discountAllowed = null;
        List<RunnerDefinition>? runners = null;
        long? version = null;
        string? eventTypeId = null;
        bool? complete = null;
        DateTime? openDate = null;
        DateTime? marketTime = null;
        bool? bspReconciled = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var propertyName = reader.ValueSpan;

            if (propertyName.SequenceEqual("bspMarket"u8))
            {
                reader.Read();
                bspMarket = reader.GetBoolean();
            }
            else if (propertyName.SequenceEqual("turnInPlayEnabled"u8))
            {
                reader.Read();
                turnInPlayEnabled = reader.GetBoolean();
            }
            else if (propertyName.SequenceEqual("persistenceEnabled"u8))
            {
                reader.Read();
                persistenceEnabled = reader.GetBoolean();
            }
            else if (propertyName.SequenceEqual("marketBaseRate"u8))
            {
                reader.Read();
                marketBaseRate = reader.GetDouble();
            }
            else if (propertyName.SequenceEqual("bettingType"u8))
            {
                reader.Read();
                bettingType = reader.GetString();
            }
            else if (propertyName.SequenceEqual("status"u8))
            {
                reader.Read();
                status = reader.GetString();
            }
            else if (propertyName.SequenceEqual("venue"u8))
            {
                reader.Read();
                venue = reader.GetString();
            }
            else if (propertyName.SequenceEqual("settledTime"u8))
            {
                reader.Read();
                settledTime = reader.GetDateTime();
            }
            else if (propertyName.SequenceEqual("timezone"u8))
            {
                reader.Read();
                timezone = reader.GetString();
            }
            else if (propertyName.SequenceEqual("eachWayDivisor"u8))
            {
                reader.Read();
                eachWayDivisor = reader.GetDouble();
            }
            else if (propertyName.SequenceEqual("regulators"u8))
            {
                regulators = ReadStringArray(ref reader);
            }
            else if (propertyName.SequenceEqual("marketType"u8))
            {
                reader.Read();
                marketType = reader.GetString();
            }
            else if (propertyName.SequenceEqual("numberOfWinners"u8))
            {
                reader.Read();
                numberOfWinners = reader.GetInt32();
            }
            else if (propertyName.SequenceEqual("countryCode"u8))
            {
                reader.Read();
                countryCode = reader.GetString();
            }
            else if (propertyName.SequenceEqual("inPlay"u8))
            {
                reader.Read();
                inPlay = reader.GetBoolean();
            }
            else if (propertyName.SequenceEqual("betDelay"u8))
            {
                reader.Read();
                betDelay = reader.GetInt32();
            }
            else if (propertyName.SequenceEqual("numberOfActiveRunners"u8))
            {
                reader.Read();
                numberOfActiveRunners = reader.GetInt32();
            }
            else if (propertyName.SequenceEqual("eventId"u8))
            {
                reader.Read();
                eventId = reader.GetString();
            }
            else if (propertyName.SequenceEqual("crossMatching"u8))
            {
                reader.Read();
                crossMatching = reader.GetBoolean();
            }
            else if (propertyName.SequenceEqual("runnersVoidable"u8))
            {
                reader.Read();
                runnersVoidable = reader.GetBoolean();
            }
            else if (propertyName.SequenceEqual("suspendTime"u8))
            {
                reader.Read();
                suspendTime = reader.GetDateTime();
            }
            else if (propertyName.SequenceEqual("discountAllowed"u8))
            {
                reader.Read();
                discountAllowed = reader.GetBoolean();
            }
            else if (propertyName.SequenceEqual("runners"u8))
            {
                runners = ReadRunnerDefinitions(ref reader);
            }
            else if (propertyName.SequenceEqual("version"u8))
            {
                reader.Read();
                version = reader.GetInt64();
            }
            else if (propertyName.SequenceEqual("eventTypeId"u8))
            {
                reader.Read();
                eventTypeId = reader.GetString();
            }
            else if (propertyName.SequenceEqual("complete"u8))
            {
                reader.Read();
                complete = reader.GetBoolean();
            }
            else if (propertyName.SequenceEqual("openDate"u8))
            {
                reader.Read();
                openDate = reader.GetDateTime();
            }
            else if (propertyName.SequenceEqual("marketTime"u8))
            {
                reader.Read();
                marketTime = reader.GetDateTime();
            }
            else if (propertyName.SequenceEqual("bspReconciled"u8))
            {
                reader.Read();
                bspReconciled = reader.GetBoolean();
            }
            else
            {
                // Skip unknown properties
                reader.Read();
                reader.Skip();
            }
        }

        return new MarketDefinition
        {
            BspMarket = bspMarket,
            TurnInPlayEnabled = turnInPlayEnabled,
            PersistenceEnabled = persistenceEnabled,
            MarketBaseRate = marketBaseRate,
            BettingType = bettingType,
            Status = status,
            Venue = venue,
            SettledTime = settledTime,
            Timezone = timezone,
            EachWayDivisor = eachWayDivisor,
            Regulators = regulators,
            MarketType = marketType,
            NumberOfWinners = numberOfWinners,
            CountryCode = countryCode,
            InPlay = inPlay,
            BetDelay = betDelay,
            NumberOfActiveRunners = numberOfActiveRunners,
            EventId = eventId,
            CrossMatching = crossMatching,
            RunnersVoidable = runnersVoidable,
            SuspendTime = suspendTime,
            DiscountAllowed = discountAllowed,
            Runners = runners,
            Version = version,
            EventTypeId = eventTypeId,
            Complete = complete,
            OpenDate = openDate,
            MarketTime = marketTime,
            BspReconciled = bspReconciled
        };
    }

    /// <summary>
    /// Reads an array of strings with optimized performance.
    /// </summary>
    private List<string>? ReadStringArray(ref Utf8JsonReader reader)
    {
        reader.Read(); // Read the array start
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var result = new List<string>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                if (value != null)
                    result.Add(value);
            }
        }

        return result;
    }

    /// <summary>
    /// Reads runner definitions array with optimized performance.
    /// </summary>
    private List<RunnerDefinition>? ReadRunnerDefinitions(ref Utf8JsonReader reader)
    {
        reader.Read(); // Read the array start
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var runners = new List<RunnerDefinition>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var runner = ReadRunnerDefinition(ref reader);
                if (runner != null)
                    runners.Add(runner);
            }
        }

        return runners;
    }

    /// <summary>
    /// Reads a RunnerDefinition using optimized parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RunnerDefinition? ReadRunnerDefinition(ref Utf8JsonReader reader)
    {
        string? status = null;
        int? sortPriority = null;
        DateTime? removalDate = null;
        long selectionId = 0;
        double? handicap = null;
        double? adjustmentFactor = null;
        double? bspLiability = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            var propertyName = reader.ValueSpan;

            if (propertyName.SequenceEqual("status"u8))
            {
                reader.Read();
                status = reader.GetString();
            }
            else if (propertyName.SequenceEqual("sortPriority"u8))
            {
                reader.Read();
                sortPriority = reader.GetInt32();
            }
            else if (propertyName.SequenceEqual("removalDate"u8))
            {
                reader.Read();
                removalDate = reader.GetDateTime();
            }
            else if (propertyName.SequenceEqual("id"u8))
            {
                reader.Read();
                selectionId = reader.GetInt64();
            }
            else if (propertyName.SequenceEqual("hc"u8))
            {
                reader.Read();
                handicap = reader.GetDouble();
            }
            else if (propertyName.SequenceEqual("adjustmentFactor"u8))
            {
                reader.Read();
                adjustmentFactor = reader.GetDouble();
            }
            else if (propertyName.SequenceEqual("bsp"u8))
            {
                reader.Read();
                bspLiability = reader.GetDouble();
            }
            else
            {
                // Skip unknown properties
                reader.Read();
                reader.Skip();
            }
        }

        return new RunnerDefinition
        {
            Status = status,
            SortPriority = sortPriority,
            RemovalDate = removalDate,
            SelectionId = selectionId,
            Handicap = handicap,
            AdjustmentFactor = adjustmentFactor,
            BspLiability = bspLiability
        };
    }
}
