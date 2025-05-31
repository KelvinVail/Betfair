using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// High-performance deserializer for OrderRunnerChange objects.
/// </summary>
internal static class OrderRunnerChangeDeserializer
{
    /// <summary>
    /// Reads order runner changes array with optimized performance using FastJsonReader.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<OrderRunnerChange>? ReadOrderRunnerChanges(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var orderRunnerChanges = new List<OrderRunnerChange>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var orderRunnerChange = ReadOrderRunnerChange(ref reader);
                if (orderRunnerChange != null)
                    orderRunnerChanges.Add(orderRunnerChange);
            }
        }

        return orderRunnerChanges.Count > 0 ? orderRunnerChanges : null;
    }

    /// <summary>
    /// Reads an OrderRunnerChange using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static OrderRunnerChange? ReadOrderRunnerChange(ref FastJsonReader reader)
    {
        long selectionId = 0;
        bool? fullImage = null;
        List<List<double>>? matchedLays = null;
        List<List<double>>? matchedBacks = null;
        double? handicap = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertyType = PropertyLookup.GetPropertyType(reader.ValueSpan);
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                switch (propertyType)
                {
                    case PropertyType.Id:
                        selectionId = reader.GetInt64();
                        break;
                    case PropertyType.Hc:
                        handicap = reader.GetNullableDouble();
                        break;
                    case PropertyType.FullImage:
                        fullImage = reader.GetBoolean();
                        break;
                    case PropertyType.Ml:
                        matchedLays = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    case PropertyType.Mb:
                        matchedBacks = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
        }

        return new OrderRunnerChange
        {
            SelectionId = selectionId,
            FullImage = fullImage,
            MatchedLays = matchedLays,
            MatchedBacks = matchedBacks,
            Handicap = handicap,
        };
    }
}
