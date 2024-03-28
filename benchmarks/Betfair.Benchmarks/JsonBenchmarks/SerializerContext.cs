using Betfair.Benchmarks.JsonBenchmarks.Responses;

namespace Betfair.Benchmarks.JsonBenchmarks;

[JsonSerializable(typeof(ChangeMessage))]
[JsonSerializable(typeof(MarketChange))]
[JsonSerializable(typeof(MarketDefinition))]
[JsonSerializable(typeof(OrderChange))]
[JsonSerializable(typeof(OrderRunnerChange))]
[JsonSerializable(typeof(RunnerChange))]
[JsonSerializable(typeof(RunnerDefinition))]
[JsonSerializable(typeof(UnmatchedOrder))]
internal partial class SerializerContext : JsonSerializerContext
{
}