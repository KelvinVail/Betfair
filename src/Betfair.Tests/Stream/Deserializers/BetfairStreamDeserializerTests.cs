using System.Text;
using Betfair.Stream.Deserializers;
using Betfair.Stream.Responses;

namespace Betfair.Tests.Stream.Deserializers;

public sealed class BetfairStreamDeserializerTests
{
    [Fact(Skip = "WIP")]
    public void ShouldBeEquivalentToSystemTextJson()
    {
        var path = Path.Combine("Stream", "Data", "MarketStream.txt");
        var json = File.ReadAllLines(path);

        foreach (var line in json)
        {
            var bytes = Encoding.UTF8.GetBytes(line);

            var changeMessage = UltraFastChangeMessageDeserializer.Deserialize(bytes);
            var systemTextJsonMessage = JsonSerializer.Deserialize<ChangeMessage>(bytes);

            changeMessage.Should().BeEquivalentTo(systemTextJsonMessage, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
        }
    }
}
