using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Betfair.Benchmarks.Deserializers;
using Betfair.Stream.Responses;
using Utf8Json.Resolvers;
using ChangeMessage = Betfair.Benchmarks.Responses.ChangeMessage;
using MarketChange = Betfair.Benchmarks.Responses.MarketChange;

namespace Betfair.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class InitialImageBenchmarks
{
    private byte[] _data;

    [GlobalSetup]
    public void Setup() =>
        _data = File.ReadAllBytes(@"./Data/InitialImage.json");

    [Benchmark(Baseline = true)]
    public ChangeMessage Utf8() =>
        Utf8Json.JsonSerializer.Deserialize<ChangeMessage>(_data, StandardResolver.CamelCase);

    //[Benchmark]
    //public ChangeMessage SystemTextJson() =>
    //    System.Text.Json.JsonSerializer.Deserialize<ChangeMessage>(_data);

    [Benchmark]
    public MarketChange CustomUtf8Reader() =>
        InitialImageDeserializer.Deserialize(_data);
}
