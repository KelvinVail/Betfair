using System.Text;
using System.Text.Json;
using Betfair.Benchmarks.Mocks;
using Betfair.Core.Login;
using Betfair.Extensions.JsonReaders;
using Betfair.Extensions.Markets;
using Betfair.Stream.Responses;
using Utf8Json.Resolvers;

namespace Betfair.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class JsonReadBenchmarks
{
    private readonly List<byte[]> _byteLines = [];
    private readonly Credentials _credentials = new ("username", "password", "appKey");
    private readonly SubscriptionStub _sub = new (Path.Combine("Data", "messages.txt"));
    private Market _market;

    [GlobalSetup]
    public void Setup()
    {
        var path = Path.Combine("Data", "messages.txt");

        _market = Market.Create(_credentials, "1.235123059", _sub).Value;

        foreach (var line in File.ReadAllLines(path))
            _byteLines.Add(Encoding.UTF8.GetBytes(line));
    }

    [Benchmark(Baseline = true)]
    public void ReadAllLinesWithUtf8JsonReader()
    {
        foreach (var line in _byteLines)
        {
            var reader = new Utf8JsonReader(line);
    
            while (reader.Read())
            {
            }
        }
    }
    
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
    // [Benchmark]
    // public void ReadAllLinesWithUtf8Json()
    // {
    //     foreach (var line in _byteLines)
    //     {
    //         var _ = Utf8Json.JsonSerializer.Deserialize<ChangeMessage>(line, StandardResolver.CamelCase);
    //     }
    // }
    //
    // [Benchmark]
    // public void DeserializeAllLinesWithSystemTextJson()
    // {
    //     foreach (var line in _byteLines)
    //     {
    //         var _ = JsonSerializer.Deserialize<ChangeMessage>(line);
    //     }
    // }

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

    [Benchmark]
    public void ReadAllLinesWithMarketCacheWithoutComments()
    {
        var options = new JsonReaderOptions
        {
            CommentHandling = JsonCommentHandling.Disallow,
        };

        foreach (var line in _byteLines)
        {
            var reader = new Utf8JsonReader(line, options);
    
            _market.ReadChangeMessage(ref reader);
        }
    }
}
