using System.Text;
using Betfair.Stream.Deserializers;
using Betfair.Stream.Responses;

namespace Betfair.Tests.Stream.Deserializers;

public sealed class BetfairStreamDeserializerTests
{
    private readonly BetfairStreamDeserializer _deserializer = new ();

    [Fact]
    public void ShouldBeEquivalentToSystemTextJson()
    {
        var path = Path.Combine("Stream", "Data", "MarketStream.txt");
        var json = File.ReadAllLines(path);

        foreach (var line in json)
        {
            var bytes = Encoding.UTF8.GetBytes(line);

            var changeMessage = _deserializer.DeserializeChangeMessage(bytes);
            var systemTextJsonMessage = JsonSerializer.Deserialize<ChangeMessage>(line);

            changeMessage.Should().BeEquivalentTo(systemTextJsonMessage, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
        }
    }
}
