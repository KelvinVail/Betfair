using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Betfair.Benchmarks.Deserializers;
using Betfair.Stream.Responses;
using StatusMessage = Betfair.Benchmarks.Responses.StatusMessage;

namespace Betfair.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class StatusMessageBenchmarks
{
    private byte[] _data;

    [GlobalSetup]
    public void Setup() =>
        _data = File.ReadAllBytes(@"./Data/StatusMessage.json");

    [Benchmark(Baseline = true)]
    public StatusMessage Utf8() =>
        Utf8Json.JsonSerializer.Deserialize<StatusMessage>(_data);

    [Benchmark]
    public (int, bool) CustomUtf8Reader() =>
        StatusMessageDeserializer.Deserialize(_data);
}
