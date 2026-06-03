using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Betfair.Stream;
using Betfair.Stream.MarketCache;
using Betfair.Stream.Responses;

namespace Betfair.Benchmarks;

[SimpleJob(RuntimeMoniker.Net10_0)]
[SimpleJob(RuntimeMoniker.NativeAot10_0)]
[MemoryDiagnoser]
public class MarketStreamPipelineBenchmarks
{
    private byte[] _fileBytes = [];
    private byte[][] _lines = [];

    [GlobalSetup]
    public void Setup()
    {
        var path = Path.Combine("Data", "MarketStream.txt");
        _fileBytes = File.ReadAllBytes(path);

        _lines = File.ReadAllLines(path)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(System.Text.Encoding.UTF8.GetBytes)
            .ToArray();
    }

    [Benchmark]
    public async Task<int> ReadThroughPipelineAndDeserialize()
    {
        using var memoryStream = new MemoryStream(_fileBytes);
        var pipeline = new Pipeline(memoryStream);
        var count = 0;

        await foreach (var line in pipeline.ReadLines(CancellationToken.None))
        {
            var changeMessage = JsonSerializer.Deserialize(line, SerializerContext.Default.ChangeMessage);
            count++;
        }

        return count;
    }

    [Benchmark]
    public async Task<int> ReadThroughPipelineOnly()
    {
        using var memoryStream = new MemoryStream(_fileBytes);
        var pipeline = new Pipeline(memoryStream);
        var count = 0;

        await foreach (var line in pipeline.ReadLines(CancellationToken.None))
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public async Task<int> ReadThroughPipelineAndDeserializeWithTimestamps()
    {
        using var memoryStream = new MemoryStream(_fileBytes);
        var pipeline = new Pipeline(memoryStream);
        var count = 0;

        await foreach (var line in pipeline.ReadLines(CancellationToken.None))
        {
            var received = DateTimeOffset.UtcNow.Ticks;
            var changeMessage = JsonSerializer.Deserialize(line, SerializerContext.Default.ChangeMessage)!;
            changeMessage.ReceivedTick = received;
            changeMessage.DeserializedTick = DateTimeOffset.UtcNow.Ticks;
            count++;
        }

        return count;
    }

    [Benchmark]
    public int Utf8JsonReaderDirect()
    {
        var count = 0;
        foreach (var line in _lines)
        {
            var summary = ChangeMessageReader.Read(line);
            count += summary.RunnerChangeCount;
        }

        return count;
    }

    [Benchmark]
    public async Task<int> PipelineWithUtf8JsonReader()
    {
        using var memoryStream = new MemoryStream(_fileBytes);
        var pipeline = new Pipeline(memoryStream);
        var count = 0;

        await foreach (var line in pipeline.ReadLines(CancellationToken.None))
        {
            var summary = ChangeMessageReader.Read(line);
            count += summary.RunnerChangeCount;
        }

        return count;
    }

    [Benchmark]
    public int MarketCacheFromBytes()
    {
        var processor = new MarketCacheProcessor();
        foreach (var line in _lines)
        {
            processor.Process(line);
        }

        return processor.Markets.Count;
    }

    [Benchmark]
    public async Task<int> PipelineWithMarketCache()
    {
        using var memoryStream = new MemoryStream(_fileBytes);
        var pipeline = new Pipeline(memoryStream);
        var processor = new MarketCacheProcessor();

        await foreach (var line in pipeline.ReadLines(CancellationToken.None))
        {
            processor.Process(line);
        }

        return processor.Markets.Count;
    }

    [Benchmark]
    public async Task<int> ZeroCopyPipelineWithMarketCache()
    {
        using var memoryStream = new MemoryStream(_fileBytes);
        var pipeline = new Pipeline(memoryStream);
        var processor = new MarketCacheProcessor();

        await pipeline.ProcessLines(processor.Process, CancellationToken.None);

        return processor.Markets.Count;
    }
}
