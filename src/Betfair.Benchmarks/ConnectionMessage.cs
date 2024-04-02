using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Betfair.Benchmarks.Deserializers;
using Betfair.Stream.Responses;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Betfair.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class ConnectionMessageBenchmarks
{
    private byte[] _data;

    [GlobalSetup]
    public void Setup() =>
        _data = File.ReadAllBytes(@"./Data/ConnectionMessage.json");

    [Benchmark(Baseline = true)]
    public Stream.Responses.ConnectionMessage Utf8() =>
        Utf8Json.JsonSerializer.Deserialize<Stream.Responses.ConnectionMessage>(_data);

    [Benchmark]
    public ReadOnlySpan<byte> CustomUtf8JsonReader() =>
        ConnectionMessageDeserializer.ConnectionId(_data);
}
