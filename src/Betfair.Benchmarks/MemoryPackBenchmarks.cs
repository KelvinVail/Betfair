using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ChangeMessage = Betfair.Benchmarks.Responses.ChangeMessage;

namespace Betfair.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class MemoryPackBenchmarks
{
    private byte[] _pack;
    private byte[] _data;

    [GlobalSetup]
    public void Setup()
    {
        var data = File.ReadAllBytes("./Data/InitialImage.json");
        var obj = JsonSerializer.Deserialize<ChangeMessage>(data);
        _pack = MemoryPack.MemoryPackSerializer.Serialize(obj);

        var lines = File.ReadAllLines("./Data/InitialImage.json");
        var line = lines[1];
        _data = Encoding.ASCII.GetBytes(line);
    }

    [Benchmark]
    public ChangeMessage MemoryPackBenchmark()
    {
        return MemoryPack.MemoryPackSerializer.Deserialize<ChangeMessage>(_pack);
    }

    [Benchmark]
    public ChangeMessage MemoryPackFromFile()
    {
        return MemoryPack.MemoryPackSerializer.Deserialize<ChangeMessage>(_data);
    }
}
