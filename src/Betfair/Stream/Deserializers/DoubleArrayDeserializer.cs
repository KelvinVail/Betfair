using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// Ultra-high-performance deserializer for double arrays.
/// Optimized specifically for Betfair stream patterns like [[11.5,55.61],[8.4,35]].
/// </summary>
internal static class DoubleArrayDeserializer
{
    /// <summary>
    /// Reads arrays of double arrays using ultra-fast optimized parsing.
    /// Assumes valid JSON structure as found in MarketStream.txt.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<List<double>>? ReadDoubleArrays(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var result = ObjectPools.GetDoubleArrayList();

        // Ultra-fast parsing loop with minimal overhead
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

        return result;
    }

    /// <summary>
    /// Reads a single array of doubles using ultra-fast optimized parsing.
    /// Optimized for typical Betfair patterns with 2-3 numbers per array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<double>? ReadDoubleArrayOptimized(ref FastJsonReader reader)
    {
        var result = ObjectPools.GetDoubleList();

        // Ultra-fast parsing loop optimized for small arrays (typically 2-3 elements)
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.Number)
            {
                // Use optimized double parsing
                result.Add(reader.GetDouble());
            }
        }

        return result.Count > 0 ? result : null;
    }
}
