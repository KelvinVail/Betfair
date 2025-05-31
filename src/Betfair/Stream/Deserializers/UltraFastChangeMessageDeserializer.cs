using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// Ultra-high-performance deserializer for ChangeMessage objects.
/// Uses direct byte scanning and fixed property order assumptions for maximum speed.
/// Optimized specifically for MarketStream.txt patterns.
/// Performance target: comparable to reading raw bytes from array.
/// </summary>
internal static class UltraFastChangeMessageDeserializer
{

    /// <summary>
    /// Ultra-fast deserialize using optimized FastJsonReader with pattern recognition.
    /// Optimized for the exact patterns found in MarketStream.txt.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ChangeMessage? Deserialize(ReadOnlySpan<byte> jsonBytes)
    {
        if (jsonBytes.Length < 10) // Minimum viable JSON size
            return null;

        try
        {
            // Use optimized FastJsonReader with enhanced pattern recognition
            var reader = new FastJsonReader(jsonBytes);
            return ReadChangeMessageOptimized(ref reader);
        }
        catch
        {
            // Fallback to original parser for edge cases
            return ChangeMessageDeserializer.Deserialize(jsonBytes);
        }
    }






    /// <summary>
    /// Ultra-optimized ChangeMessage reader using direct byte-level parsing.
    /// Optimized specifically for MarketStream.txt patterns with maximum performance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChangeMessage? ReadChangeMessageOptimized(ref FastJsonReader reader)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return null;

        // Pre-allocate object for direct property setting
        var message = new ChangeMessage();

        // Ultra-fast parsing optimized for MarketStream.txt property order
        // Properties appear in this exact order: op, id, initialClk/clk, conflateMs, heartbeatMs, pt, ct, mc
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertySpan = reader.ValueSpan;
                if (!reader.Read()) // Move to value
                    break;

