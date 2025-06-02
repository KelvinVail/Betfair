using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

internal static class UltraFastChangeMessageDeserializer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ChangeMessage? Deserialize(ReadOnlySpan<byte> jsonBytes)
    {
        var reader = new FastJsonReader(jsonBytes);
        return ReadChangeMessageOptimized(ref reader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ChangeMessage? ReadChangeMessageOptimized(ref FastJsonReader reader)
    {
        var message = new ChangeMessage();

        // Ultra-fast parsing optimized for MarketStream.txt property order
        // Properties appear in this exact order: op, id, initialClk/clk, conflateMs, heartbeatMs, pt, ct, mc
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var propertySpan = reader.ValueSpan;
                if (!reader.Read()) break;

                // Ultra-fast property matching using direct byte comparisons
                // Optimized for the exact patterns in MarketStream.txt
                switch (propertySpan.Length)
                {
                    case 2:
                        // Most frequent 2-char properties: op, id, pt, ct, mc
                        var b0 = propertySpan[0];

                        if (b0 == (byte)'o')
                            message.Operation = reader.GetString();
                        else if (b0 == (byte)'i')
                            message.Id = reader.GetInt32();
                        else if (b0 == (byte)'p')
                            message.PublishTime = reader.GetInt64();
                        else if (b0 == (byte)'c')
                            message.ChangeType = reader.GetString();
                        else if (b0 == (byte)'m')
                            message.MarketChanges = ReadMarketChangesOptimized(ref reader);
                        break;

                    case 3:
                        // clk property
                        message.Clock = reader.GetString();
                        break;

                    case 10:
                        // initialClk or conflateMs
                        if (propertySpan[0] == (byte)'i')
                            message.InitialClock = reader.GetString();
                        else if (propertySpan[0] == (byte)'c')
                            message.ConflateMs = reader.GetInt32();
                        break;

                    case 11:
                        message.HeartbeatMs = reader.GetInt32();
                        break;
                }
            }
        }

        return message;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<MarketChange>? ReadMarketChangesOptimized(ref FastJsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return null;

        var marketChanges = new List<MarketChange>(1);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var marketChange = ReadMarketChangeOptimized(ref reader);
                if (marketChange != null)
                    marketChanges.Add(marketChange);
            }
        }

        return marketChanges;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static MarketChange? ReadMarketChangeOptimized(ref FastJsonReader reader)
    {
        var marketChange = new MarketChange();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var propertySpan = reader.ValueSpan;
                if (!reader.Read()) break;

                // Optimized property matching for MarketChange
                var b0 = propertySpan[0];
                switch (propertySpan.Length)
                {
                    case 2:
                        if (b0 == (byte)'i')
                            marketChange.MarketId = reader.GetString();
                        else if (b0 == (byte)'r')
                            marketChange.RunnerChanges = ReadRunnerChangesOptimized(ref reader);
                        else if (b0 == (byte)'t')
                            marketChange.TotalAmountMatched = reader.GetNullableDouble();
                        break;

                    case 3:
                        // img property
                        if (b0 == (byte)'i')
                            marketChange.ReplaceCache = reader.GetBoolean();
                        else if (b0 == (byte)'c')
                            marketChange.Conflated = reader.GetBoolean();
                        break;

                    case 16:
                        // marketDefinition
                        marketChange.MarketDefinition = MarketDefinitionDeserializer.ReadMarketDefinition(ref reader);
                        break;
                }
            }
        }

        return marketChange;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<RunnerChange> ReadRunnerChangesOptimized(ref FastJsonReader reader)
    {
        var runnerChanges = new List<RunnerChange>(4);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var runnerChange = ReadRunnerChangeOptimized(ref reader);
                if (runnerChange != null)
                    runnerChanges.Add(runnerChange);
            }
        }

        return runnerChanges;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static RunnerChange ReadRunnerChangeOptimized(ref FastJsonReader reader)
    {
        var runnerChange = new RunnerChange();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.String)
            {
                var propertySpan = reader.ValueSpan;
                if (!reader.Read()) // Move to value
                    break;

                // Ultra-fast property matching for RunnerChange
                switch (propertySpan.Length)
                {
                    case 2:
                        var b0 = propertySpan[0];

                        if (b0 == (byte)'i')
                            runnerChange.SelectionId = reader.GetInt64();
                        else if (b0 == (byte)'t')
                            runnerChange.TotalMatched = reader.GetNullableDouble();
                        else if (b0 == (byte)'h')
                            runnerChange.Handicap = reader.GetNullableDouble();
                        break;

                    case 3:
                        var c0 = propertySpan[0];
                        var c2 = propertySpan[2];

                        if (c0 == (byte)'a' && c2 == (byte)'b') // "atb"
                            runnerChange.AvailableToBack = ReadDoubleArrayArray(ref reader);
                        else if (c0 == (byte)'a' && c2 == (byte)'l') // "atb"
                            runnerChange.AvailableToLay = ReadDoubleArrayArray(ref reader);
                        else if (c0 == (byte)'t') // "trd"
                            runnerChange.Traded = ReadDoubleArrayArray(ref reader);
                        else if (c0 == (byte)'l') // "ltp"
                            runnerChange.LastTradedPrice = reader.GetNullableDouble();
                        else if (c2 == (byte)'f') // "spf"
                            runnerChange.StartingPriceFar = reader.GetNullableDouble();
                        else if (c2 == (byte)'n') // "spn"
                            runnerChange.StartingPriceNear = reader.GetNullableDouble();
                        else if (c0 == (byte)'s' && c2 == (byte)'b') // "spb"
                            runnerChange.StartingPriceBack = ReadDoubleArrayArray(ref reader);
                        else if (c0 == (byte)'s' && c2 == (byte)'l') // "spl"
                            runnerChange.StartingPriceLay = ReadDoubleArrayArray(ref reader);
                        break;

                    case 4:
                        // batb, batl properties
                        if (propertySpan[3] == (byte)'b')
                            runnerChange.BestAvailableToBack = ReadDoubleArrayArray(ref reader);
                        else if (propertySpan[3] == (byte)'l')
                            runnerChange.BestAvailableToLay = ReadDoubleArrayArray(ref reader);
                        break;

                    case 5:
                        // bdatb, bdatl properties
                        if (propertySpan[4] == (byte)'b')
                            runnerChange.BestDisplayAvailableToBack = ReadDoubleArrayArray(ref reader);
                        else if (propertySpan[4] == (byte)'l')
                            runnerChange.BestDisplayAvailableToLay = ReadDoubleArrayArray(ref reader);
                        break;
                }
            }
        }

        return runnerChange;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<List<double>> ReadDoubleArrayArray(ref FastJsonReader reader)
    {
        var result = new List<List<double>>(3);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var innerArray = new List<double>(3);

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (reader.TokenType == JsonTokenType.Number)
                        innerArray.Add(reader.GetDouble());
                }

                result.Add(innerArray);
            }
        }

        return result;
    }
}
