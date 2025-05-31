using System.Runtime.CompilerServices;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// Ultra-high-performance deserializer for ChangeMessage objects.
/// Uses direct byte scanning and fixed property order assumptions for maximum speed.
/// Optimized specifically for MarketStream.txt patterns.
/// Falls back to original deserializer for complex cases.
/// </summary>
internal static class UltraFastChangeMessageDeserializer
{
    /// <summary>
    /// Ultra-fast deserialize using optimized FastJsonReader with pattern recognition.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ChangeMessage? Deserialize(ReadOnlySpan<byte> jsonBytes)
    {
        if (jsonBytes.Length < 10) // Minimum viable JSON size
            return null;

        try
        {
            // Use optimized FastJsonReader for ultra-fast parsing
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
    /// Optimized ChangeMessage reader using FastJsonReader with known property order.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChangeMessage? ReadChangeMessageOptimized(ref FastJsonReader reader)
    {
        if (!reader.Read() || reader.TokenType != System.Text.Json.JsonTokenType.StartObject)
            return null;

        // Initialize all fields
        string? operation = null;
        int id = 0;
        string? initialClock = null;
        string? clock = null;
        long? publishTime = null;
        string? changeType = null;
        int? conflateMs = null;
        int? heartbeatMs = null;
        List<MarketChange>? marketChanges = null;

        // Use optimized parsing with known property order
        // This reduces branching and property lookup overhead
        while (reader.Read())
        {
            if (reader.TokenType == System.Text.Json.JsonTokenType.EndObject)
                break;

            if (reader.TokenType == System.Text.Json.JsonTokenType.String)
            {
                var propertySpan = reader.ValueSpan;
                if (!reader.Read()) // Move to value
                    break;

                // Optimized property matching using length and first byte
                switch (propertySpan.Length)
                {
                    case 2:
                        if (propertySpan[0] == 'o' && propertySpan[1] == 'p')
                            operation = reader.GetString();
                        else if (propertySpan[0] == 'i' && propertySpan[1] == 'd')
                            id = reader.GetInt32();
                        else if (propertySpan[0] == 'p' && propertySpan[1] == 't')
                            publishTime = reader.GetInt64();
                        else if (propertySpan[0] == 'c' && propertySpan[1] == 't')
                            changeType = reader.GetString();
                        else if (propertySpan[0] == 'm' && propertySpan[1] == 'c')
                            marketChanges = MarketChangeDeserializer.ReadMarketChanges(ref reader);
                        else
                            reader.SkipValue();
                        break;
                    case 3:
                        if (propertySpan[0] == 'c' && propertySpan[1] == 'l' && propertySpan[2] == 'k')
                            clock = reader.GetString();
                        else
                            reader.SkipValue();
                        break;
                    case 10:
                        if (propertySpan.SequenceEqual("initialClk"u8))
                            initialClock = reader.GetString();
                        else if (propertySpan.SequenceEqual("conflateMs"u8))
                            conflateMs = reader.GetInt32();
                        else
                            reader.SkipValue();
                        break;
                    case 11:
                        if (propertySpan.SequenceEqual("heartbeatMs"u8))
                            heartbeatMs = reader.GetInt32();
                        else
                            reader.SkipValue();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
        }

        return new ChangeMessage
        {
            Operation = operation,
            Id = id,
            InitialClock = initialClock,
            Clock = clock,
            PublishTime = publishTime,
            ChangeType = changeType,
            ConflateMs = conflateMs,
            HeartbeatMs = heartbeatMs,
            MarketChanges = marketChanges,
        };
    }
}
