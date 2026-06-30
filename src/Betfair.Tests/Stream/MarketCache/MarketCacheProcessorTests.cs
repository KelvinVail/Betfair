#pragma warning disable SA1009 // StyleCop false positive with C# 11 UTF-8 string literals (u8 suffix)

using System.Text;
using Betfair.Stream.MarketCache;

namespace Betfair.Tests.Stream.MarketCache;

public class MarketCacheProcessorTests
{
    private readonly MarketCacheProcessor _processor = new ();

    [Fact]
    public void ProcessEmptyObjectDoesNotThrow()
    {
        _processor.Process("{}"u8);

        _processor.Markets.Should().BeEmpty();
    }

    [Fact]
    public void ProcessNonMcmOperationIsIgnored()
    {
        var data = """{"op":"connection","connectionId":"abc"}"""u8;

        _processor.Process(data);

        _processor.Markets.Should().BeEmpty();
    }

    [Fact]
    public void ProcessExtractsPublishTime()
    {
        var data = """{"op":"mcm","id":2,"pt":1743511672980,"mc":[]}"""u8;

        _processor.Process(data);

        _processor.PublishTime.Should().Be(1743511672980);
    }

    [Fact]
    public void ProcessExtractsClockToken()
    {
        var data = """{"op":"mcm","id":2,"clk":"AAMABAAE","pt":1743511673000,"mc":[]}"""u8;

        _processor.Process(data);

        _processor.Clock.Should().Be("AAMABAAE");
    }

    [Fact]
    public void ProcessExtractsInitialClockToken()
    {
        var data = """{"op":"mcm","id":2,"initialClk":"ic123","clk":"c456","pt":100,"mc":[]}"""u8;

        _processor.Process(data);

        _processor.InitialClock.Should().Be("ic123");
        _processor.Clock.Should().Be("c456");
    }

    [Fact]
    public void ClockBytesReturnsUtf8EncodedClock()
    {
        var data = """{"op":"mcm","id":2,"clk":"AAMABAAE","pt":100,"mc":[]}"""u8;

        _processor.Process(data);

        _processor.ClockBytes.ToArray().Should().BeEquivalentTo(Encoding.UTF8.GetBytes("AAMABAAE"));
    }

    [Fact]
    public void ClockIsNullBeforeFirstProcess()
    {
        _processor.Clock.Should().BeNull();
    }

    [Fact]
    public void InitialClockIsNullBeforeFirstProcess()
    {
        _processor.InitialClock.Should().BeNull();
    }

