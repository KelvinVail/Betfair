using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;

namespace Betfair.Extensions.JsonReaders;

internal static class RunnerDefinitionJsonReader
{
    private static readonly Dictionary<byte, RunnerStatus> _runnerStatusMap = new ()
    {
        [(byte)'A'] = RunnerStatus.Active,
        [(byte)'W'] = RunnerStatus.Winner,
        [(byte)'L'] = RunnerStatus.Loser,
        [(byte)'R'] = RunnerStatus.Removed,
        [(byte)'H'] = RunnerStatus.Hidden,
    };

    internal static void ReadRunners(this Market market, ref Utf8JsonReader reader)
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
            market.InternalRunners.Add(id, Runner.Create(id, runnerStatus, adjustmentFactor));
        
        // TODO: Handle processing Runners more than once.

        // TODO: Add an internal AddRunner method to Market.
    }

    private static long FindId(ref Utf8JsonReader reader)
    {
        var copy = reader;

        while (copy.Read())
        {
            if (copy.TokenType == JsonTokenType.EndObject) break;
            if (!copy.PropertyName("id"u8)) continue;

            copy.Read();
            return copy.GetInt64();
        }

        return 0;
    }

    private static double ReadAdjustmentFactor(ref Utf8JsonReader reader, double adjustmentFactor)
    {
        if (!reader.PropertyName("adjustmentFactor"u8))
            return adjustmentFactor;

        reader.Read();
        return reader.GetDouble();
    }

    private static RunnerStatus ReadRunnerStatus(ref Utf8JsonReader reader, RunnerStatus runnerStatus)
    {
        // TODO: Test all runner statuses.

        if (!reader.PropertyName("status"u8))
            return runnerStatus;

        reader.Read();
        return _runnerStatusMap[reader.ValueSpan[0]];
    }
}
