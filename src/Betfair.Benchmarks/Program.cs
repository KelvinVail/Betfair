using BenchmarkDotNet.Running;
using Betfair.Benchmarks;

// BenchmarkRunner.Run<ConnectionMessageBenchmarks>();
// BenchmarkRunner.Run<StatusMessageBenchmarks>();
// BenchmarkRunner.Run<InitialImageBenchmarks>();
BenchmarkRunner.Run<MemoryPackBenchmarks>();

