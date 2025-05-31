using System.Text;
using Betfair.Stream.Deserializers;
using Betfair.Stream.Responses;

namespace Betfair.Tests.Stream.Deserializers;

public sealed class BetfairStreamDeserializerTests
{
    private readonly BetfairStreamDeserializer _deserializer = new ();

    [Fact]
    public void CanDeserializeSimpleChangeMessage()
    {
        var json = """{"op":"mcm","id":2,"clk":"AAAAAAAA","pt":1743511672980}""" + "\n";
        var bytes = Encoding.UTF8.GetBytes(json);

        var message = _deserializer.DeserializeChangeMessage(bytes);

        message.Should().NotBeNull();
        message.Operation.Should().Be("mcm");
        message.Id.Should().Be(2);
        message.Clock.Should().Be("AAAAAAAA");
        message.PublishTime.Should().Be(1743511672980);
    }

    [Fact]
    public void CanDeserializeChangeMessageWithMarketChanges()
    {
        var json = """{"op":"mcm","id":2,"pt":1743511672980,"mc":[{"id":"1.241629436","tv":67505.47}]}""" + "\n";
        var bytes = Encoding.UTF8.GetBytes(json);

        var message = _deserializer.DeserializeChangeMessage(bytes);

        message.Should().NotBeNull();
        message.Operation.Should().Be("mcm");
        message.Id.Should().Be(2);
        message.PublishTime.Should().Be(1743511672980);
        message.MarketChanges.Should().NotBeNull();
        message.MarketChanges.Should().HaveCount(1);

        var marketChange = message.MarketChanges![0];
        marketChange.MarketId.Should().Be("1.241629436");
        marketChange.TotalAmountMatched.Should().Be(67505.47);
    }

    [Fact]
    public void CanDeserializeChangeMessageWithRunnerChanges()
    {
        var json = """{"op":"mcm","id":2,"pt":1743511672980,"mc":[{"id":"1.241629436","rc":[{"id":270592,"tv":5569.21,"ltp":11.5}]}]}""" + "\n";
        var bytes = Encoding.UTF8.GetBytes(json);

        var changeMessage = _deserializer.DeserializeChangeMessage(bytes);

        changeMessage.Should().NotBeNull();
        changeMessage!.Operation.Should().Be("mcm");
        changeMessage.Id.Should().Be(2);
        changeMessage.PublishTime.Should().Be(1743511672980);
        changeMessage.MarketChanges.Should().NotBeNull();
        changeMessage.MarketChanges.Should().HaveCount(1);

        var marketChange = changeMessage.MarketChanges![0];
        marketChange.MarketId.Should().Be("1.241629436");
        marketChange.RunnerChanges.Should().NotBeNull();
        marketChange.RunnerChanges.Should().HaveCount(1);

        var runnerChange = marketChange.RunnerChanges![0];
        runnerChange.SelectionId.Should().Be(270592);
        runnerChange.TotalMatched.Should().Be(5569.21);
        runnerChange.LastTradedPrice.Should().Be(11.5);
    }

    [Fact]
    public void CanHandleMultipleMessages()
    {
        var json1 = """{"op":"mcm","id":1,"pt":1743511672980}""" + "\n";
        var json2 = """{"op":"mcm","id":2,"pt":1743511672981}""" + "\n";
        var bytes1 = Encoding.UTF8.GetBytes(json1);
        var bytes2 = Encoding.UTF8.GetBytes(json2);

        var message1 = _deserializer.DeserializeChangeMessage(bytes1);
        var message2 = _deserializer.DeserializeChangeMessage(bytes2);

        message1.Should().NotBeNull();
        message1!.Id.Should().Be(1);
        message1.PublishTime.Should().Be(1743511672980);

        message2.Should().NotBeNull();
        message2!.Id.Should().Be(2);
        message2.PublishTime.Should().Be(1743511672981);
    }

