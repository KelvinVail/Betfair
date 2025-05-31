using System.Diagnostics;
using Betfair.Stream.Deserializers;
using Xunit;
using Xunit.Abstractions;

namespace Betfair.Tests.Stream.Deserializers;

/// <summary>
/// Performance tests to demonstrate the ultra-fast deserializer optimizations.
/// </summary>
public class DeserializerPerformanceTests
{
    private readonly ITestOutputHelper _output;
    private readonly byte[] _sampleMarketStreamData;

    public DeserializerPerformanceTests(ITestOutputHelper output)
    {
        _output = output;
        
        // Load sample data from MarketStream.txt
        var testDataPath = Path.Combine("Stream", "Data", "MarketStream.txt");
        var lines = File.ReadAllLines(testDataPath);
        
        // Use the first line with market changes for testing
        var sampleLine = lines.FirstOrDefault(l => l.Contains("\"mc\":[")) ?? lines[0];
        _sampleMarketStreamData = System.Text.Encoding.UTF8.GetBytes(sampleLine);
    }

    [Fact]
    public void UltraFastDeserializer_PerformanceComparison()
    {
        const int iterations = 10000;
        
        // Warm up
        var deserializer = new BetfairStreamDeserializer();
        for (int i = 0; i < 100; i++)
        {
            deserializer.DeserializeChangeMessage(_sampleMarketStreamData);
        }

        // Measure ultra-fast deserializer
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var result = deserializer.DeserializeChangeMessage(_sampleMarketStreamData);

            // Ensure the result is not null to prevent optimization
            Assert.NotNull(result);
        }
        stopwatch.Stop();

        var ultraFastTime = stopwatch.ElapsedMilliseconds;
        var ultraFastThroughput = iterations / (stopwatch.ElapsedMilliseconds / 1000.0);

        _output.WriteLine($"Ultra-Fast Deserializer Performance:");
        _output.WriteLine($"  Iterations: {iterations:N0}");
        _output.WriteLine($"  Total Time: {ultraFastTime:N0} ms");
        _output.WriteLine($"  Average Time: {(double)ultraFastTime / iterations:F4} ms per operation");
        _output.WriteLine($"  Throughput: {ultraFastThroughput:N0} operations/second");
        _output.WriteLine($"  Data Size: {_sampleMarketStreamData.Length:N0} bytes");
        _output.WriteLine($"  Throughput: {(ultraFastThroughput * _sampleMarketStreamData.Length) / (1024 * 1024):F2} MB/second");

        // Verify the result is correct
        var changeMessage = deserializer.DeserializeChangeMessage(_sampleMarketStreamData);
        
        Assert.NotNull(changeMessage);
        Assert.NotNull(changeMessage.Operation);
        Assert.True(changeMessage.Id > 0);
        
        _output.WriteLine($"\nDeserialization Result Validation:");
        _output.WriteLine($"  Operation: {changeMessage.Operation}");
        _output.WriteLine($"  ID: {changeMessage.Id}");
        _output.WriteLine($"  Clock: {changeMessage.Clock}");
        _output.WriteLine($"  Market Changes: {changeMessage.MarketChanges?.Count ?? 0}");
        
        if (changeMessage.MarketChanges?.Count > 0)
        {
            var firstMarket = changeMessage.MarketChanges[0];
            _output.WriteLine($"  First Market ID: {firstMarket.MarketId}");
            _output.WriteLine($"  Runner Changes: {firstMarket.RunnerChanges?.Count ?? 0}");
            
            if (firstMarket.RunnerChanges?.Count > 0)
            {
                var firstRunner = firstMarket.RunnerChanges[0];
                _output.WriteLine($"  First Runner ID: {firstRunner.SelectionId}");
                _output.WriteLine($"  Available to Back: {firstRunner.AvailableToBack?.Count ?? 0} price levels");
                _output.WriteLine($"  Available to Lay: {firstRunner.AvailableToLay?.Count ?? 0} price levels");
            }
        }

        // Performance assertion - should be very fast
        // Target: process at least 1000 messages per second
        Assert.True(ultraFastThroughput > 1000, 
            $"Expected throughput > 1000 ops/sec, but got {ultraFastThroughput:F0} ops/sec");
    }

    [Fact]
    public void UltraFastDeserializer_MemoryEfficiency()
    {
        const int iterations = 1000;
        
        // Measure memory before
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var memoryBefore = GC.GetTotalMemory(false);

        // Run deserializations
        var deserializer = new BetfairStreamDeserializer();
        for (int i = 0; i < iterations; i++)
        {
            var result = deserializer.DeserializeChangeMessage(_sampleMarketStreamData);
            Assert.NotNull(result);
        }

        // Measure memory after
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var memoryAfter = GC.GetTotalMemory(false);

        var memoryUsed = memoryAfter - memoryBefore;
        var memoryPerOperation = memoryUsed / (double)iterations;

        _output.WriteLine($"Memory Efficiency Test:");
        _output.WriteLine($"  Iterations: {iterations:N0}");
        _output.WriteLine($"  Memory Before: {memoryBefore:N0} bytes");
        _output.WriteLine($"  Memory After: {memoryAfter:N0} bytes");
        _output.WriteLine($"  Memory Used: {memoryUsed:N0} bytes");
        _output.WriteLine($"  Memory per Operation: {memoryPerOperation:F2} bytes");

        // Memory efficiency assertion - should use minimal memory per operation
        // Target: less than 10KB per operation (very generous, should be much less)
        Assert.True(memoryPerOperation < 10240, 
            $"Expected memory usage < 10KB per operation, but got {memoryPerOperation:F0} bytes");
    }

    [Fact]
    public void UltraFastDeserializer_AccuracyTest()
    {
        // Test with multiple lines from MarketStream.txt to ensure accuracy
        var testDataPath = Path.Combine("Stream", "Data", "MarketStream.txt");
        var lines = File.ReadAllLines(testDataPath);
        
        var deserializer = new BetfairStreamDeserializer();
        int successfulDeserializations = 0;
        int totalLines = 0;

        foreach (var line in lines.Take(100)) // Test first 100 lines
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            totalLines++;
            var lineBytes = System.Text.Encoding.UTF8.GetBytes(line);
            
            try
            {
                var result = deserializer.DeserializeChangeMessage(lineBytes);
                if (result != null)
                {
                    successfulDeserializations++;
                    
                    // Basic validation
                    Assert.NotNull(result.Operation);
                    Assert.True(result.Id > 0);
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Failed to deserialize line: {line.Substring(0, Math.Min(100, line.Length))}...");
                _output.WriteLine($"Error: {ex.Message}");
            }
        }

        var successRate = (double)successfulDeserializations / totalLines * 100;
        
        _output.WriteLine($"Accuracy Test Results:");
        _output.WriteLine($"  Total Lines Tested: {totalLines}");
        _output.WriteLine($"  Successful Deserializations: {successfulDeserializations}");
        _output.WriteLine($"  Success Rate: {successRate:F1}%");

        // Accuracy assertion - should successfully deserialize most valid JSON
        Assert.True(successRate > 90, 
            $"Expected success rate > 90%, but got {successRate:F1}%");
    }
}
