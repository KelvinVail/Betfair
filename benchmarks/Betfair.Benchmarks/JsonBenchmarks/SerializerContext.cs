using Betfair.Benchmarks.JsonBenchmarks.Minimal;
using Betfair.Benchmarks.JsonBenchmarks.Responses;
using MarketChange = Betfair.Benchmarks.JsonBenchmarks.Responses.MarketChange;
using RunnerChange = Betfair.Benchmarks.JsonBenchmarks.Responses.RunnerChange;

namespace Betfair.Benchmarks.JsonBenchmarks;

[JsonSerializable(typeof(ChangeMessage))]
[JsonSerializable(typeof(MarketChange))]
[JsonSerializable(typeof(MarketDefinition))]
[JsonSerializable(typeof(OrderChange))]
[JsonSerializable(typeof(OrderRunnerChange))]
[JsonSerializable(typeof(RunnerChange))]
[JsonSerializable(typeof(RunnerDefinition))]
[JsonSerializable(typeof(UnmatchedOrder))]
[JsonSerializable(typeof(Change))]
[JsonSerializable(typeof(Betfair.Benchmarks.JsonBenchmarks.Minimal.MarketChangeMin))]
[JsonSerializable(typeof(Betfair.Benchmarks.JsonBenchmarks.Minimal.RunnerChangeMin))]
internal partial class SerializerContext : JsonSerializerContext
{
}