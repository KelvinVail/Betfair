using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using Betfair.Stream.Deserializers;
using Betfair.Stream.Responses;
using Utf8Json.Resolvers;

namespace Betfair.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class JsonReadBenchmarks
{
    private readonly List<byte[]> _byteLines = [];
    private ReadOnlyMemory<byte>[] _memoryLines;

    [GlobalSetup]
    public void Setup()
    {
        var path = Path.Combine("Data", "MarketStream.txt");

        foreach (var line in File.ReadAllLines(path))
            _byteLines.Add(Encoding.UTF8.GetBytes(line));

        _memoryLines = _byteLines.Select(b => (ReadOnlyMemory<byte>)b).ToArray();
    }

    //[Benchmark]
    //public void ReadAllLinesWithUtf8JsonReader()
    //{
    //    foreach (var line in _byteLines)
    //    {
    //        var reader = new Utf8JsonReader(line);

    //        while (reader.Read())
    //        {
    //        }
    //    }
    //}

    //[Benchmark(Baseline = true)]
    //public void ReadAllBytes()
    //{
    //    long count = 0;
    //    foreach (var line in _byteLines)
    //    {
    //        foreach (var b in line.AsSpan())
    //        {
    //            count++;
    //        }
    //    }
    //}

    //[Benchmark]
    //public void ReadAllLinesWithFastJsonReader()
    //{
    //    foreach (var line in _byteLines)
    //    {
    //        var reader = new FastJsonReader(line);

    //        while (reader.Read())
    //        {
    //        }
    //    }

    //}

    //[Benchmark]
    //public void ReadAllLinesWithJsonDocument()
    //{
    //    foreach (var line in _byteLines)
    //    {
    //        using var doc = JsonDocument.Parse(line);

    //        foreach (var element in doc.RootElement.EnumerateObject())
    //        {
    //        }
    //    }
    //}

    //[Benchmark]
    //public void DeserializeAllLinesWithUtf8Json()
    //{
    //    foreach (var line in _byteLines)
    //    {
    //        var _ = Utf8Json.JsonSerializer.Deserialize<ChangeMessage>(line, StandardResolver.CamelCase);
    //    }
    //}

    //[Benchmark]
    //public void DeserializeAllLinesWithSystemTextJson()
    //{
    //    var context = SerializerContextExtensions.GetTypeInfo<ChangeMessage>();
    //    foreach (var line in _byteLines)
    //    {
    //        var _ = JsonSerializer.Deserialize(line, context);
    //    }
    //}

    [Benchmark]
    public void DeserializeAllLinesWithCustomDeserializer()
    {
        foreach (var line in _byteLines)
        {
            var _ = UltraFastChangeMessageDeserializer.Deserialize(line.AsSpan());
        }
    }

    [Benchmark]
    public void DeserializeAllLinesWithCustomDeserializerFromMem()
    {
        foreach (var line in _memoryLines)
        {
            var _ = UltraFastChangeMessageDeserializer.Deserialize(line.Span);
        }
    }

    //[Benchmark]
    //public void DeserializeAllLinesWithStructDeserializer()
    //{
    //    foreach (var line in _byteLines)
    //    {
    //        var _ = new ChangeMessageStruct(line);
    //    }
    //}

    // [Benchmark]
    // public void ReadAllLinesWithMarketCache()
    // {
    //     // 232us
    //     foreach (var line in _byteLines)
    //     {
    //         var reader = new Utf8JsonReader(line);
    //
    //         _market.ReadChangeMessage(ref reader);
    //     }
    // }
    //
    // [Benchmark]
    // public void ReadAllLinesWithMarketCacheWithoutComments()
    // {
    //     var options = new JsonReaderOptions
    //     {
    //         CommentHandling = JsonCommentHandling.Disallow,
    //     };
    //
    //     foreach (var line in _byteLines)
    //     {
    //         var reader = new Utf8JsonReader(line, options);
    //
    //         _market.ReadChangeMessage(ref reader);
    //     }
    // }
}
