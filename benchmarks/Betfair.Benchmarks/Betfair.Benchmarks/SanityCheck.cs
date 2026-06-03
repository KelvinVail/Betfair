using System.Text;

namespace Betfair.Benchmarks;

/// <summary>
/// Quick validation that ChangeMessageReader produces correct results.
/// Run with: dotnet run -- sanity
/// </summary>
public static class SanityCheck
{
    public static void Run()
    {
        var path = Path.Combine("Data", "MarketStream.txt");
        var lines = File.ReadAllLines(path)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(Encoding.UTF8.GetBytes)
            .ToArray();

        Console.WriteLine($"Total lines: {lines.Length}");

        var totalRunners = 0;
        var totalPricePoints = 0;
        var totalMarkets = 0;

        foreach (var line in lines)
        {
            var summary = ChangeMessageReader.Read(line);
            totalRunners += summary.RunnerChangeCount;
            totalPricePoints += summary.PricePointCount;
            totalMarkets += summary.MarketChangeCount;
        }

        Console.WriteLine($"Total market changes: {totalMarkets}");
        Console.WriteLine($"Total runner changes: {totalRunners}");
        Console.WriteLine($"Total price points read: {totalPricePoints}");

        // Show first few messages
        for (int i = 0; i < Math.Min(5, lines.Length); i++)
        {
            var summary = ChangeMessageReader.Read(lines[i]);
            Console.WriteLine($"Line {i}: Op={summary.OperationByte:X2} Id={summary.Id} PT={summary.PublishTime} " +
                              $"Markets={summary.MarketChangeCount} Runners={summary.RunnerChangeCount} " +
                              $"PricePoints={summary.PricePointCount} TV={summary.TotalVolume}");
        }
    }
}
