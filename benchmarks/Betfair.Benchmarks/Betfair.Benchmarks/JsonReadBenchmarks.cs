using System.Text;
using Betfair.Stream.Deserializers;
using Betfair.Stream.Responses;
using Utf8Json.Resolvers;

namespace Betfair.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class JsonReadBenchmarks
{
    private readonly List<byte[]> _byteLines = [];

    [GlobalSetup]
    public void Setup()
    {
        var path = Path.Combine("Data", "messages.txt");

        foreach (var line in File.ReadAllLines(path))
            _byteLines.Add(Encoding.UTF8.GetBytes(line));
    }

    // [Benchmark(Baseline = true)]
    // public void ReadAllLinesWithUtf8JsonReader()
    // {
    //     foreach (var line in _byteLines)
    //     {
    //         var reader = new Utf8JsonReader(line);
    //
    //         while (reader.Read())
    //         {
    //         }
    //     }
    // }
    //
    //[Benchmark(Baseline = true)]
    //public void ReadAllBytes()
    //{
    //    long count = 0;
    //    foreach (var line in _byteLines)
    //    {
    //        foreach (var b in line)
    //        {
    //            count++;
    //        }
    //    }
    //}
    //
    // [Benchmark]
    // public void ReadAllLinesWithJsonDocument()
    // {
    //     foreach (var line in _byteLines)
    //     {
    //         using var doc = JsonDocument.Parse(line);
    //
    //         foreach (var element in doc.RootElement.EnumerateObject())
    //         {
    //         }
    //     }
    // }
    //
    [Benchmark(Baseline = true)]
    public void ReadAllLinesWithUtf8Json()
    {
        foreach (var line in _byteLines)
        {
            var _ = Utf8Json.JsonSerializer.Deserialize<ChangeMessage>(line, StandardResolver.CamelCase);
        }
    }

    //[Benchmark]
    //public void DeserializeAllLinesWithSystemTextJson()
    //{
    //    foreach (var line in _byteLines)
    //    {
    //        var _ = JsonSerializer.Deserialize<ChangeMessage>(line);
    //    }
    //}

    [Benchmark]
    public void DeserializeAllLinesWithCustomDeserializer()
    {
        var deserializer = new BetfairStreamDeserializer();
        foreach (var line in _byteLines)
        {
            var _ = deserializer.DeserializeChangeMessage(line);
        }
    }

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

    // [Benchmark]
    // public void ReadAllLinesWithByteReader()
    // {
    //     foreach (var line in _byteLines)
    //     {
    //         var reader = new BetfairJsonReader(line);
    //
    //         while (reader.Read())
    //         {
    //         }
    //     }
    //
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
