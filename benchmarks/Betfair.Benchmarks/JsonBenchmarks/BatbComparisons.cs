using System.Buffers;
using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Betfair.Benchmarks.JsonBenchmarks.Custom;
using Betfair.Benchmarks.JsonBenchmarks.Minimal;
using Betfair.Benchmarks.JsonBenchmarks.Responses;
using Utf8Json.Resolvers;

namespace Betfair.Benchmarks.JsonBenchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
//[MemoryDiagnoser]
public class BatbComparisons
{
    private string _json;
    private byte[] _data;
    private MemoryStream _ms = new ();

    [GlobalSetup]
    public void Setup()
    {
        _json = File.ReadAllText(@".\JsonBenchmarks\Data\batb.json");
        _data = Encoding.UTF8.GetBytes(_json);
        _ms.Write(_data);
    }

    //[Benchmark]
    //public ChangeMessage SystemTextJson() =>
    //    System.Text.Json.JsonSerializer.Deserialize<ChangeMessage>(_data);

    [Benchmark]
    public ChangeMessage Utf8CamelCase()
    {
        _ms.Position = 0;
        return Utf8Json.JsonSerializer.Deserialize<ChangeMessage>(_ms, StandardResolver.CamelCase);
    }

    [Benchmark]
    public Change Utf8CamelCaseMin()
    {
        _ms.Position = 0;
        return Utf8Json.JsonSerializer.Deserialize<Change>(_ms, StandardResolver.CamelCase);
    }

    [Benchmark]
    public ChangeMessage Compiled() =>
        System.Text.Json.JsonSerializer.Deserialize(_data, SerializerContext.Default.ChangeMessage);

    [Benchmark]
    public Change CompiledMin() =>
        System.Text.Json.JsonSerializer.Deserialize(_data, SerializerContext.Default.Change);

    [Benchmark]
    public ChangeMessage CustomReader() =>
        Reader.Deserialize(_data);
}
