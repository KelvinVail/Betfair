using System.Buffers.Text;
using System.Text;
using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;

namespace Betfair.Extensions.JsonReaders;

internal static class ChangeMessageJsonReader
{
    private static readonly Dictionary<byte, MarketStatus> _marketStatusMap = new()
    {
        [(byte)'O'] = MarketStatus.Open,
        [(byte)'S'] = MarketStatus.Suspended,
        [(byte)'C'] = MarketStatus.Closed,
        [(byte)'I'] = MarketStatus.Inactive,
    };

    private static readonly Dictionary<byte, RunnerStatus> _runnerStatusMap = new()
    {
        [(byte)'A'] = RunnerStatus.Active,
        [(byte)'W'] = RunnerStatus.Winner,
        [(byte)'L'] = RunnerStatus.Loser,
        [(byte)'R'] = RunnerStatus.Removed,
        [(byte)'H'] = RunnerStatus.Hidden,
    };

    internal static void ReadChangeMessage(this Market market, ref Utf8JsonReader reader)
    {
        while (reader.Read())
        {
            if (!reader.PropertyName("op"u8))
                continue;

            reader.Read();
            if (reader.Value("mcm"u8))
                market.ReadMarketChangeMessage(ref reader);
            else
                return;
        }
    }

    private static void ReadMarketChangeMessage(this Market market, ref Utf8JsonReader reader)
    {
        var objectCount = 0;
        long pt = 0;
        while (reader.Read())
        {
            if (reader.PropertyName("pt"u8))
            {
                reader.Read();
                pt = reader.GetInt64();
            }

            if (reader.EndOfObject(ref objectCount)) break;

            market.ReadMarketChange(ref reader, pt);
        }
    }

    private static void ReadMarketChange(this Market market, ref Utf8JsonReader reader, long publishTime)
    {
        if (!reader.PropertyName("mc"u8)) return;

        var objectCount = 0;
        while (reader.Read())
        {
            if (reader.EndOfObject(ref objectCount)) break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                switch (reader.ValueSpan)
                {
                    case var span when span.SequenceEqual("id"u8):
                        reader.Read();
                        if (!reader.ValueSpan.SequenceEqual(Encoding.UTF8.GetBytes(market.Id)))
                            return;
                        break;
                    case var span when span.SequenceEqual("tv"u8):
                        reader.Read();
                        market.TradedVolume = reader.GetDouble();
                        break;
                    case var span when span.SequenceEqual("marketDefinition"u8):
                        market.ReadMarketDefinition(ref reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        market.PublishTime = publishTime;
    }

    private static void ReadMarketDefinition(this Market market, ref Utf8JsonReader reader)
    {
        if (!reader.PropertyName("marketDefinition"u8)) return;

        var objectCount = 0;
        while (reader.Read())
        {
            if (reader.EndOfObject(ref objectCount)) break;
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                switch (reader.ValueSpan)
                {
                    case var span when span.SequenceEqual("marketTime"u8):
                        reader.Read();
                        market.StartTime = reader.GetDateTimeOffset();
                        break;
                    case var span when span.SequenceEqual("inPlay"u8):
                        reader.Read();
                        market.IsInPlay = reader.GetBoolean();
                        break;
                    case var span when span.SequenceEqual("status"u8):
                        reader.Read();
                        market.Status = _marketStatusMap[reader.ValueSpan[0]];
                        break;
                    case var span when span.SequenceEqual("version"u8):
                        reader.Read();
                        market.Version = reader.GetInt64();
                        break;
                    case var span when span.SequenceEqual("runners"u8):
                        market.ReadRunners(ref reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }
    }

    private static void ReadRunners(this Market market, ref Utf8JsonReader reader)
    {
        if (!reader.PropertyName("runners"u8)) return;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;

            market.ReadRunner(ref reader);
        }
    }

    private static void ReadRunner(this Market market, ref Utf8JsonReader reader)
    {
        var id = FindId(ref reader);
        double adjustmentFactor = 0;
        RunnerStatus runnerStatus = RunnerStatus.Hidden;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) break;

            adjustmentFactor = ReadAdjustmentFactor(ref reader, adjustmentFactor);
            runnerStatus = ReadRunnerStatus(ref reader, runnerStatus);
        }

        if (runnerStatus == RunnerStatus.Active)
            market.InternalRunners.TryAdd(id, Runner.Create(id, runnerStatus, adjustmentFactor));

        // TODO: Handle processing Runners more than once.

        // TODO: Add an internal AddRunner method to Market.
    }

    private static long FindId(ref Utf8JsonReader reader)
    {
        var copy = reader;

        while (copy.Read())
        {
            if (copy.TokenType == JsonTokenType.EndObject) break;
            if (copy.TokenType == JsonTokenType.PropertyName && copy.ValueSpan.SequenceEqual("id"u8))
            {
                copy.Read();
                return copy.GetInt64();
            }
        }

        return 0;
    }

    private static double ReadAdjustmentFactor(ref Utf8JsonReader reader, double adjustmentFactor)
    {
        if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("adjustmentFactor"u8))
        {
            reader.Read();
            return reader.GetDouble();
        }

        return adjustmentFactor;
    }

    private static RunnerStatus ReadRunnerStatus(ref Utf8JsonReader reader, RunnerStatus runnerStatus)
    {
        // TODO: Test all runner statuses.

        if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("status"u8))
        {
            reader.Read();
            return _runnerStatusMap[reader.ValueSpan[0]];
        }

        return runnerStatus;
    }
}
