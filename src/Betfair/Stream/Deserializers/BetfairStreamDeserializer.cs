using System.Buffers;
using System.Buffers.Text;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

public sealed class BetfairStreamDeserializer
{
    // Object pools for reducing GC pressure
    private static readonly ConcurrentQueue<List<MarketChange>> _marketChangeListPool = new();
    private static readonly ConcurrentQueue<List<OrderChange>> _orderChangeListPool = new();
    private static readonly ConcurrentQueue<List<RunnerChange>> _runnerChangeListPool = new();
    private static readonly ConcurrentQueue<List<List<double>>> _doubleArrayListPool = new();
    private static readonly ConcurrentQueue<List<double>> _doubleListPool = new();
    private static readonly ConcurrentQueue<List<string>> _stringListPool = new();
    private static readonly ConcurrentQueue<List<RunnerDefinition>> _runnerDefinitionListPool = new();

    /// <summary>
    /// High-performance JSON reader optimized for Betfair stream data parsing.
    /// Reads bytes directly from streams to minimize GC pressure and maximize speed.
    /// </summary>
    private ref struct FastJsonReader
    {
        private readonly ReadOnlySpan<byte> _buffer;
        private int _lastValueStart;
        private int _lastValueEnd;

        public FastJsonReader(ReadOnlySpan<byte> data)
        {
            _buffer = data;
            Position = 0;
            TokenType = JsonTokenType.None;
            _lastValueStart = 0;
            _lastValueEnd = 0;

            // Validate that we have valid JSON structure
            if (data.Length == 0)
                throw new JsonException("Empty JSON");
        }

        public int Position { get; private set; }
        public JsonTokenType TokenType { get; private set; }
        public ReadOnlySpan<byte> ValueSpan => _buffer[_lastValueStart.._lastValueEnd];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Read()
        {
            // Skip whitespace
            SkipWhitespace();

            if (Position >= _buffer.Length)
                return false;

            byte currentByte = _buffer[Position];

            switch (currentByte)
            {
                case (byte)'{':
                    TokenType = JsonTokenType.StartObject;
                    Position++;
                    return true;
                case (byte)'}':
                    TokenType = JsonTokenType.EndObject;
                    Position++;
                    return true;
                case (byte)'[':
                    TokenType = JsonTokenType.StartArray;
                    Position++;
                    return true;
                case (byte)']':
                    TokenType = JsonTokenType.EndArray;
                    Position++;
                    return true;
                case (byte)'"':
                    return ReadString();
                case (byte)',':
                    Position++;
                    return Read(); // Skip comma and read next token
                case (byte)':':
                    Position++;
                    return Read(); // Skip colon and read next token
                case (byte)'t':
                case (byte)'f':
                    return ReadBoolean();
                case (byte)'n':
                    return ReadNull();
                default:
                    if (IsDigit(currentByte) || currentByte == (byte)'-')
                        return ReadNumber();

                    throw new JsonException($"Unexpected character: {(char)currentByte}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Skip()
        {
            if (TokenType == JsonTokenType.StartObject)
            {
                int depth = 1;
                while (depth > 0 && Read())
                {
                    if (TokenType == JsonTokenType.StartObject)
                        depth++;
                    else if (TokenType == JsonTokenType.EndObject)
                        depth--;
                }
            }
            else if (TokenType == JsonTokenType.StartArray)
            {
                int depth = 1;
                while (depth > 0 && Read())
                {
                    if (TokenType == JsonTokenType.StartArray)
                        depth++;
                    else if (TokenType == JsonTokenType.EndArray)
                        depth--;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SkipWhitespace()
        {
            while (Position < _buffer.Length)
            {
                byte b = _buffer[Position];
                if (b == ' ' || b == '\t' || b == '\r' || b == '\n')
                    Position++;
                else
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(byte b) => b >= '0' && b <= '9';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ReadString()
        {
            Position++; // Skip opening quote
            _lastValueStart = Position;

            while (Position < _buffer.Length)
            {
                byte b = _buffer[Position];
                if (b == '"')
                {
                    _lastValueEnd = Position;
                    Position++; // Skip closing quote
                    TokenType = JsonTokenType.String;
                    return true;
                }
                if (b == '\\')
                {
                    Position++; // Skip escaped character
                    if (Position >= _buffer.Length)
                        throw new JsonException("Incomplete escape sequence");
                }
                Position++;
            }
            throw new JsonException("Unterminated string");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ReadNumber()
        {
            _lastValueStart = Position;

            // Skip optional minus
            if (Position < _buffer.Length && _buffer[Position] == '-')
                Position++;

            // Read digits
            while (Position < _buffer.Length && IsDigit(_buffer[Position]))
                Position++;

            // Read decimal part if present
            if (Position < _buffer.Length && _buffer[Position] == '.')
            {
                Position++;
                while (Position < _buffer.Length && IsDigit(_buffer[Position]))
                    Position++;
            }

            // Read exponent if present
            if (Position < _buffer.Length && (_buffer[Position] == 'e' || _buffer[Position] == 'E'))
            {
                Position++;
                if (Position < _buffer.Length && (_buffer[Position] == '+' || _buffer[Position] == '-'))
                    Position++;
                while (Position < _buffer.Length && IsDigit(_buffer[Position]))
                    Position++;
            }

            _lastValueEnd = Position;
            TokenType = JsonTokenType.Number;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ReadBoolean()
        {
            _lastValueStart = Position;

            if (Position + 4 <= _buffer.Length &&
                _buffer[Position] == 't' && _buffer[Position + 1] == 'r' &&
                _buffer[Position + 2] == 'u' && _buffer[Position + 3] == 'e')
            {
                Position += 4;
                _lastValueEnd = Position;
                TokenType = JsonTokenType.True;
                return true;
            }

            if (Position + 5 <= _buffer.Length &&
                _buffer[Position] == 'f' && _buffer[Position + 1] == 'a' &&
                _buffer[Position + 2] == 'l' && _buffer[Position + 3] == 's' &&
                _buffer[Position + 4] == 'e')
            {
                Position += 5;
                _lastValueEnd = Position;
                TokenType = JsonTokenType.False;
                return true;
            }

            throw new JsonException("Invalid boolean value");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ReadNull()
        {
            _lastValueStart = Position;

            if (Position + 4 <= _buffer.Length &&
                _buffer[Position] == 'n' && _buffer[Position + 1] == 'u' &&
                _buffer[Position + 2] == 'l' && _buffer[Position + 3] == 'l')
            {
                Position += 4;
                _lastValueEnd = Position;
                TokenType = JsonTokenType.Null;
                return true;
            }

            throw new JsonException("Invalid null value");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetInt32()
        {
            if (TokenType == JsonTokenType.Number)
            {
                Utf8Parser.TryParse(ValueSpan, out int value, out _);
                return value;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetInt64()
        {
            if (TokenType == JsonTokenType.Number)
            {
                Utf8Parser.TryParse(ValueSpan, out long value, out _);
                return value;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetDouble()
        {
            if (TokenType == JsonTokenType.Number)
            {
                Utf8Parser.TryParse(ValueSpan, out double value, out _);
                return value;
            }
            return 0.0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetBoolean()
        {
            return TokenType == JsonTokenType.True;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime GetDateTime()
        {
            if (TokenType == JsonTokenType.String)
            {
                Utf8Parser.TryParse(ValueSpan, out DateTime value, out _, 'O');
                return value;
            }
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? GetString()
        {
            if (TokenType == JsonTokenType.String)
                return ValueSpan.Length == 0 ? string.Empty : System.Text.Encoding.UTF8.GetString(ValueSpan);
            if (TokenType == JsonTokenType.Null)
                return null;
            return string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double? GetNullableDouble()
        {
            if (TokenType == JsonTokenType.Number)
            {
                // Try parsing as int first (since JSON often has integers)
                if (Utf8Parser.TryParse(ValueSpan, out int intValue, out _))
                    return intValue;

                // If that fails, try parsing as double
                if (Utf8Parser.TryParse(ValueSpan, out double doubleValue, out _))
                    return doubleValue;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime? GetNullableDateTime()
        {
            if (TokenType == JsonTokenType.String)
            {
                var str = System.Text.Encoding.UTF8.GetString(ValueSpan);
                if (DateTime.TryParse(str, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime value))
                    return value;
            }
            return null;
        }

    }



    private enum PropertyType : byte
    {
        Unknown = 0,
        Op = 1,
        Id = 2,
        InitialClk = 3,
        Clk = 4,
        Pt = 5,
        Ct = 6,
        Mc = 7,
        Oc = 8,
        StatusCode = 9,
        ErrorCode = 10,
        ConnectionId = 11,
        ConnectionClosed = 12,
        ConnectionsAvailable = 13,
        ConflateMs = 14,
        HeartbeatMs = 15,
        SegmentType = 16,
        MarketDefinition = 17,
        Tv = 18,
        Rc = 19,
        Img = 20,
        Con = 21,
        Ltp = 22,
        Spf = 23,
        Spn = 24,
        Hc = 25,
        Batb = 26,
        Spb = 27,
        Bdatl = 28,
        Trd = 29,
        Atb = 30,
        Spl = 31,
        Atl = 32,
        Batl = 33,
        Bdatb = 34,
        AccountId = 35,
        Closed = 36,
        BspMarket = 37,
        TurnInPlayEnabled = 38,
        PersistenceEnabled = 39,
        MarketBaseRate = 40,
        BettingType = 41,
        Status = 42,
        Venue = 43,
        SettledTime = 44,
        Timezone = 45,
        EachWayDivisor = 46,
        Regulators = 47,
        MarketType = 48,
        NumberOfWinners = 49,
        CountryCode = 50,
        InPlay = 51,
        BetDelay = 52,
        NumberOfActiveRunners = 53,
        EventId = 54,
        CrossMatching = 55,
        RunnersVoidable = 56,
        SuspendTime = 57,
        DiscountAllowed = 58,
        Runners = 59,
        Version = 60,
        EventTypeId = 61,
        Complete = 62,
        OpenDate = 63,
        MarketTime = 64,
        BspReconciled = 65,
        SortPriority = 66,
        RemovalDate = 67,
        AdjustmentFactor = 68,
        Bsp = 69
    }



    // Object pool helper methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<T> GetPooledList<T>(ConcurrentQueue<List<T>> pool)
    {
        if (pool.TryDequeue(out var list))
        {
            list.Clear();
            return list;
        }
        return new List<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReturnToPool<T>(List<T> list, ConcurrentQueue<List<T>> pool)
    {
        if (list.Count < 1000) // Prevent memory bloat
        {
            pool.Enqueue(list);
        }
    }

    // Optimized property lookup using length-based switching and direct byte comparisons
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetPropertyType(ReadOnlySpan<byte> propertyName)
    {
        // Fast path for most common properties by length
        return propertyName.Length switch
        {
            2 => GetTwoCharProperty(propertyName),
            3 => GetThreeCharProperty(propertyName),
            4 => GetFourCharProperty(propertyName),
            5 => GetFiveCharProperty(propertyName),
            6 => GetSixCharProperty(propertyName),
            7 => GetSevenCharProperty(propertyName),
            8 => GetEightCharProperty(propertyName),
            9 => GetNineCharProperty(propertyName),
            10 => GetTenCharProperty(propertyName),
            11 => GetElevenCharProperty(propertyName),
            12 => GetTwelveCharProperty(propertyName),
            13 => GetThirteenCharProperty(propertyName),
            14 => GetFourteenCharProperty(propertyName),
            15 => GetFifteenCharProperty(propertyName),
            16 => GetSixteenCharProperty(propertyName),
            17 => GetSeventeenCharProperty(propertyName),
            18 => GetEighteenCharProperty(propertyName),
            19 => GetNineteenCharProperty(propertyName),
            20 => GetTwentyCharProperty(propertyName),
            21 => GetTwentyOneCharProperty(propertyName),
            _ => PropertyType.Unknown
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetTwoCharProperty(ReadOnlySpan<byte> span)
    {
        if (span[0] == 'o' && span[1] == 'p') return PropertyType.Op;
        if (span[0] == 'i' && span[1] == 'd') return PropertyType.Id;
        if (span[0] == 'p' && span[1] == 't') return PropertyType.Pt;
        if (span[0] == 'c' && span[1] == 't') return PropertyType.Ct;
        if (span[0] == 'm' && span[1] == 'c') return PropertyType.Mc;
        if (span[0] == 'o' && span[1] == 'c') return PropertyType.Oc;
        if (span[0] == 't' && span[1] == 'v') return PropertyType.Tv;
        if (span[0] == 'r' && span[1] == 'c') return PropertyType.Rc;
        if (span[0] == 'h' && span[1] == 'c') return PropertyType.Hc;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetThreeCharProperty(ReadOnlySpan<byte> span)
    {
        if (span[0] == 'c' && span[1] == 'l' && span[2] == 'k') return PropertyType.Clk;
        if (span[0] == 'i' && span[1] == 'm' && span[2] == 'g') return PropertyType.Img;
        if (span[0] == 'c' && span[1] == 'o' && span[2] == 'n') return PropertyType.Con;
        if (span[0] == 'l' && span[1] == 't' && span[2] == 'p') return PropertyType.Ltp;
        if (span[0] == 's' && span[1] == 'p' && span[2] == 'f') return PropertyType.Spf;
        if (span[0] == 's' && span[1] == 'p' && span[2] == 'n') return PropertyType.Spn;
        if (span[0] == 's' && span[1] == 'p' && span[2] == 'b') return PropertyType.Spb;
        if (span[0] == 't' && span[1] == 'r' && span[2] == 'd') return PropertyType.Trd;
        if (span[0] == 'a' && span[1] == 't' && span[2] == 'b') return PropertyType.Atb;
        if (span[0] == 's' && span[1] == 'p' && span[2] == 'l') return PropertyType.Spl;
        if (span[0] == 'a' && span[1] == 't' && span[2] == 'l') return PropertyType.Atl;
        if (span[0] == 'b' && span[1] == 's' && span[2] == 'p') return PropertyType.Bsp;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetFourCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("batb"u8)) return PropertyType.Batb;
        if (span.SequenceEqual("batl"u8)) return PropertyType.Batl;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetFiveCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("bdatl"u8)) return PropertyType.Bdatl;
        if (span.SequenceEqual("bdatb"u8)) return PropertyType.Bdatb;
        if (span.SequenceEqual("venue"u8)) return PropertyType.Venue;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetSixCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("status"u8)) return PropertyType.Status;
        if (span.SequenceEqual("closed"u8)) return PropertyType.Closed;
        if (span.SequenceEqual("inPlay"u8)) return PropertyType.InPlay;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetSevenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("version"u8)) return PropertyType.Version;
        if (span.SequenceEqual("runners"u8)) return PropertyType.Runners;
        if (span.SequenceEqual("eventId"u8)) return PropertyType.EventId;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetEightCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("timezone"u8)) return PropertyType.Timezone;
        if (span.SequenceEqual("openDate"u8)) return PropertyType.OpenDate;
        if (span.SequenceEqual("betDelay"u8)) return PropertyType.BetDelay;
        if (span.SequenceEqual("complete"u8)) return PropertyType.Complete;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetNineCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("accountId"u8)) return PropertyType.AccountId;
        if (span.SequenceEqual("bspMarket"u8)) return PropertyType.BspMarket;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetTenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("statusCode"u8)) return PropertyType.StatusCode;
        if (span.SequenceEqual("initialClk"u8)) return PropertyType.InitialClk;
        if (span.SequenceEqual("conflateMs"u8)) return PropertyType.ConflateMs;
        if (span.SequenceEqual("marketType"u8)) return PropertyType.MarketType;
        if (span.SequenceEqual("marketTime"u8)) return PropertyType.MarketTime;
        if (span.SequenceEqual("regulators"u8)) return PropertyType.Regulators;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetElevenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("segmentType"u8)) return PropertyType.SegmentType;
        if (span.SequenceEqual("bettingType"u8)) return PropertyType.BettingType;
        if (span.SequenceEqual("heartbeatMs"u8)) return PropertyType.HeartbeatMs;
        if (span.SequenceEqual("settledTime"u8)) return PropertyType.SettledTime;
        if (span.SequenceEqual("suspendTime"u8)) return PropertyType.SuspendTime;
        if (span.SequenceEqual("eventTypeId"u8)) return PropertyType.EventTypeId;
        if (span.SequenceEqual("removalDate"u8)) return PropertyType.RemovalDate;
        if (span.SequenceEqual("countryCode"u8)) return PropertyType.CountryCode;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetTwelveCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("connectionId"u8)) return PropertyType.ConnectionId;
        if (span.SequenceEqual("sortPriority"u8)) return PropertyType.SortPriority;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetThirteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("crossMatching"u8)) return PropertyType.CrossMatching;
        if (span.SequenceEqual("bspReconciled"u8)) return PropertyType.BspReconciled;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetFourteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("eachWayDivisor"u8)) return PropertyType.EachWayDivisor;
        if (span.SequenceEqual("marketBaseRate"u8)) return PropertyType.MarketBaseRate;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetFifteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("numberOfWinners"u8)) return PropertyType.NumberOfWinners;
        if (span.SequenceEqual("runnersVoidable"u8)) return PropertyType.RunnersVoidable;
        if (span.SequenceEqual("discountAllowed"u8)) return PropertyType.DiscountAllowed;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetSixteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("marketDefinition"u8)) return PropertyType.MarketDefinition;
        if (span.SequenceEqual("connectionClosed"u8)) return PropertyType.ConnectionClosed;
        if (span.SequenceEqual("adjustmentFactor"u8)) return PropertyType.AdjustmentFactor;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetSeventeenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("turnInPlayEnabled"u8)) return PropertyType.TurnInPlayEnabled;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetEighteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("persistenceEnabled"u8)) return PropertyType.PersistenceEnabled;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetNineteenCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("connectionsAvailable"u8)) return PropertyType.ConnectionsAvailable;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetTwentyCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("numberOfActiveRunners"u8)) return PropertyType.NumberOfActiveRunners;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PropertyType GetTwentyOneCharProperty(ReadOnlySpan<byte> span)
    {
        if (span.SequenceEqual("numberOfActiveRunners"u8)) return PropertyType.NumberOfActiveRunners;
        return PropertyType.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChangeMessage? DeserializeChangeMessage(byte[] lineBytes)
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
            var reader = new FastJsonReader(lineSpan);
            return ReadChangeMessageOptimized(ref reader);
        }
        catch
        {
            // Skip invalid JSON messages
            return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ChangeMessage? ReadChangeMessageOptimized(ref FastJsonReader reader)
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

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertyType = GetPropertyType(reader.ValueSpan);
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                switch (propertyType)
                {
                    case PropertyType.Op:
                        operation = reader.GetString();
                        break;
                    case PropertyType.Id:
                        id = reader.GetInt32();
                        break;
                    case PropertyType.InitialClk:
                        initialClock = reader.GetString();
                        break;
                    case PropertyType.Clk:
                        clock = reader.GetString();
                        break;
                    case PropertyType.Pt:
                        publishTime = reader.GetInt64();
                        break;
                    case PropertyType.Ct:
                        changeType = reader.GetString();
                        break;
                    case PropertyType.StatusCode:
                        statusCode = reader.GetString();
                        break;
                    case PropertyType.ErrorCode:
                        errorCode = reader.GetString();
                        break;
                    case PropertyType.ConnectionId:
                        connectionId = reader.GetString();
                        break;
                    case PropertyType.ConnectionClosed:
                        connectionClosed = reader.GetBoolean();
                        break;
                    case PropertyType.ConnectionsAvailable:
                        connectionsAvailable = reader.GetInt32();
                        break;
                    case PropertyType.ConflateMs:
                        conflateMs = reader.GetInt32();
                        break;
                    case PropertyType.HeartbeatMs:
                        heartbeatMs = reader.GetInt32();
                        break;
                    case PropertyType.SegmentType:
                        segmentType = reader.GetString();
                        break;
                    case PropertyType.Mc:
                        marketChanges = ReadMarketChangesOptimized(ref reader);
                        break;
                    case PropertyType.Oc:
                        orderChanges = ReadOrderChangesOptimized(ref reader);
                        break;
                    default:
                        reader.Skip();
                        break;
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
            OrderChanges = orderChanges,
        };
    }

    /// <summary>
    /// Reads market changes array with optimized performance using FastJsonReader.
    /// </summary>
    private List<MarketChange>? ReadMarketChangesOptimized(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var marketChanges = GetPooledList(_marketChangeListPool);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var marketChange = ReadMarketChangeOptimized(ref reader);
                if (marketChange != null)
                    marketChanges.Add(marketChange);
            }
        }

        return marketChanges.Count > 0 ? marketChanges : null;
    }

    /// <summary>
    /// Reads order changes array with optimized performance using FastJsonReader.
    /// </summary>
    private List<OrderChange>? ReadOrderChangesOptimized(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var orderChanges = GetPooledList(_orderChangeListPool);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var orderChange = ReadOrderChangeOptimized(ref reader);
                if (orderChange != null)
                    orderChanges.Add(orderChange);
            }
        }

        return orderChanges.Count > 0 ? orderChanges : null;
    }

    /// <summary>
    /// Reads a MarketChange using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MarketChange? ReadMarketChangeOptimized(ref FastJsonReader reader)
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

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertyType = GetPropertyType(reader.ValueSpan);
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                switch (propertyType)
                {
                    case PropertyType.Id:
                        marketId = reader.GetString();
                        break;
                    case PropertyType.Tv:
                        totalAmountMatched = reader.GetDouble();
                        break;
                    case PropertyType.Rc:
                        runnerChanges = ReadRunnerChangesOptimized(ref reader);
                        break;
                    case PropertyType.Img:
                        replaceCache = reader.GetBoolean();
                        break;
                    case PropertyType.Con:
                        conflated = reader.GetBoolean();
                        break;
                    case PropertyType.MarketDefinition:
                        marketDefinition = ReadMarketDefinitionOptimized(ref reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        return new MarketChange
        {
            MarketId = marketId,
            TotalAmountMatched = totalAmountMatched,
            RunnerChanges = runnerChanges,
            ReplaceCache = replaceCache,
            Conflated = conflated,
            MarketDefinition = marketDefinition,
        };
    }



    /// <summary>
    /// Reads an OrderChange using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private OrderChange? ReadOrderChangeOptimized(ref FastJsonReader reader)
    {
        string? marketId = null;
        long? accountId = null;
        bool? closed = null;
        List<OrderRunnerChange>? orderRunnerChanges = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertyType = GetPropertyType(reader.ValueSpan);
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                switch (propertyType)
                {
                    case PropertyType.Id:
                        marketId = reader.GetString();
                        break;
                    case PropertyType.AccountId:
                        accountId = reader.GetInt64();
                        break;
                    case PropertyType.Closed:
                        closed = reader.GetBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        return new OrderChange
        {
            MarketId = marketId,
            AccountId = accountId,
            Closed = closed,
            OrderRunnerChanges = orderRunnerChanges,
        };
    }

    /// <summary>
    /// Reads runner changes array with optimized FastJsonReader performance.
    /// </summary>
    private List<RunnerChange>? ReadRunnerChangesOptimized(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var runnerChanges = GetPooledList(_runnerChangeListPool);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var runnerChange = ReadRunnerChangeOptimized(ref reader);
                if (runnerChange != null)
                    runnerChanges.Add(runnerChange);
            }
        }

        // Always return the list, even if empty, to match System.Text.Json behavior
        return runnerChanges;
    }

    /// <summary>
    /// Reads a RunnerChange using optimized FastJsonReader parsing.
    /// </summary>
    private RunnerChange? ReadRunnerChangeOptimized(ref FastJsonReader reader)
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

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertyType = GetPropertyType(reader.ValueSpan);
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                switch (propertyType)
                {
                    case PropertyType.Id:
                        selectionId = reader.GetInt64();
                        break;
                    case PropertyType.Tv:
                        totalMatched = reader.GetDouble();
                        break;
                    case PropertyType.Ltp:
                        lastTradedPrice = reader.GetDouble();
                        break;
                    case PropertyType.Spf:
                        startingPriceFar = reader.GetDouble();
                        break;
                    case PropertyType.Spn:
                        startingPriceNear = reader.GetDouble();
                        break;
                    case PropertyType.Hc:
                        handicap = reader.GetDouble();
                        break;
                    case PropertyType.Batb:
                        bestAvailableToBack = ReadDoubleArraysOptimized(ref reader);
                        break;
                    case PropertyType.Spb:
                        startingPriceBack = ReadDoubleArraysOptimized(ref reader);
                        break;
                    case PropertyType.Bdatl:
                        bestDisplayAvailableToLay = ReadDoubleArraysOptimized(ref reader);
                        break;
                    case PropertyType.Trd:
                        traded = ReadDoubleArraysOptimized(ref reader);
                        break;
                    case PropertyType.Atb:
                        availableToBack = ReadDoubleArraysOptimized(ref reader);
                        break;
                    case PropertyType.Spl:
                        startingPriceLay = ReadDoubleArraysOptimized(ref reader);
                        break;
                    case PropertyType.Atl:
                        availableToLay = ReadDoubleArraysOptimized(ref reader);
                        break;
                    case PropertyType.Batl:
                        bestAvailableToLay = ReadDoubleArraysOptimized(ref reader);
                        break;
                    case PropertyType.Bdatb:
                        bestDisplayAvailableToBack = ReadDoubleArraysOptimized(ref reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
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
            BestDisplayAvailableToBack = bestDisplayAvailableToBack,
        };
    }



    /// <summary>
    /// Reads arrays of double arrays using optimized FastJsonReader.
    /// </summary>
    private List<List<double>>? ReadDoubleArraysOptimized(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var result = GetPooledList(_doubleArrayListPool);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var innerArray = ReadDoubleArrayOptimized(ref reader);
                if (innerArray != null && innerArray.Count > 0)
                    result.Add(innerArray);
            }
        }

        // Always return the list, even if empty, to match System.Text.Json behavior
        return result;
    }

    /// <summary>
    /// Reads a single array of doubles using optimized FastJsonReader.
    /// </summary>
    private List<double>? ReadDoubleArrayOptimized(ref FastJsonReader reader)
    {
        var result = GetPooledList(_doubleListPool);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.Number)
            {
                result.Add(reader.GetDouble());
            }
        }

        // Don't return to pool here since we're returning the list
        return result.Count > 0 ? result : null;
    }

    /// <summary>
    /// Reads a MarketDefinition using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MarketDefinition? ReadMarketDefinitionOptimized(ref FastJsonReader reader)
    {
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

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertyName = reader.ValueSpan;
                var propertyType = GetPropertyType(propertyName);
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                switch (propertyType)
                {
                    case PropertyType.BspMarket:
                        bspMarket = reader.GetBoolean();
                        break;
                    case PropertyType.TurnInPlayEnabled:
                        turnInPlayEnabled = reader.GetBoolean();
                        break;
                    case PropertyType.PersistenceEnabled:
                        persistenceEnabled = reader.GetBoolean();
                        break;
                    case PropertyType.MarketBaseRate:
                        marketBaseRate = reader.GetNullableDouble();
                        break;
                    case PropertyType.BettingType:
                        bettingType = reader.GetString();
                        break;
                    case PropertyType.Status:
                        status = reader.GetString();
                        break;
                    case PropertyType.Venue:
                        venue = reader.GetString();
                        break;
                    case PropertyType.Timezone:
                        timezone = reader.GetString();
                        break;
                    case PropertyType.SettledTime:
                        settledTime = reader.GetNullableDateTime();
                        break;
                    case PropertyType.SuspendTime:
                        suspendTime = reader.GetNullableDateTime();
                        break;
                    case PropertyType.OpenDate:
                        openDate = reader.GetNullableDateTime();
                        break;
                    case PropertyType.MarketTime:
                        marketTime = reader.GetNullableDateTime();
                        break;
                    case PropertyType.EachWayDivisor:
                        eachWayDivisor = reader.GetNullableDouble();
                        break;
                    case PropertyType.Regulators:
                        regulators = ReadStringArrayOptimized(ref reader);
                        break;
                    case PropertyType.MarketType:
                        marketType = reader.GetString();
                        break;
                    case PropertyType.NumberOfWinners:
                        numberOfWinners = reader.GetInt32();
                        break;
                    case PropertyType.CountryCode:
                        countryCode = reader.GetString();
                        break;
                    case PropertyType.InPlay:
                        inPlay = reader.GetBoolean();
                        break;
                    case PropertyType.BetDelay:
                        betDelay = reader.GetInt32();
                        break;
                    case PropertyType.NumberOfActiveRunners:
                        numberOfActiveRunners = reader.GetInt32();
                        break;
                    case PropertyType.EventId:
                        eventId = reader.GetString();
                        break;
                    case PropertyType.CrossMatching:
                        crossMatching = reader.GetBoolean();
                        break;
                    case PropertyType.RunnersVoidable:
                        runnersVoidable = reader.GetBoolean();
                        break;
                    case PropertyType.DiscountAllowed:
                        discountAllowed = reader.GetBoolean();
                        break;
                    case PropertyType.Runners:
                        runners = ReadRunnerDefinitionsOptimized(ref reader);
                        break;
                    case PropertyType.Version:
                        version = reader.GetInt64();
                        break;
                    case PropertyType.EventTypeId:
                        eventTypeId = reader.GetString();
                        break;
                    case PropertyType.Complete:
                        complete = reader.GetBoolean();
                        break;
                    case PropertyType.BspReconciled:
                        bspReconciled = reader.GetBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
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
            BspReconciled = bspReconciled,
        };
    }

    /// <summary>
    /// Reads an array of strings using optimized FastJsonReader.
    /// </summary>
    private List<string>? ReadStringArrayOptimized(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var result = GetPooledList(_stringListPool);

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

        // Always return the list, even if empty, to match System.Text.Json behavior
        return result;
    }

    /// <summary>
    /// Reads runner definitions array using optimized FastJsonReader.
    /// </summary>
    private List<RunnerDefinition>? ReadRunnerDefinitionsOptimized(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var runners = GetPooledList(_runnerDefinitionListPool);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var runner = ReadRunnerDefinitionOptimized(ref reader);
                if (runner != null)
                    runners.Add(runner);
            }
        }

        // Always return the list, even if empty, to match System.Text.Json behavior
        return runners;
    }

    /// <summary>
    /// Reads a RunnerDefinition using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private RunnerDefinition? ReadRunnerDefinitionOptimized(ref FastJsonReader reader)
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

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertyType = GetPropertyType(reader.ValueSpan);
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                switch (propertyType)
                {
                    case PropertyType.Status:
                        status = reader.GetString();
                        break;
                    case PropertyType.SortPriority:
                        sortPriority = reader.GetInt32();
                        break;
                    case PropertyType.RemovalDate:
                        removalDate = reader.GetDateTime();
                        break;
                    case PropertyType.Id:
                        selectionId = reader.GetInt64();
                        break;
                    case PropertyType.Hc:
                        handicap = reader.GetDouble();
                        break;
                    case PropertyType.AdjustmentFactor:
                        adjustmentFactor = reader.GetDouble();
                        break;
                    case PropertyType.Bsp:
                        bspLiability = reader.GetDouble();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
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
            BspLiability = bspLiability,
        };
    }
}
