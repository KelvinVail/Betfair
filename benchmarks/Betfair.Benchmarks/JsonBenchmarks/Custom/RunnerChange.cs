using System.Buffers;
using System.Text.Json;

namespace Betfair.Benchmarks.JsonBenchmarks.Custom;

public class RunnerChange
{
    public static IEnumerable<RunnerChange> GetRunnerChanges(ReadOnlySequence<byte> data)
    {
        foreach (var runner in GetRunners(data))
            yield return runner;
    }

    private static IEnumerable<RunnerChange> GetRunners(ReadOnlySequence<byte> data)
    {
        var reader = new Utf8JsonReader(data);
        var runnerChanges = new List<RunnerChange>();
        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName) continue;
            if (!reader.ValueTextEquals("rc")) continue;
            while (reader.Read())
            {
                var runner = GetRunnerChange(ref reader);
                if (runner == null) break;
                runnerChanges.Add(runner);
            }
        }

        return runnerChanges;
    }

    private static RunnerChange? GetRunnerChange(ref Utf8JsonReader reader)
    {
        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName) continue;
            if (!reader.ValueTextEquals("id")) continue;
            reader.Read();
            // if (reader.TokenType == JsonTokenType.String) continue;
            var selectionId = reader.GetInt64();
            return new RunnerChange ();
        }

        return null;
    }
}
