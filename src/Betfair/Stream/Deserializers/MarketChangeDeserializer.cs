using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// High-performance deserializer for MarketChange objects.
/// </summary>
internal static class MarketChangeDeserializer
{
    /// <summary>
    /// Reads market changes array with optimized performance using FastJsonReader.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<MarketChange>? ReadMarketChanges(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var marketChanges = ObjectPools.GetMarketChangeList();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var marketChange = ReadMarketChange(ref reader);
                if (marketChange != null)
                    marketChanges.Add(marketChange);
            }
        }

        return marketChanges.Count > 0 ? marketChanges : null;
    }

    /// <summary>
    /// Reads a MarketChange using optimized FastJsonReader parsing.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static MarketChange? ReadMarketChange(ref FastJsonReader reader)
    {
        string? marketId = null;
        double? totalAmountMatched = null;
        List<RunnerChange>? runnerChanges = null;
        bool? replaceCache = null;
        bool? conflated = null;
        MarketDefinition? marketDefinition = null;

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
                        marketId = reader.GetString();
                        break;
                    case PropertyType.Tv:
                        totalAmountMatched = reader.GetDouble();
                        break;
                    case PropertyType.Rc:
                        runnerChanges = RunnerChangeDeserializer.ReadRunnerChanges(ref reader);
                        break;
                    case PropertyType.Img:
                        replaceCache = reader.GetBoolean();
                        break;
                    case PropertyType.Con:
                        conflated = reader.GetBoolean();
                        break;
                    case PropertyType.MarketDefinition:
                        marketDefinition = MarketDefinitionDeserializer.ReadMarketDefinition(ref reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
        }

        return new MarketChange
        {
            MarketId = marketId,
            TotalAmountMatched = totalAmountMatched,
            RunnerChanges = runnerChanges,
            ReplaceCache = replaceCache,
            Conflated = conflated,
            MarketDefinition = marketDefinition,
        };
    }
}
