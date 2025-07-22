using Betfair.Stream.Responses;

namespace Betfair.Tests.Stream.Responses;

public class OrderRunnerChangeTests
{
    [Fact]
    public void CanDeserializeOrderRunnerChangeWithStrategyMatchedData()
    {
        var json = """
        {
            "fullImage": true,
            "id": 80990361,
            "uo": [
                {
                    "id": "395588562633",
                    "p": 3.2,
                    "s": 1,
                    "side": "L",
                    "status": "E",
                    "pt": "L",
                    "ot": "L",
                    "pd": 1753200758000,
                    "sm": 0,
                    "sr": 1,
                    "sl": 0,
                    "sc": 0,
                    "sv": 0,
                    "rac": "",
                    "rc": "REG_GGC",
                    "rfo": "",
                    "rfs": "other"
                }
            ],
            "mb": [[3.2, 1], [3.35, 1]],
            "ml": [[3.35, 1]],
            "smc": {
                "other": {
                    "mb": [[3.2, 1]]
                },
                "myStrategy": {
                    "mb": [[3.35, 1]],
                    "ml": [[3.35, 1]]
                }
            }
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.OrderRunnerChange);

        result.Should().NotBeNull();
        result!.FullImage.Should().BeTrue();
        result.SelectionId.Should().Be(80990361);
        result.MatchedBacks.Should().HaveCount(2);
        result.MatchedBacks![0].Should().BeEquivalentTo(new[] { 3.2, 1.0 });
        result.MatchedBacks[1].Should().BeEquivalentTo(new[] { 3.35, 1.0 });
        result.MatchedLays.Should().HaveCount(1);
        result.MatchedLays![0].Should().BeEquivalentTo(new[] { 3.35, 1.0 });

        result.UnmatchedOrders.Should().HaveCount(1);
        var unmatchedOrder = result.UnmatchedOrders![0];
        unmatchedOrder.BetId.Should().Be("395588562633");
        unmatchedOrder.Price.Should().Be(3.2);
        unmatchedOrder.Size.Should().Be(1);
        unmatchedOrder.Side.Should().Be("L");
        unmatchedOrder.ReferenceStrategy.Should().Be("other");

        result.StrategyMatchedChange.Should().NotBeNull();
        result.StrategyMatchedChange.Should().HaveCount(2);
        result.StrategyMatchedChange.Should().ContainKey("other");
        result.StrategyMatchedChange.Should().ContainKey("myStrategy");

        var otherStrategy = result.StrategyMatchedChange!["other"];
        otherStrategy.MatchedBacks.Should().HaveCount(1);
        otherStrategy.MatchedBacks![0].Should().BeEquivalentTo(new[] { 3.2, 1.0 });
        otherStrategy.MatchedLays.Should().BeNull();

        var myStrategy = result.StrategyMatchedChange["myStrategy"];
        myStrategy.MatchedBacks.Should().HaveCount(1);
        myStrategy.MatchedBacks![0].Should().BeEquivalentTo(new[] { 3.35, 1.0 });
        myStrategy.MatchedLays.Should().HaveCount(1);
        myStrategy.MatchedLays![0].Should().BeEquivalentTo(new[] { 3.35, 1.0 });
    }

    [Fact]
    public void CanDeserializeOrderRunnerChangeWithoutStrategyMatchedData()
    {
        var json = """
        {
            "fullImage": true,
            "id": 80990361,
            "mb": [[3.2, 1]],
            "ml": [[3.35, 1]]
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.OrderRunnerChange);

        result.Should().NotBeNull();
        result!.FullImage.Should().BeTrue();
        result.SelectionId.Should().Be(80990361);
        result.MatchedBacks.Should().HaveCount(1);
        result.MatchedLays.Should().HaveCount(1);
        result.StrategyMatchedChange.Should().BeNull();
        result.UnmatchedOrders.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeStrategyMatchedDataWithOnlyMatchedBacks()
    {
        var json = """
        {
            "mb": [[3.2, 1], [3.35, 2]]
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StrategyMatchedData);

        result.Should().NotBeNull();
        result!.MatchedBacks.Should().HaveCount(2);
        result.MatchedBacks![0].Should().BeEquivalentTo(new[] { 3.2, 1.0 });
        result.MatchedBacks[1].Should().BeEquivalentTo(new[] { 3.35, 2.0 });
        result.MatchedLays.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeStrategyMatchedDataWithOnlyMatchedLays()
    {
        var json = """
        {
            "ml": [[3.35, 1]]
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StrategyMatchedData);

        result.Should().NotBeNull();
        result!.MatchedLays.Should().HaveCount(1);
        result.MatchedLays![0].Should().BeEquivalentTo(new[] { 3.35, 1.0 });
        result.MatchedBacks.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeStrategyMatchedDataWithBothMatchedBacksAndLays()
    {
        var json = """
        {
            "mb": [[3.2, 1]],
            "ml": [[3.35, 1]]
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StrategyMatchedData);

        result.Should().NotBeNull();
        result!.MatchedBacks.Should().HaveCount(1);
        result.MatchedBacks![0].Should().BeEquivalentTo(new[] { 3.2, 1.0 });
        result.MatchedLays.Should().HaveCount(1);
        result.MatchedLays![0].Should().BeEquivalentTo(new[] { 3.35, 1.0 });
    }
}
