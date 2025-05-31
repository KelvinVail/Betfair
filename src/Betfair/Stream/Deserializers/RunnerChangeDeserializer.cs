using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// High-performance deserializer for RunnerChange objects.
/// </summary>
internal static class RunnerChangeDeserializer
{
    /// <summary>
    /// Reads runner changes array with optimized FastJsonReader performance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<RunnerChange>? ReadRunnerChanges(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var runnerChanges = ObjectPools.GetRunnerChangeList();

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

        // Always return the list, even if empty, to match System.Text.Json behavior
        return runnerChanges;
    }

    /// <summary>
    /// Reads a RunnerChange using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static RunnerChange? ReadRunnerChange(ref FastJsonReader reader)
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
                var propertyType = PropertyLookup.GetPropertyType(reader.ValueSpan);
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
                        bestAvailableToBack = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    case PropertyType.Spb:
                        startingPriceBack = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    case PropertyType.Bdatl:
                        bestDisplayAvailableToLay = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    case PropertyType.Trd:
                        traded = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    case PropertyType.Atb:
                        availableToBack = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    case PropertyType.Spl:
                        startingPriceLay = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    case PropertyType.Atl:
                        availableToLay = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    case PropertyType.Batl:
                        bestAvailableToLay = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    case PropertyType.Bdatb:
                        bestDisplayAvailableToBack = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        break;
                    default:
                        reader.SkipValue();
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
}
