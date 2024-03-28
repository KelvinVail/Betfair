using System.Buffers;
using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Betfair.Benchmarks.JsonBenchmarks.Custom;
using Betfair.Benchmarks.JsonBenchmarks.Responses;
using Utf8Json.Resolvers;

namespace Betfair.Benchmarks.JsonBenchmarks;

public class BatbComparisons
{
    private readonly string _json;
    private readonly byte[] _data;
    private readonly MemoryStream _ms = new ();
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public BatbComparisons()
    {
        _json = File.ReadAllText(@".\JsonBenchmarks\Data\batb.json");
        _data = Encoding.UTF8.GetBytes(_json);
        _ms.Write(_data);
    }

    // [Benchmark]
    // public ChangeMessage SystemTextJson() =>
    //     System.Text.Json.JsonSerializer.Deserialize<ChangeMessage>(_data, _options);

    [Benchmark]
    public ChangeMessage Utf8CamelCase()
    {
        _ms.Position = 0;
        return Utf8Json.JsonSerializer.Deserialize<ChangeMessage>(_ms, StandardResolver.CamelCase);
    }
    //
    // [Benchmark]
    // public ChangeMessage Compiled() =>
    //     System.Text.Json.JsonSerializer.Deserialize(_data, SerializerContext.Default.ChangeMessage);

    [Benchmark]
    public ChangeMessage CustomReader() =>
        Reader.Deserialize(_data);
}