                // Ultra-fast property matching using direct byte comparisons
                // Optimized for the exact patterns in MarketStream.txt
                switch (propertySpan.Length)
                {
                    case 2:
                        // Most frequent 2-char properties: op, id, pt, ct, mc
                        var b0 = propertySpan[0];
                        var b1 = propertySpan[1];

                        if (b0 == (byte)'o' && b1 == (byte)'p')
                            message.Operation = reader.GetString();
                        else if (b0 == (byte)'i' && b1 == (byte)'d')
                            message.Id = reader.GetInt32();
                        else if (b0 == (byte)'p' && b1 == (byte)'t')
                            message.PublishTime = reader.GetInt64();
                        else if (b0 == (byte)'c' && b1 == (byte)'t')
                            message.ChangeType = reader.GetString();
                        else if (b0 == (byte)'m' && b1 == (byte)'c')
                            message.MarketChanges = ReadMarketChangesOptimized(ref reader);
                        else
                            reader.SkipValue();
                        break;

                    case 3:
                        // clk property
                        if (propertySpan[0] == (byte)'c' && propertySpan[1] == (byte)'l' && propertySpan[2] == (byte)'k')
                            message.Clock = reader.GetString();
                        else
                            reader.SkipValue();
                        break;

                    case 10:
                        // initialClk or conflateMs
                        if (propertySpan[0] == (byte)'i' && propertySpan[1] == (byte)'n' && propertySpan[2] == (byte)'i' &&
                            propertySpan[3] == (byte)'t' && propertySpan[4] == (byte)'i' && propertySpan[5] == (byte)'a' &&
                            propertySpan[6] == (byte)'l' && propertySpan[7] == (byte)'C' && propertySpan[8] == (byte)'l' &&
                            propertySpan[9] == (byte)'k')
                            message.InitialClock = reader.GetString();
                        else if (propertySpan[0] == (byte)'c' && propertySpan[1] == (byte)'o' && propertySpan[2] == (byte)'n' &&
                                 propertySpan[3] == (byte)'f' && propertySpan[4] == (byte)'l' && propertySpan[5] == (byte)'a' &&
                                 propertySpan[6] == (byte)'t' && propertySpan[7] == (byte)'e' && propertySpan[8] == (byte)'M' &&
                                 propertySpan[9] == (byte)'s')
                            message.ConflateMs = reader.GetInt32();
                        else
                            reader.SkipValue();
                        break;

                    case 11:
                        // heartbeatMs
                        if (propertySpan[0] == (byte)'h' && propertySpan[1] == (byte)'e' && propertySpan[2] == (byte)'a' &&
                            propertySpan[3] == (byte)'r' && propertySpan[4] == (byte)'t' && propertySpan[5] == (byte)'b' &&
                            propertySpan[6] == (byte)'e' && propertySpan[7] == (byte)'a' && propertySpan[8] == (byte)'t' &&
                            propertySpan[9] == (byte)'M' && propertySpan[10] == (byte)'s')
                            message.HeartbeatMs = reader.GetInt32();
                        else
                            reader.SkipValue();
                        break;

                    default:
                        // Unknown property - skip it
                        reader.SkipValue();
                        break;
                }
            }
        }

        return message;
    }

    /// <summary>
    /// Ultra-optimized MarketChanges reader for MarketStream.txt patterns.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<MarketChange>? ReadMarketChangesOptimized(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var marketChanges = new List<MarketChange>(1); // Most common case is 1 market change

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

        // Always return a list, even if empty, to match System.Text.Json behavior
        return marketChanges;
    }

    /// <summary>
    /// Ultra-optimized MarketChange reader for MarketStream.txt patterns.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static MarketChange? ReadMarketChangeOptimized(ref FastJsonReader reader)
    {
        var marketChange = new MarketChange();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertySpan = reader.ValueSpan;
                if (!reader.Read()) // Move to value
                    break;

                // Optimized property matching for MarketChange
                switch (propertySpan.Length)
                {
                    case 2:
                        var b0 = propertySpan[0];
                        var b1 = propertySpan[1];

                        if (b0 == (byte)'i' && b1 == (byte)'d')
                            marketChange.MarketId = reader.GetString();
                        else if (b0 == (byte)'r' && b1 == (byte)'c')
                            marketChange.RunnerChanges = ReadRunnerChangesOptimized(ref reader);
                        else if (b0 == (byte)'t' && b1 == (byte)'v')
                            marketChange.TotalAmountMatched = reader.GetNullableDouble();
                        else
                            reader.SkipValue();
                        break;

                    case 3:
                        // img property
                        if (propertySpan[0] == (byte)'i' && propertySpan[1] == (byte)'m' && propertySpan[2] == (byte)'g')
                            marketChange.ReplaceCache = reader.GetBoolean();
                        else if (propertySpan[0] == (byte)'c' && propertySpan[1] == (byte)'o' && propertySpan[2] == (byte)'n')
                            marketChange.Conflated = reader.GetBoolean();
                        else
                            reader.SkipValue();
                        break;

                    case 16:
                        // marketDefinition
                        if (propertySpan[0] == (byte)'m' && propertySpan[1] == (byte)'a' && propertySpan[2] == (byte)'r' &&
                            propertySpan[3] == (byte)'k' && propertySpan[4] == (byte)'e' && propertySpan[5] == (byte)'t' &&
                            propertySpan[6] == (byte)'D' && propertySpan[7] == (byte)'e' && propertySpan[8] == (byte)'f' &&
                            propertySpan[9] == (byte)'i' && propertySpan[10] == (byte)'n' && propertySpan[11] == (byte)'i' &&
                            propertySpan[12] == (byte)'t' && propertySpan[13] == (byte)'i' && propertySpan[14] == (byte)'o' &&
                            propertySpan[15] == (byte)'n')
                            marketChange.MarketDefinition = MarketDefinitionDeserializer.ReadMarketDefinition(ref reader);
                        else
                            reader.SkipValue();
                        break;

                    default:
                        reader.SkipValue();
                        break;
                }
            }
        }

        return marketChange;
    }

    /// <summary>
    /// Ultra-optimized RunnerChanges reader for MarketStream.txt patterns.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<RunnerChange>? ReadRunnerChangesOptimized(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var runnerChanges = new List<RunnerChange>(4); // Common case is 2-4 runner changes

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

        // Always return a list, even if empty, to match System.Text.Json behavior
        return runnerChanges;
    }

    /// <summary>
    /// Ultra-optimized RunnerChange reader for MarketStream.txt patterns.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static RunnerChange? ReadRunnerChangeOptimized(ref FastJsonReader reader)
    {
        var runnerChange = new RunnerChange();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertySpan = reader.ValueSpan;
                if (!reader.Read()) // Move to value
                    break;

                // Ultra-fast property matching for RunnerChange
                switch (propertySpan.Length)
                {
                    case 2:
                        var b0 = propertySpan[0];
                        var b1 = propertySpan[1];

                        if (b0 == (byte)'i' && b1 == (byte)'d')
                            runnerChange.SelectionId = reader.GetInt64();
                        else if (b0 == (byte)'t' && b1 == (byte)'v')
                            runnerChange.TotalMatched = reader.GetNullableDouble();
                        else if (b0 == (byte)'h' && b1 == (byte)'c')
                            runnerChange.Handicap = reader.GetNullableDouble();
                        else
                            reader.SkipValue();
                        break;

                    case 3:
                        var c0 = propertySpan[0];
                        var c1 = propertySpan[1];
                        var c2 = propertySpan[2];

                        if (c0 == (byte)'a' && c1 == (byte)'t' && c2 == (byte)'b')
                            runnerChange.AvailableToBack = ReadDoubleArrayArray(ref reader);
                        else if (c0 == (byte)'a' && c1 == (byte)'t' && c2 == (byte)'l')
                            runnerChange.AvailableToLay = ReadDoubleArrayArray(ref reader);
                        else if (c0 == (byte)'t' && c1 == (byte)'r' && c2 == (byte)'d')
                            runnerChange.Traded = ReadDoubleArrayArray(ref reader);
                        else if (c0 == (byte)'l' && c1 == (byte)'t' && c2 == (byte)'p')
                            runnerChange.LastTradedPrice = reader.GetNullableDouble();
                        else if (c0 == (byte)'s' && c1 == (byte)'p' && c2 == (byte)'f')
                            runnerChange.StartingPriceFar = reader.GetNullableDouble();
                        else if (c0 == (byte)'s' && c1 == (byte)'p' && c2 == (byte)'n')
                            runnerChange.StartingPriceNear = reader.GetNullableDouble();
                        else if (c0 == (byte)'s' && c1 == (byte)'p' && c2 == (byte)'b')
                            runnerChange.StartingPriceBack = ReadDoubleArrayArray(ref reader);
                        else if (c0 == (byte)'s' && c1 == (byte)'p' && c2 == (byte)'l')
                            runnerChange.StartingPriceLay = ReadDoubleArrayArray(ref reader);
                        else
                            reader.SkipValue();
                        break;

                    case 4:
                        // batb, batl properties
                        if (propertySpan[0] == (byte)'b' && propertySpan[1] == (byte)'a' &&
                            propertySpan[2] == (byte)'t' && propertySpan[3] == (byte)'b')
                            runnerChange.BestAvailableToBack = ReadDoubleArrayArray(ref reader);
                        else if (propertySpan[0] == (byte)'b' && propertySpan[1] == (byte)'a' &&
                                 propertySpan[2] == (byte)'t' && propertySpan[3] == (byte)'l')
                            runnerChange.BestAvailableToLay = ReadDoubleArrayArray(ref reader);
                        else
                            reader.SkipValue();
                        break;

                    case 5:
                        // bdatb, bdatl properties
                        if (propertySpan[0] == (byte)'b' && propertySpan[1] == (byte)'d' &&
                            propertySpan[2] == (byte)'a' && propertySpan[3] == (byte)'t' && propertySpan[4] == (byte)'b')
                            runnerChange.BestDisplayAvailableToBack = ReadDoubleArrayArray(ref reader);
                        else if (propertySpan[0] == (byte)'b' && propertySpan[1] == (byte)'d' &&
                                 propertySpan[2] == (byte)'a' && propertySpan[3] == (byte)'t' && propertySpan[4] == (byte)'l')
                            runnerChange.BestDisplayAvailableToLay = ReadDoubleArrayArray(ref reader);
                        else
                            reader.SkipValue();
                        break;

                    default:
                        reader.SkipValue();
                        break;
                }
            }
        }

        return runnerChange;
    }

    /// <summary>
    /// Ultra-fast double array array reader for price/size data.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<List<double>>? ReadDoubleArrayArray(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var result = new List<List<double>>(3); // Common case is 0-3 price levels

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var innerArray = new List<double>(3); // Price, size, and optional third value

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (reader.TokenType == JsonTokenType.Number)
                        innerArray.Add(reader.GetDouble());
                }

                if (innerArray.Count > 0)
                    result.Add(innerArray);
            }
        }

        // Always return a list, even if empty, to match System.Text.Json behavior
        return result;
    }
}
