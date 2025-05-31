using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// Ultra-high-performance deserializer for RunnerChange objects.
/// Optimized for known property order in MarketStream.txt.
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
                var propertySpan = reader.ValueSpan;
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                // Ultra-fast property matching using length and first byte
                // Based on MarketStream.txt analysis: atb, atl, trd, batb, batl, ltp, tv, id
                switch (propertySpan.Length)
                {
                    case 2:
                        if (propertySpan[0] == 'i' && propertySpan[1] == 'd')
                            selectionId = reader.GetInt64();
                        else if (propertySpan[0] == 't' && propertySpan[1] == 'v')
                            totalMatched = reader.GetDouble();
                        else if (propertySpan[0] == 'h' && propertySpan[1] == 'c')
                            handicap = reader.GetDouble();
                        else
                            reader.SkipValue();
                        break;
                    case 3:
                        if (propertySpan[0] == 'a' && propertySpan[1] == 't' && propertySpan[2] == 'b')
                            availableToBack = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        else if (propertySpan[0] == 'a' && propertySpan[1] == 't' && propertySpan[2] == 'l')
                            availableToLay = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        else if (propertySpan[0] == 't' && propertySpan[1] == 'r' && propertySpan[2] == 'd')
                            traded = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        else if (propertySpan[0] == 'l' && propertySpan[1] == 't' && propertySpan[2] == 'p')
                            lastTradedPrice = reader.GetDouble();
                        else if (propertySpan[0] == 's' && propertySpan[1] == 'p' && propertySpan[2] == 'f')
                            startingPriceFar = reader.GetDouble();
                        else if (propertySpan[0] == 's' && propertySpan[1] == 'p' && propertySpan[2] == 'n')
                            startingPriceNear = reader.GetDouble();
                        else if (propertySpan[0] == 's' && propertySpan[1] == 'p' && propertySpan[2] == 'b')
                            startingPriceBack = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        else if (propertySpan[0] == 's' && propertySpan[1] == 'p' && propertySpan[2] == 'l')
                            startingPriceLay = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        else
                            reader.SkipValue();
                        break;
                    case 4:
                        if (propertySpan.SequenceEqual("batb"u8))
                            bestAvailableToBack = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        else if (propertySpan.SequenceEqual("batl"u8))
                            bestAvailableToLay = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        else
                            reader.SkipValue();
                        break;
                    case 5:
                        if (propertySpan.SequenceEqual("bdatb"u8))
                            bestDisplayAvailableToBack = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        else if (propertySpan.SequenceEqual("bdatl"u8))
                            bestDisplayAvailableToLay = DoubleArrayDeserializer.ReadDoubleArrays(ref reader);
                        else
                            reader.SkipValue();
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
