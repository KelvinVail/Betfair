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
    /// Ultra-optimized ChangeMessage reader using FastJsonReader with enhanced pattern recognition.
    /// Uses pre-computed patterns and optimized property order for maximum performance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChangeMessage? ReadChangeMessageOptimized(ref FastJsonReader reader)
    {
        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
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

        // Ultra-fast parsing optimized for MarketStream.txt property order
        // Properties typically appear in this order: op, id, initialClk/clk, conflateMs, heartbeatMs, pt, ct, mc
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertySpan = reader.ValueSpan;
                if (!reader.Read()) // Move to value
                    break;

                // Ultra-fast property matching using optimized patterns
                // Order by frequency in MarketStream.txt for maximum performance
                if (propertySpan.Length == 2)
                {
                    // Most common 2-char properties: op, id, pt, ct, mc
                    var first = propertySpan[0];
                    var second = propertySpan[1];

                    if (first == 'o' && second == 'p')
                        operation = reader.GetString();
                    else if (first == 'i' && second == 'd')
                        id = reader.GetInt32();
                    else if (first == 'p' && second == 't')
                        publishTime = reader.GetInt64();
                    else if (first == 'c' && second == 't')
                        changeType = reader.GetString();
                    else if (first == 'm' && second == 'c')
                        marketChanges = MarketChangeDeserializer.ReadMarketChanges(ref reader);
                    else
                        reader.SkipValue();
                }
                else if (propertySpan.Length == 3 && propertySpan[0] == 'c' && propertySpan[1] == 'l' && propertySpan[2] == 'k')
                {
                    // clk property
                    clock = reader.GetString();
                }
                else if (propertySpan.Length == 10)
                {
                    // initialClk or conflateMs
                    if (propertySpan[0] == 'i' && propertySpan.SequenceEqual("initialClk"u8))
                        initialClock = reader.GetString();
                    else if (propertySpan[0] == 'c' && propertySpan.SequenceEqual("conflateMs"u8))
                        conflateMs = reader.GetInt32();
                    else
                        reader.SkipValue();
                }
                else if (propertySpan.Length == 11 && propertySpan[0] == 'h' && propertySpan.SequenceEqual("heartbeatMs"u8))
                {
                    // heartbeatMs property
                    heartbeatMs = reader.GetInt32();
                }
                else
                {
                    // Unknown property - skip it
                    reader.SkipValue();
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
