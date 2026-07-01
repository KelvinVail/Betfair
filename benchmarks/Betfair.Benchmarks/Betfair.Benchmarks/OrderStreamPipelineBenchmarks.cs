using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Betfair.Stream;
using Betfair.Stream.OrderCache;
using Betfair.Stream.Responses;

namespace Betfair.Benchmarks;

[SimpleJob(RuntimeMoniker.Net10_0)]
[MemoryDiagnoser]
public class OrderStreamPipelineBenchmarks
{
    private byte[] _fileBytes = [];
    private byte[][] _allLines = [];

    // First line is the full image; remaining lines are deltas
    private byte[] _imageLine = [];
    private byte[][] _deltaLines = [];
    private byte[] _deltasFileBytes = [];

    // Pre-warmed processor with image already applied
    private OrderCacheProcessor _warmedProcessor = new ();

    [GlobalSetup]
    public void Setup()
    {
        var path = Path.Combine("Data", "OrderStream.txt");
        _fileBytes = File.ReadAllBytes(path);

        _allLines = File.ReadAllLines(path)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(System.Text.Encoding.UTF8.GetBytes)
            .ToArray();

        _imageLine = _allLines[0];
        _deltaLines = _allLines.Skip(1).ToArray();

        // Build file bytes containing only delta lines for pipeline benchmarks
        _deltasFileBytes = System.Text.Encoding.UTF8.GetBytes(
            string.Join("\n", File.ReadAllLines(path).Skip(1).Where(l => !string.IsNullOrWhiteSpace(l))));

        // Pre-warm the processor with the initial image
        _warmedProcessor.Process(_imageLine);
    }

    [Benchmark(Baseline = true)]
    public int DeserializeWithSystemTextJson()
    {
        var count = 0;
        foreach (var line in _allLines)
        {
            var msg = JsonSerializer.Deserialize(line, SerializerContext.Default.ChangeMessage);
            if (msg?.OrderChanges != null)
                count += msg.OrderChanges.Count;
        }

        return count;
    }

    [Benchmark]
    public int OrderCacheFromBytes_FullStream()
    {
        var processor = new OrderCacheProcessor();
        foreach (var line in _allLines)
        {
            processor.Process(line);
        }

        return processor.Markets.Count;
    }

    [Benchmark]
    public int OrderCacheFromBytes_DeltasOnly()
    {
        // Process only deltas against a pre-warmed cache
        // This measures steady-state performance (no initial image allocation)
        foreach (var line in _deltaLines)
        {
            _warmedProcessor.Process(line);
        }

        return _warmedProcessor.Markets.Count;
    }

    [Benchmark]
    public async Task<int> ZeroCopyPipeline_DeltasOnly()
    {
        using var memoryStream = new MemoryStream(_deltasFileBytes);
        var pipeline = new Pipeline(memoryStream);

        await pipeline.ProcessLines(_warmedProcessor.Process, CancellationToken.None);

        return _warmedProcessor.Markets.Count;
    }

    [Benchmark]
    public async Task<int> ZeroCopyPipeline_FullStream()
    {
        using var memoryStream = new MemoryStream(_fileBytes);
        var pipeline = new Pipeline(memoryStream);
        var processor = new OrderCacheProcessor();

        await pipeline.ProcessLines(processor.Process, CancellationToken.None);

        return processor.Markets.Count;
    }
}
