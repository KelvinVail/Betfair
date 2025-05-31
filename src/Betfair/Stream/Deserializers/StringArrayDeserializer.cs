using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// High-performance deserializer for string arrays.
/// </summary>
internal static class StringArrayDeserializer
{
    /// <summary>
    /// Reads an array of strings using optimized FastJsonReader.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<string>? ReadStringArray(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var result = ObjectPools.GetStringList();

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
}
