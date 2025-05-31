using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// High-performance deserializer for double arrays.
/// </summary>
internal static class DoubleArrayDeserializer
{
    /// <summary>
    /// Reads arrays of double arrays using optimized FastJsonReader.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<List<double>>? ReadDoubleArrays(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var result = ObjectPools.GetDoubleArrayList();

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

        // Always return the list, even if empty, to match System.Text.Json behavior
        return result;
    }

    /// <summary>
    /// Reads a single array of doubles using optimized FastJsonReader.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<double>? ReadDoubleArray(ref FastJsonReader reader)
    {
        var result = ObjectPools.GetDoubleList();

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
}