    [Fact]
    public void SkipsInvalidMessages()
    {
        var invalidJson = """{"invalid": "json}""" + "\n"; // Missing closing quote
        var validJson = """{"op":"mcm","id":2,"pt":1743511672980}""" + "\n";
        var invalidBytes = Encoding.UTF8.GetBytes(invalidJson);
        var validBytes = Encoding.UTF8.GetBytes(validJson);

        var invalidMessage = _deserializer.DeserializeChangeMessage(invalidBytes);
        var validMessage = _deserializer.DeserializeChangeMessage(validBytes);

        invalidMessage.Should().BeNull(); // Invalid JSON should return null
        validMessage.Should().NotBeNull();
        validMessage!.Id.Should().Be(2);
    }

    [Fact]
    public void CanDeserializeSimpleChangeMessageFromByteArray()
    {
        var json = """{"op":"mcm","id":2,"clk":"AAAAAAAA","pt":1743511672980}""";
        var bytes = Encoding.UTF8.GetBytes(json);

        var changeMessage = _deserializer.DeserializeChangeMessage(bytes);

        changeMessage.Should().NotBeNull();
        changeMessage!.Operation.Should().Be("mcm");
        changeMessage.Id.Should().Be(2);
        changeMessage.Clock.Should().Be("AAAAAAAA");
        changeMessage.PublishTime.Should().Be(1743511672980);
    }

    [Fact]
    public void CanDeserializeSimpleChangeMessageFromSpan()
    {
        var json = """{"op":"mcm","id":2,"clk":"AAAAAAAA","pt":1743511672980}""";
        var bytes = Encoding.UTF8.GetBytes(json);

        var changeMessage = _deserializer.DeserializeChangeMessage(bytes.AsSpan());

        changeMessage.Should().NotBeNull();
        changeMessage!.Operation.Should().Be("mcm");
        changeMessage.Id.Should().Be(2);
        changeMessage.Clock.Should().Be("AAAAAAAA");
        changeMessage.PublishTime.Should().Be(1743511672980);
    }

    [Fact]
    public void ReturnsNullForInvalidJson()
    {
        var invalidJson = """{"op":"mcm","id":2,"clk":"AAAAAAAA","pt":""";
        var bytes = Encoding.UTF8.GetBytes(invalidJson);

        var changeMessage = _deserializer.DeserializeChangeMessage(bytes);

        changeMessage.Should().BeNull();
    }

    [Fact]
    public void ReturnsNullForEmptyByteArray()
    {
        var bytes = Array.Empty<byte>();

        var changeMessage = _deserializer.DeserializeChangeMessage(bytes);

        changeMessage.Should().BeNull();
    }

    [Fact]
    public void ReturnsNullForNullByteArray()
    {
        byte[]? bytes = null;

        var changeMessage = _deserializer.DeserializeChangeMessage(bytes!);

        changeMessage.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeComplexChangeMessageWithMarketChanges()
    {
        var json = """{"op":"mcm","id":2,"clk":"AAAAAAAA","pt":1743511672980,"mc":[{"id":"1.123456","tv":1000.50,"rc":[{"id":123,"tv":500.25,"ltp":2.5}]}]}""";
        var bytes = Encoding.UTF8.GetBytes(json);

        var changeMessage = _deserializer.DeserializeChangeMessage(bytes);

        changeMessage.Should().NotBeNull();
        changeMessage!.Operation.Should().Be("mcm");
        changeMessage.Id.Should().Be(2);
        changeMessage.Clock.Should().Be("AAAAAAAA");
        changeMessage.PublishTime.Should().Be(1743511672980);
        changeMessage.MarketChanges.Should().NotBeNull();
        changeMessage.MarketChanges!.Should().HaveCount(1);

        var marketChange = changeMessage.MarketChanges[0];
        marketChange.MarketId.Should().Be("1.123456");
        marketChange.TotalAmountMatched.Should().Be(1000.50);
        marketChange.RunnerChanges.Should().NotBeNull();
        marketChange.RunnerChanges!.Should().HaveCount(1);

        var runnerChange = marketChange.RunnerChanges[0];
        runnerChange.SelectionId.Should().Be(123);
        runnerChange.TotalMatched.Should().Be(500.25);
        runnerChange.LastTradedPrice.Should().Be(2.5);
    }

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
