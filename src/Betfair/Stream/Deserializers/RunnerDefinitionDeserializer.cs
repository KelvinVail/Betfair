using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// High-performance deserializer for RunnerDefinition objects.
/// </summary>
internal static class RunnerDefinitionDeserializer
{
    /// <summary>
    /// Reads an array of RunnerDefinition objects using optimized FastJsonReader.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<RunnerDefinition>? ReadRunnerDefinitions(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var runners = ObjectPools.GetRunnerDefinitionList();

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

        // Always return the list, even if empty, to match System.Text.Json behavior
        return runners;
    }

    /// <summary>
    /// Reads a RunnerDefinition using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static RunnerDefinition? ReadRunnerDefinition(ref FastJsonReader reader)
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
                var propertyType = PropertyLookup.GetPropertyType(reader.ValueSpan);
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
                        removalDate = reader.GetNullableDateTime();
                        break;
                    case PropertyType.Id:
                        selectionId = reader.GetInt64();
                        break;
                    case PropertyType.Hc:
                        handicap = reader.GetNullableDouble();
                        break;
                    case PropertyType.AdjustmentFactor:
                        adjustmentFactor = reader.GetNullableDouble();
                        break;
                    case PropertyType.Bsp:
                        bspLiability = reader.GetNullableDouble();
                        break;
                    default:
                        reader.SkipValue();
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
