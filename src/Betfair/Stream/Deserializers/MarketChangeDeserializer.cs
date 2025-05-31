using System.Runtime.CompilerServices;
using System.Text.Json;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// Ultra-high-performance deserializer for MarketChange objects.
/// Optimized for known property order in MarketStream.txt.
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
    /// Reads a MarketChange using ultra-fast optimized parsing with direct property matching.
    /// Optimized for the exact property order found in MarketStream.txt.
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
                var propertySpan = reader.ValueSpan;
                if (!reader.Read()) // Move to value
                    throw new JsonException("Incomplete JSON: expected value after property name");

                // Ultra-fast property matching using length and first byte
                // Based on MarketStream.txt analysis: id, marketDefinition, rc, img, tv
                switch (propertySpan.Length)
                {
                    case 2:
                        if (propertySpan[0] == 'i' && propertySpan[1] == 'd')
                            marketId = reader.GetString();
                        else if (propertySpan[0] == 'r' && propertySpan[1] == 'c')
                            runnerChanges = RunnerChangeDeserializer.ReadRunnerChanges(ref reader);
                        else if (propertySpan[0] == 't' && propertySpan[1] == 'v')
                            totalAmountMatched = reader.GetDouble();
                        else
                            reader.SkipValue();
                        break;
                    case 3:
                        if (propertySpan[0] == 'i' && propertySpan[1] == 'm' && propertySpan[2] == 'g')
                            replaceCache = reader.GetBoolean();
                        else if (propertySpan[0] == 'c' && propertySpan[1] == 'o' && propertySpan[2] == 'n')
                            conflated = reader.GetBoolean();
                        else
                            reader.SkipValue();
                        break;
                    case 16:
                        if (propertySpan.SequenceEqual("marketDefinition"u8))
                            marketDefinition = MarketDefinitionDeserializer.ReadMarketDefinition(ref reader);
                        else
                            reader.SkipValue();
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
