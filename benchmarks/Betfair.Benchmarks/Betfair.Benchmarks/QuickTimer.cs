using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Betfair.Stream;
using Betfair.Stream.MarketCache;
using Betfair.Stream.Responses;

namespace Betfair.Benchmarks;

/// <summary>
/// Quick timing comparison without BenchmarkDotNet overhead.
/// Run with: dotnet run -c Release -- quick
/// </summary>
public static class QuickTimer
{
    private const int Warmup = 5;
    private const int Iterations = 20;

    public static void Run()
    {
        var path = Path.Combine("Data", "MarketStream.txt");
        var fileBytes = File.ReadAllBytes(path);
        var lines = File.ReadAllLines(path)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(Encoding.UTF8.GetBytes)
            .ToArray();

        Console.WriteLine($"Lines: {lines.Length}");
        Console.WriteLine($"Total bytes: {fileBytes.Length:N0}");
        Console.WriteLine();

        // Warmup & measure: System.Text.Json deserialization
        RunBenchmark("System.Text.Json (typed objects)", () =>
        {
            foreach (var line in lines)
            {
                _ = JsonSerializer.Deserialize(line, SerializerContext.Default.ChangeMessage);
            }
        });

        // Warmup & measure: Utf8JsonReader direct (no cache)
        RunBenchmark("Utf8JsonReader (zero-alloc, no cache)", () =>
        {
            foreach (var line in lines)
            {
                _ = ChangeMessageReader.Read(line);
            }
        });

        // MarketCacheProcessor — full pipeline with usable cache
        RunBenchmark("MarketCacheProcessor (full cache)", () =>
        {
            var processor = new MarketCacheProcessor();
            foreach (var line in lines)
            {
                processor.Process(line);
            }
        });

        // Pipeline + MarketCacheProcessor
        RunBenchmark("Pipeline + MarketCacheProcessor", () =>
        {
            using var ms = new MemoryStream(fileBytes);
            var pipeline = new Pipeline(ms);
            var processor = new MarketCacheProcessor();
            var enumerator = pipeline.ReadLines(CancellationToken.None).GetAsyncEnumerator();
            while (enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult())
            {
                processor.Process(enumerator.Current);
            }
        });

        // Zero-copy Pipeline + MarketCacheProcessor
        RunBenchmark("ZeroCopy Pipeline + MarketCacheProcessor", () =>
        {
            using var ms = new MemoryStream(fileBytes);
            var pipeline = new Pipeline(ms);
            var processor = new MarketCacheProcessor();
            pipeline.ProcessLines(processor.Process, CancellationToken.None).GetAwaiter().GetResult();
        });

        Console.WriteLine();

        // Show cache contents for validation
        Console.WriteLine("--- Cache Validation ---");
        var validationProcessor = new MarketCacheProcessor();
        foreach (var line in lines)
            validationProcessor.Process(line);

        foreach (var (id, market) in validationProcessor.Markets)
        {
            Console.WriteLine($"Market: {id} | Runners: {market.RunnerCount} | TV: {market.TotalMatched}");
            foreach (var (selId, runner) in market.Runners)
            {
                Console.WriteLine($"  {selId}: LTP={runner.LastTradedPrice} TV={runner.TotalMatched} " +
                                  $"ATB={runner.AvailableToBack.Count} ATL={runner.AvailableToLay.Count} " +
                                  $"Traded={runner.Traded.Count}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Python comparison (betfairlightweight orjson + MarketBookCache): ~1593ms");
    }

    private static void RunBenchmark(string name, Action action)
    {
        // Warmup
        for (int i = 0; i < Warmup; i++)
            action();

        // Force GC before measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var times = new double[Iterations];
        var sw = new Stopwatch();

        var allocBefore = GC.GetTotalAllocatedBytes(precise: true);
        for (int i = 0; i < Iterations; i++)
        {
            sw.Restart();
            action();
            sw.Stop();
            times[i] = sw.Elapsed.TotalMilliseconds;
        }
        var allocAfter = GC.GetTotalAllocatedBytes(precise: true);

        var allocPerIteration = (allocAfter - allocBefore) / Iterations;
        var mean = times.Average();
        var median = times.OrderBy(t => t).ElementAt(Iterations / 2);
        var min = times.Min();

        Console.WriteLine($"{name,-45} | Mean: {mean,8:F3}ms | Median: {median,8:F3}ms | Min: {min,8:F3}ms | Alloc: {allocPerIteration / 1024.0 / 1024.0,8:F2} MB");
    }
}
