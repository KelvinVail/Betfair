using BenchmarkDotNet.Running;
using Betfair.Benchmarks;

if (args.Length > 0 && args[0] == "sanity")
{
    SanityCheck.Run();
    return;
}

if (args.Length > 0 && args[0] == "quick")
{
    QuickTimer.Run();
    return;
}

BenchmarkRunner.Run<MarketStreamPipelineBenchmarks>();
BenchmarkRunner.Run<OrderStreamPipelineBenchmarks>();
