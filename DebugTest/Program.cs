using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Betfair.Stream.Deserializers;
using Betfair.Stream.Responses;

// Debug test to find which line is failing
var path = Path.Combine("src", "Betfair.Tests", "Stream", "Data", "MarketStream.txt");
var lines = File.ReadAllLines(path);

var deserializer = new BetfairStreamDeserializer();

for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
    Console.WriteLine($"Testing line {i + 1}:");

    try
    {
        var bytes = Encoding.UTF8.GetBytes(line);
        var changeMessage = deserializer.DeserializeChangeMessage(bytes);
        var systemTextJsonMessage = JsonSerializer.Deserialize<ChangeMessage>(line);

        // Check if they have different MarketChanges
        if ((changeMessage?.MarketChanges == null) != (systemTextJsonMessage?.MarketChanges == null))
        {
            Console.WriteLine($"  MISMATCH MarketChanges: Our={changeMessage?.MarketChanges?.Count}, Sys={systemTextJsonMessage?.MarketChanges?.Count}");
            Console.WriteLine($"  Line: {line.Substring(0, Math.Min(100, line.Length))}...");
            return;
        }

        // Check if they have different RunnerChanges or BestAvailableToBack values
        if (changeMessage?.MarketChanges?.Count > 0 && systemTextJsonMessage?.MarketChanges?.Count > 0)
        {
            var ourMc = changeMessage.MarketChanges[0];
            var sysMc = systemTextJsonMessage.MarketChanges[0];

            // Check if RunnerChanges nullability differs
            if ((ourMc.RunnerChanges == null) != (sysMc.RunnerChanges == null))
            {
                Console.WriteLine($"  MISMATCH RunnerChanges: Our={ourMc.RunnerChanges?.Count}, Sys={sysMc.RunnerChanges?.Count}");
                Console.WriteLine($"  Line: {line.Substring(0, Math.Min(100, line.Length))}...");
                return;
            }

            if (ourMc.RunnerChanges?.Count > 0 && sysMc.RunnerChanges?.Count > 0)
            {
                for (int j = 0; j < Math.Min(ourMc.RunnerChanges.Count, sysMc.RunnerChanges.Count); j++)
                {
                    var ourRc = ourMc.RunnerChanges[j];
                    var sysRc = sysMc.RunnerChanges[j];

                    if ((ourRc.BestAvailableToBack == null) != (sysRc.BestAvailableToBack == null))
                    {
                        Console.WriteLine($"  MISMATCH at runner {j}: Our={ourRc.BestAvailableToBack?.Count}, Sys={sysRc.BestAvailableToBack?.Count}");
                        Console.WriteLine($"  Line: {line.Substring(0, Math.Min(100, line.Length))}...");
                        return;
                    }
                }
            }
        }

        Console.WriteLine($"  Line {i + 1} OK");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  ERROR on line {i + 1}: {ex.Message}");
        Console.WriteLine($"  Line: {line.Substring(0, Math.Min(100, line.Length))}...");
        return;
    }
}
