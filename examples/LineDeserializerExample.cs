using System.Text;
using Betfair.Stream.Deserializers;

namespace Examples;

/// <summary>
/// Example demonstrating how to use the new LineDeserializer to transform
/// single lines of JSON bytes into ChangeMessage objects.
/// </summary>
public class LineDeserializerExample
{
    public static void Main()
    {
        // Create a new LineDeserializer instance
        var deserializer = new LineDeserializer();

        // Example 1: Deserialize from byte array
        Console.WriteLine("=== Example 1: Deserialize from byte array ===");
        var json1 = """{"op":"mcm","id":2,"clk":"AAAAAAAA","pt":1743511672980}""";
        var bytes1 = Encoding.UTF8.GetBytes(json1);
        
        var changeMessage1 = deserializer.DeserializeChangeMessage(bytes1);
        if (changeMessage1 != null)
        {
            Console.WriteLine($"Operation: {changeMessage1.Operation}");
            Console.WriteLine($"ID: {changeMessage1.Id}");
            Console.WriteLine($"Clock: {changeMessage1.Clock}");
            Console.WriteLine($"Publish Time: {changeMessage1.PublishTime}");
        }

        Console.WriteLine();

        // Example 2: Deserialize from ReadOnlySpan<byte>
        Console.WriteLine("=== Example 2: Deserialize from ReadOnlySpan<byte> ===");
        var json2 = """{"op":"mcm","id":3,"clk":"BBBBBBBB","pt":1743511672981,"mc":[{"id":"1.123456","tv":1000.50}]}""";
        var bytes2 = Encoding.UTF8.GetBytes(json2);
        
        var changeMessage2 = deserializer.DeserializeChangeMessage(bytes2.AsSpan());
        if (changeMessage2 != null)
        {
            Console.WriteLine($"Operation: {changeMessage2.Operation}");
            Console.WriteLine($"ID: {changeMessage2.Id}");
            Console.WriteLine($"Clock: {changeMessage2.Clock}");
            Console.WriteLine($"Publish Time: {changeMessage2.PublishTime}");
            Console.WriteLine($"Market Changes Count: {changeMessage2.MarketChanges?.Count ?? 0}");
            
            if (changeMessage2.MarketChanges?.Count > 0)
            {
                var marketChange = changeMessage2.MarketChanges[0];
                Console.WriteLine($"  Market ID: {marketChange.MarketId}");
                Console.WriteLine($"  Total Volume: {marketChange.TotalAmountMatched}");
            }
        }

        Console.WriteLine();

        // Example 3: Handle invalid JSON gracefully
        Console.WriteLine("=== Example 3: Handle invalid JSON ===");
        var invalidJson = """{"op":"mcm","id":4,"clk":"CCCCCCCC","pt":"""; // Incomplete JSON
        var invalidBytes = Encoding.UTF8.GetBytes(invalidJson);
        
        var changeMessage3 = deserializer.DeserializeChangeMessage(invalidBytes);
        if (changeMessage3 == null)
        {
            Console.WriteLine("Invalid JSON was handled gracefully - returned null");
        }

        Console.WriteLine();

        // Example 4: Process multiple lines
        Console.WriteLine("=== Example 4: Process multiple lines ===");
        var jsonLines = new[]
        {
            """{"op":"mcm","id":5,"clk":"DDDDDDDD","pt":1743511672982}""",
            """{"op":"mcm","id":6,"clk":"EEEEEEEE","pt":1743511672983}""",
            """{"op":"mcm","id":7,"clk":"FFFFFFFF","pt":1743511672984}"""
        };

        foreach (var jsonLine in jsonLines)
        {
            var lineBytes = Encoding.UTF8.GetBytes(jsonLine);
            var changeMessage = deserializer.DeserializeChangeMessage(lineBytes);
            
            if (changeMessage != null)
            {
                Console.WriteLine($"Processed message ID: {changeMessage.Id}, Clock: {changeMessage.Clock}");
            }
        }
    }
}