    [Fact]
    public void ProcessCreatesMarketFromMcArray()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0,"tv":100.0,"atb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123").Should().NotBeNull();
    }

    [Fact]
    public void ProcessSetsMarketPublishTime()
    {
        var data = """{"op":"mcm","id":2,"pt":9999,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.PublishTime.Should().Be(9999);
    }

    [Fact]
    public void ProcessSetsRunnerLastTradedPrice()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.5}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111);
        runner!.LastTradedPrice.Should().Be(5.5);
    }

    [Fact]
    public void ProcessSetsRunnerTotalMatched()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"tv":1234.56}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111);
        runner!.TotalMatched.Should().Be(1234.56);
    }

    [Fact]
    public void ProcessPopulatesAvailableToBackLadder()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atb":[[2.5,100.0],[3.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToBack[2.5].Should().Be(100.0);
        runner.AvailableToBack[3.0].Should().Be(50.0);
        runner.AvailableToBack.Count.Should().Be(2);
    }

    [Fact]
    public void ProcessPopulatesAvailableToLayLadder()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atl":[[4.0,200.0],[5.0,75.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToLay[4.0].Should().Be(200.0);
        runner.AvailableToLay[5.0].Should().Be(75.0);
    }

    [Fact]
    public void ProcessPopulatesTradedLadder()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"trd":[[5.0,500.0],[6.0,300.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.Traded[5.0].Should().Be(500.0);
        runner.Traded[6.0].Should().Be(300.0);
    }

    [Fact]
    public void ProcessPopulatesBestAvailableToBack()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"batb":[[0,5.0,100.0],[1,4.5,50.0],[2,4.0,25.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.BestAvailableToBack.GetPrice(0).Should().Be(5.0);
        runner.BestAvailableToBack.GetSize(0).Should().Be(100.0);
        runner.BestAvailableToBack.GetPrice(1).Should().Be(4.5);
        runner.BestAvailableToBack.GetSize(1).Should().Be(50.0);
        runner.BestAvailableToBack.GetPrice(2).Should().Be(4.0);
        runner.BestAvailableToBack.GetSize(2).Should().Be(25.0);
    }

    [Fact]
    public void ProcessPopulatesBestAvailableToLay()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"batl":[[0,6.0,80.0],[1,6.5,40.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.BestAvailableToLay.GetPrice(0).Should().Be(6.0);
        runner.BestAvailableToLay.GetSize(0).Should().Be(80.0);
        runner.BestAvailableToLay.GetPrice(1).Should().Be(6.5);
        runner.BestAvailableToLay.GetSize(1).Should().Be(40.0);
    }

    [Fact]
    public void ProcessSetsStartingPriceNear()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"spn":3.5}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StartingPriceNear.Should().Be(3.5);
    }

    [Fact]
    public void ProcessSetsStartingPriceFar()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"spf":7.0}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StartingPriceFar.Should().Be(7.0);
    }

    [Fact]
    public void ProcessPopulatesStartingPriceBackLadder()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"spb":[[3.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StartingPriceBack[3.0].Should().Be(50.0);
    }

    [Fact]
    public void ProcessPopulatesStartingPriceLayLadder()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"spl":[[4.0,60.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StartingPriceLay[4.0].Should().Be(60.0);
    }

    [Fact]
    public void ProcessSetsMarketTotalMatched()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","tv":67505.47,"rc":[{"id":111,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.TotalMatched.Should().Be(67505.47);
    }

    [Fact]
    public void ProcessImageFlagClearsRunnersBeforePopulating()
    {
        // First delta adds a runner
        var delta = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0}]}]}"""u8;
        _processor.Process(delta);

        // Image replaces existing state
        var image = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","img":true,"rc":[{"id":222,"ltp":6.0}]}]}"""u8;
        _processor.Process(image);

        var market = _processor.GetMarket("1.123")!;
        market.IsImage.Should().BeTrue();
        market.GetRunner(111).Should().BeNull();
        market.GetRunner(222).Should().NotBeNull();
        market.GetRunner(222)!.LastTradedPrice.Should().Be(6.0);
    }

    [Fact]
    public void ProcessDeltaUpdatesExistingRunnerState()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0,"atb":[[2.0,100.0]]}]}]}"""u8;
        _processor.Process(first);

        var second = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":6.0,"atb":[[2.0,150.0],[3.0,50.0]]}]}]}"""u8;
        _processor.Process(second);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.LastTradedPrice.Should().Be(6.0);
        runner.AvailableToBack[2.0].Should().Be(150.0);
        runner.AvailableToBack[3.0].Should().Be(50.0);
    }

    [Fact]
    public void ProcessHandlesMultipleRunnersInSingleMessage()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0},{"id":222,"ltp":8.0},{"id":333,"ltp":12.0}]}]}"""u8;

        _processor.Process(data);

        var market = _processor.GetMarket("1.123")!;
        market.GetRunner(111)!.LastTradedPrice.Should().Be(5.0);
        market.GetRunner(222)!.LastTradedPrice.Should().Be(8.0);
        market.GetRunner(333)!.LastTradedPrice.Should().Be(12.0);
    }

    [Fact]
    public void ProcessHandlesMultipleMarketsInSingleMessage()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.111","rc":[{"id":1,"ltp":2.0}]},{"id":"1.222","rc":[{"id":2,"ltp":3.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.111")!.GetRunner(1)!.LastTradedPrice.Should().Be(2.0);
        _processor.GetMarket("1.222")!.GetRunner(2)!.LastTradedPrice.Should().Be(3.0);
    }

    [Fact]
    public void GetMarketReturnsNullForUnknownMarket()
    {
        _processor.GetMarket("1.999").Should().BeNull();
    }

    [Fact]
    public void MarketsPropertyReturnsSingleMarketDictionary()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0}]}]}"""u8;
        _processor.Process(data);

        _processor.Markets.Should().HaveCount(1);
        _processor.Markets.Should().ContainKey("1.123");
    }

    [Fact]
    public void ProcessSetsLastProcessingTime()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        _processor.LastProcessingTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public void ProcessHandlesRunnerWithHandicap()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"hc":1.5,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.Handicap.Should().Be(1.5);
    }

    [Fact]
    public void ProcessMalformedJsonThrowsJsonReaderException()
    {
        var act = () => _processor.Process("not json at all"u8);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ProcessReadsMarketDefinition()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","marketDefinition":{"status":"OPEN","inPlay":false,"venue":"Lingfield","countryCode":"GB","betDelay":0,"numberOfActiveRunners":9},"rc":[{"id":111,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        var def = _processor.GetMarket("1.123")!.Definition;
        def.Status.Should().Be("OPEN");
        def.InPlay.Should().BeFalse();
        def.Venue.Should().Be("Lingfield");
        def.CountryCode.Should().Be("GB");
        def.BetDelay.Should().Be(0);
        def.NumberOfActiveRunners.Should().Be(9);
    }

    [Fact]
    public void ProcessZeroSizeRemovesFromLadder()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atb":[[2.0,100.0],[3.0,50.0]]}]}]}"""u8;
        _processor.Process(first);

        var second = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","rc":[{"id":111,"atb":[[2.0,0]]}]}]}"""u8;
        _processor.Process(second);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToBack[2.0].Should().Be(0);
        runner.AvailableToBack[3.0].Should().Be(50.0);
        runner.AvailableToBack.Count.Should().Be(1);
    }

    [Fact]
    public void ProcessFullStreamLineFromTestData()
    {
        // Use first line from actual stream data (the SUB_IMAGE)
        var line = File.ReadAllLines("Stream/Data/MarketStream.txt")[0];
        var data = Encoding.UTF8.GetBytes(line);

        _processor.Process(data);

        var market = _processor.GetMarket("1.241629436");
        market.Should().NotBeNull();
        market!.IsImage.Should().BeTrue();
        market.TotalMatched.Should().Be(67505.47);
        market.RunnerCount.Should().Be(9);

        // Verify a specific runner from the image
        var runner = market.GetRunner(270592)!;
        runner.LastTradedPrice.Should().Be(5.2);
        runner.TotalMatched.Should().Be(14939.77);
        runner.BestAvailableToBack.GetPrice(0).Should().Be(5.2);
        runner.BestAvailableToBack.GetSize(0).Should().Be(82.98);

        // Market definition should be parsed
        market.Definition.Status.Should().Be("OPEN");
        market.Definition.Venue.Should().Be("Lingfield");
        market.Definition.CountryCode.Should().Be("GB");
        market.Definition.NumberOfActiveRunners.Should().Be(9);
    }

    [Fact]
    public void ProcessMultipleStreamLinesAppliesDeltasCorrectly()
    {
        var lines = File.ReadAllLines("Stream/Data/MarketStream.txt");

        // Process image + several deltas
        foreach (var line in lines.Take(5))
            _processor.Process(Encoding.UTF8.GetBytes(line));

        var market = _processor.GetMarket("1.241629436")!;

        // After delta on line 5 (index 4): runner 270592 atb at 5.2 updated to 139.47
        var runner = market.GetRunner(270592)!;
        runner.AvailableToBack[5.2].Should().Be(139.47);

        // Clock should be updated to the last processed line's clock
        _processor.Clock.Should().NotBeNull();
        _processor.PublishTime.Should().BeGreaterThan(0);
    }

    [Fact]
    public void ProcessPopulatesBestDisplayAvailableToBack()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"bdatb":[[0,5.0,100.0],[1,4.5,50.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.BestDisplayAvailableToBack.GetPrice(0).Should().Be(5.0);
        runner.BestDisplayAvailableToBack.GetSize(0).Should().Be(100.0);
        runner.BestDisplayAvailableToBack.GetPrice(1).Should().Be(4.5);
        runner.BestDisplayAvailableToBack.GetSize(1).Should().Be(50.0);
    }

    [Fact]
    public void ProcessPopulatesBestDisplayAvailableToLay()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"bdatl":[[0,6.0,80.0],[1,6.5,40.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.BestDisplayAvailableToLay.GetPrice(0).Should().Be(6.0);
        runner.BestDisplayAvailableToLay.GetSize(0).Should().Be(80.0);
        runner.BestDisplayAvailableToLay.GetPrice(1).Should().Be(6.5);
        runner.BestDisplayAvailableToLay.GetSize(1).Should().Be(40.0);
    }

    [Fact]
    public void ProcessImageBeforeRcClearsAndRepopulates()
    {
        // Pre-populate a runner
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0},{"id":222,"ltp":3.0}]}]}"""u8;
        _processor.Process(first);

        // Image with img BEFORE rc in property order
        var image = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","img":true,"rc":[{"id":333,"ltp":9.0}]}]}"""u8;
        _processor.Process(image);

        var market = _processor.GetMarket("1.123")!;
        market.GetRunner(111).Should().BeNull();
        market.GetRunner(222).Should().BeNull();
        market.GetRunner(333)!.LastTradedPrice.Should().Be(9.0);
    }

    [Fact]
    public void ProcessUpdatesClockOnEachMessage()
    {
        var first = """{"op":"mcm","id":2,"clk":"AAA","pt":100,"mc":[]}"""u8;
        _processor.Process(first);
        _processor.Clock.Should().Be("AAA");

        var second = """{"op":"mcm","id":2,"clk":"BBB","pt":200,"mc":[]}"""u8;
        _processor.Process(second);
        _processor.Clock.Should().Be("BBB");
    }

    [Fact]
    public void ProcessSkipsUnknownTopLevelProperties()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"unknownProp":{"nested":true},"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.GetRunner(111)!.LastTradedPrice.Should().Be(5.0);
    }

    [Fact]
    public void ProcessSkipsUnknownRunnerProperties()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"unknownProp":42,"ltp":7.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.GetRunner(111)!.LastTradedPrice.Should().Be(7.0);
    }
}
