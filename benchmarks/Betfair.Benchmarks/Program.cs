using BenchmarkDotNet.Running;
using Betfair.Benchmarks.JsonBenchmarks;

// var runner = new BatbComparisons();
// var x = runner.Compiled();
// Console.WriteLine(x.Operation);
// Console.WriteLine(x.MarketChanges[0].MarketId);

BenchmarkRunner.Run<BatbComparisons>();
