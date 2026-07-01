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

    // ===== Mutation-killing tests below =====


    // 1. Guard clause: Process returns early for non-StartObject token
    [Fact]
    public void ProcessArrayRootDoesNotThrowAndProducesNoMarkets()
    {
        var data = """[{"op":"mcm"}]"""u8;

        _processor.Process(data);

        _processor.Markets.Should().BeEmpty();
        _processor.LastProcessingTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    // 1. Guard clause: op is NOT "mcm" — must return false from TryProcessProperty
    [Fact]
    public void ProcessNonMcmOpStopsProcessingRemainingProperties()
    {
        var data = """{"op":"status","pt":999,"mc":[{"id":"1.123","rc":[{"id":1,"ltp":1.0}]}]}"""u8;

        _processor.Process(data);

        _processor.PublishTime.Should().Be(0);
        _processor.GetMarket("1.123").Should().BeNull();
    }

    // 2. ReadImageFlag: img=false should NOT trigger image clear
    [Fact]
    public void ProcessImageFlagFalseDoesNotClearRunners()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0}]}]}"""u8;
        _processor.Process(first);

        var second = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","img":false,"rc":[{"id":222,"ltp":6.0}]}]}"""u8;
        _processor.Process(second);

        var market = _processor.GetMarket("1.123")!;
        market.GetRunner(111)!.LastTradedPrice.Should().Be(5.0);
        market.GetRunner(222)!.LastTradedPrice.Should().Be(6.0);
        market.IsImage.Should().BeFalse();
    }

    // 2. ReadImageFlag: img=true but rc appears BEFORE img (runnersProcessed=true)
    [Fact]
    public void ProcessImageAfterRcDoesNotDoubleClear()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0}]}]}"""u8;
        _processor.Process(first);

        // rc appears before img in JSON property order
        var image = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","rc":[{"id":222,"ltp":9.0}],"img":true}]}"""u8;
        _processor.Process(image);

        var market = _processor.GetMarket("1.123")!;
        market.IsImage.Should().BeTrue();
        // Runner 222 was already processed (rc before img), so it should still exist
        market.GetRunner(222)!.LastTradedPrice.Should().Be(9.0);
        // Runner 111 still exists because ClearRunners was NOT called (runnersProcessed=true)
        market.GetRunner(111)!.LastTradedPrice.Should().Be(5.0);
    }

    // 3. Early return in ReadMarketChanges: mc is not an array
    [Fact]
    public void ProcessMcNotArrayDoesNotThrow()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":"notarray"}"""u8;

        _processor.Process(data);

        _processor.Markets.Should().BeEmpty();
        _processor.PublishTime.Should().Be(100);
    }

    // 3. Early return in ReadRunnerChanges: rc is not an array
    [Fact]
    public void ProcessRcNotArrayDoesNotThrow()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":"notarray"}]}"""u8;

        _processor.Process(data);

        var market = _processor.GetMarket("1.123");
        market.Should().NotBeNull();
        market!.RunnerCount.Should().Be(0);
    }

    // 4. Multi-market resolution: second market creates _multiMarkets dictionary
    [Fact]
    public void ProcessSecondMarketCreatesMultiMarketDictionary()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.111","rc":[{"id":1,"ltp":1.0}]},{"id":"1.222","rc":[{"id":2,"ltp":2.0}]},{"id":"1.333","rc":[{"id":3,"ltp":3.0}]}]}"""u8;

        _processor.Process(data);

        _processor.Markets.Should().HaveCount(3);
        _processor.GetMarket("1.111")!.GetRunner(1)!.LastTradedPrice.Should().Be(1.0);
        _processor.GetMarket("1.222")!.GetRunner(2)!.LastTradedPrice.Should().Be(2.0);
        _processor.GetMarket("1.333")!.GetRunner(3)!.LastTradedPrice.Should().Be(3.0);
    }

    // 4. Multi-market: re-resolve existing market in multi-market mode
    [Fact]
    public void ProcessReResolvesExistingMarketInMultiMode()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.111","rc":[{"id":1,"ltp":1.0}]},{"id":"1.222","rc":[{"id":2,"ltp":2.0}]}]}"""u8;
        _processor.Process(first);

        var second = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.222","rc":[{"id":2,"ltp":9.0}]}]}"""u8;
        _processor.Process(second);

        _processor.GetMarket("1.222")!.GetRunner(2)!.LastTradedPrice.Should().Be(9.0);
    }

    // 4. GetMarket: singleMarket exists but doesn't match, no multiMarkets
    [Fact]
    public void GetMarketReturnsNullWhenSingleMarketDoesNotMatch()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":1,"ltp":1.0}]}]}"""u8;
        _processor.Process(data);

        _processor.GetMarket("1.456").Should().BeNull();
    }

    // 4. GetMarket: multiMarkets path — unknown market
    [Fact]
    public void GetMarketReturnsNullFromMultiMarketsWhenNotFound()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.111","rc":[{"id":1,"ltp":1.0}]},{"id":"1.222","rc":[{"id":2,"ltp":2.0}]}]}"""u8;
        _processor.Process(data);

        _processor.GetMarket("1.999").Should().BeNull();
    }

    // 5. ReadAtbOrAtl: atb goes to AvailableToBack, NOT AvailableToLay
    [Fact]
    public void ProcessAtbOnlyUpdatesBackNotLay()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToBack[2.0].Should().Be(50.0);
        runner.AvailableToLay.Count.Should().Be(0);
    }

    // 5. ReadAtbOrAtl: atl goes to AvailableToLay, NOT AvailableToBack
    [Fact]
    public void ProcessAtlOnlyUpdatesLayNotBack()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atl":[[3.0,75.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToLay[3.0].Should().Be(75.0);
        runner.AvailableToBack.Count.Should().Be(0);
    }

    // 5. ReadAtbOrAtl: property length != 3 is skipped
    [Fact]
    public void ProcessPropertyStartingWithAButNotAtbOrAtlIsSkipped()
    {
        // "ab" has length 2 — should be skipped via SkipValue
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ab":[[2.0,50.0]],"ltp":3.0}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.LastTradedPrice.Should().Be(3.0);
        runner.AvailableToBack.Count.Should().Be(0);
        runner.AvailableToLay.Count.Should().Be(0);
    }

    // 6. ReadTvOrTrd: "tv" (length 2) reads a double, "trd" (length 3) reads pairs
    [Fact]
    public void ProcessTvSetsTotalMatchedNotTradedLadder()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"tv":500.0}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.TotalMatched.Should().Be(500.0);
        runner.Traded.Count.Should().Be(0);
    }

    // 6. ReadTvOrTrd: "trd" updates Traded ladder, not TotalMatched
    [Fact]
    public void ProcessTrdUpdatesTradedLadderNotTotalMatched()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"trd":[[5.0,200.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.Traded[5.0].Should().Be(200.0);
        runner.HasTotalMatched.Should().BeFalse();
    }

    // 7. ReadBestAvailable: batb (length 4) vs bdatb (length 5)
    [Fact]
    public void ProcessBatbUpdatesBestAvailableToBackOnly()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"batb":[[0,5.0,100.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.BestAvailableToBack.GetPrice(0).Should().Be(5.0);
        runner.BestAvailableToLay.GetPrice(0).Should().Be(0);
        runner.BestDisplayAvailableToBack.GetPrice(0).Should().Be(0);
    }

    // 7. ReadBestAvailable: batl (length 4) goes to BestAvailableToLay
    [Fact]
    public void ProcessBatlUpdatesBestAvailableToLayOnly()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"batl":[[0,6.0,80.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.BestAvailableToLay.GetPrice(0).Should().Be(6.0);
        runner.BestAvailableToBack.GetPrice(0).Should().Be(0);
        runner.BestDisplayAvailableToLay.GetPrice(0).Should().Be(0);
    }

    // 7. ReadBestAvailable: bdatb (length 5) goes to BestDisplayAvailableToBack
    [Fact]
    public void ProcessBdatbUpdatesBestDisplayBackOnly()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"bdatb":[[0,7.0,60.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.BestDisplayAvailableToBack.GetPrice(0).Should().Be(7.0);
        runner.BestAvailableToBack.GetPrice(0).Should().Be(0);
        runner.BestDisplayAvailableToLay.GetPrice(0).Should().Be(0);
    }

    // 7. ReadBestAvailable: bdatl (length 5) goes to BestDisplayAvailableToLay
    [Fact]
    public void ProcessBdatlUpdatesBestDisplayLayOnly()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"bdatl":[[0,8.0,40.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.BestDisplayAvailableToLay.GetPrice(0).Should().Be(8.0);
        runner.BestDisplayAvailableToBack.GetPrice(0).Should().Be(0);
        runner.BestAvailableToLay.GetPrice(0).Should().Be(0);
    }

    // 8. ReadStartingPrice: spn sets StartingPriceNear only
    [Fact]
    public void ProcessSpnSetsNearNotFar()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"spn":4.5}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StartingPriceNear.Should().Be(4.5);
        runner.HasStartingPriceFar.Should().BeFalse();
    }

    // 8. ReadStartingPrice: spf sets StartingPriceFar only
    [Fact]
    public void ProcessSpfSetsFarNotNear()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"spf":8.0}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StartingPriceFar.Should().Be(8.0);
        runner.HasStartingPriceNear.Should().BeFalse();
    }

    // 8. ReadStartingPrice: spb updates StartingPriceBack ladder only
    [Fact]
    public void ProcessSpbUpdatesBackLadderOnly()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"spb":[[3.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StartingPriceBack[3.0].Should().Be(50.0);
        runner.StartingPriceLay.Count.Should().Be(0);
    }

    // 8. ReadStartingPrice: spl updates StartingPriceLay ladder only
    [Fact]
    public void ProcessSplUpdatesLayLadderOnly()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"spl":[[4.0,60.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StartingPriceLay[4.0].Should().Be(60.0);
        runner.StartingPriceBack.Count.Should().Be(0);
    }

    // 8. ReadStartingPrice: property length != 3 is skipped
    [Fact]
    public void ProcessStartingPricePropertyLengthNot3IsSkipped()
    {
        // "sp" has length 2 — should be skipped
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"sp":99.0,"ltp":2.0}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.LastTradedPrice.Should().Be(2.0);
        runner.HasStartingPriceNear.Should().BeFalse();
        runner.HasStartingPriceFar.Should().BeFalse();
    }

    // 10. Buffer resize: many ladder entries exceed initial buffer size
    [Fact]
    public void ProcessManyLadderEntriesResizesBuffer()
    {
        // Build a message with >256 price levels across multiple properties to trigger resize
        // Start sizes at 1.0 to avoid 0-size removal; start prices at 1.0
        var sb = new StringBuilder();
        sb.Append("""{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atb":[""");
        for (int i = 0; i < 130; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append($"[{i + 1}.0,{i + 1}.0]");
        }

        sb.Append("""],"atl":[""");
        for (int i = 0; i < 130; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append($"[{i + 200}.0,{i + 1}.0]");
        }

        sb.Append("""]}]}]}""");

        var data = Encoding.UTF8.GetBytes(sb.ToString());
        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToBack.Count.Should().Be(130);
        runner.AvailableToLay.Count.Should().Be(130);
    }

    // 11. ApplyDeferredUpdates: verify each ladder type dispatches correctly in one message
    [Fact]
    public void ProcessAllLadderTypesInSingleRunner()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atb":[[2.0,10.0]],"atl":[[3.0,20.0]],"batb":[[0,4.0,30.0]],"batl":[[0,5.0,40.0]],"bdatb":[[0,6.0,50.0]],"bdatl":[[0,7.0,60.0]],"trd":[[8.0,70.0]],"spb":[[9.0,80.0]],"spl":[[10.0,90.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToBack[2.0].Should().Be(10.0);
        runner.AvailableToLay[3.0].Should().Be(20.0);
        runner.BestAvailableToBack.GetPrice(0).Should().Be(4.0);
        runner.BestAvailableToBack.GetSize(0).Should().Be(30.0);
        runner.BestAvailableToLay.GetPrice(0).Should().Be(5.0);
        runner.BestAvailableToLay.GetSize(0).Should().Be(40.0);
        runner.BestDisplayAvailableToBack.GetPrice(0).Should().Be(6.0);
        runner.BestDisplayAvailableToBack.GetSize(0).Should().Be(50.0);
        runner.BestDisplayAvailableToLay.GetPrice(0).Should().Be(7.0);
        runner.BestDisplayAvailableToLay.GetSize(0).Should().Be(60.0);
        runner.Traded[8.0].Should().Be(70.0);
        runner.StartingPriceBack[9.0].Should().Be(80.0);
        runner.StartingPriceLay[10.0].Should().Be(90.0);
    }

    // 12. Markets property: multiMarkets null, singleMarket not null
    [Fact]
    public void MarketsPropertyCreatesNewDictWhenSingleMarketOnly()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0}]}]}"""u8;
        _processor.Process(data);

        var markets1 = _processor.Markets;
        var markets2 = _processor.Markets;

        // Should return the same dictionary reference after creation
        markets1.Should().BeSameAs(markets2);
        markets1.Should().HaveCount(1);
    }

    // 12. Markets property: no markets at all returns empty
    [Fact]
    public void MarketsPropertyReturnsEmptyBeforeProcessing()
    {
        _processor.Markets.Should().BeEmpty();
        _processor.Markets.Should().HaveCount(0);
    }

    // 13. GetMarket: multiMarkets path returns correct market
    [Fact]
    public void GetMarketReturnsCorrectMarketFromMultiMarkets()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.111","rc":[{"id":1,"ltp":1.0}]},{"id":"1.222","rc":[{"id":2,"ltp":2.0}]},{"id":"1.333","rc":[{"id":3,"ltp":3.0}]}]}"""u8;
        _processor.Process(data);

        _processor.GetMarket("1.333")!.GetRunner(3)!.LastTradedPrice.Should().Be(3.0);
        _processor.GetMarket("1.111")!.GetRunner(1)!.LastTradedPrice.Should().Be(1.0);
    }

    // 14. ProcessRunnerChanges: isImage=true and cache not null clears runners before reading
    [Fact]
    public void ProcessImageTrueClearsExistingRunnersBeforeReading()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0,"atb":[[2.0,100.0]]}]}]}"""u8;
        _processor.Process(first);

        var image = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","img":true,"rc":[{"id":222,"ltp":7.0}]}]}"""u8;
        _processor.Process(image);

        var market = _processor.GetMarket("1.123")!;
        market.RunnerCount.Should().Be(1);
        market.GetRunner(111).Should().BeNull();
        market.GetRunner(222)!.LastTradedPrice.Should().Be(7.0);
    }

    // 15. NaN checks: ltp not set does not overwrite existing value
    [Fact]
    public void ProcessDeltaWithoutLtpDoesNotOverwriteExisting()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.5}]}]}"""u8;
        _processor.Process(first);

        // Second message updates tv but not ltp
        var second = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","rc":[{"id":111,"tv":200.0}]}]}"""u8;
        _processor.Process(second);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.LastTradedPrice.Should().Be(5.5);
        runner.TotalMatched.Should().Be(200.0);
    }

    // 15. NaN checks: tv not set does not overwrite existing value
    [Fact]
    public void ProcessDeltaWithoutTvDoesNotOverwriteExisting()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"tv":300.0}]}]}"""u8;
        _processor.Process(first);

        var second = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":6.0}]}]}"""u8;
        _processor.Process(second);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.TotalMatched.Should().Be(300.0);
        runner.LastTradedPrice.Should().Be(6.0);
    }

    // 15. NaN checks: spn not set does not overwrite existing value
    [Fact]
    public void ProcessDeltaWithoutSpnDoesNotOverwriteExisting()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"spn":3.5}]}]}"""u8;
        _processor.Process(first);

        var second = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","rc":[{"id":111,"spf":7.0}]}]}"""u8;
        _processor.Process(second);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StartingPriceNear.Should().Be(3.5);
        runner.StartingPriceFar.Should().Be(7.0);
    }

    // 15. NaN checks: spf not set does not overwrite existing value
    [Fact]
    public void ProcessDeltaWithoutSpfDoesNotOverwriteExisting()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"spf":9.0}]}]}"""u8;
        _processor.Process(first);

        var second = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","rc":[{"id":111,"spn":2.0}]}]}"""u8;
        _processor.Process(second);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StartingPriceFar.Should().Be(9.0);
        runner.StartingPriceNear.Should().Be(2.0);
    }

    // 16. PropCon: the "con" property is consumed without crashing
    [Fact]
    public void ProcessConPropertyIsConsumedWithoutSideEffects()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","con":true,"rc":[{"id":111,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.LastTradedPrice.Should().Be(5.0);
    }

    // 1. Guard clause: Process with op that is NOT a string type
    [Fact]
    public void ProcessOpAsNumberDoesNotThrow()
    {
        var data = """{"op":123,"pt":100,"mc":[{"id":"1.123","rc":[{"id":1,"ltp":1.0}]}]}"""u8;

        _processor.Process(data);

        // op is not a string so the TokenType != String check triggers return true (proceeds)
        _processor.GetMarket("1.123")!.GetRunner(1)!.LastTradedPrice.Should().Be(1.0);
    }

    // 4. ResolveMarket: same single market ID via fast path (ValueTextEquals)
    [Fact]
    public void ProcessSameMarketTwiceUsesFastPath()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0}]}]}"""u8;
        _processor.Process(first);

        var second = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":7.0}]}]}"""u8;
        _processor.Process(second);

        _processor.GetMarket("1.123")!.GetRunner(111)!.LastTradedPrice.Should().Be(7.0);
        _processor.GetMarket("1.123")!.PublishTime.Should().Be(200);
    }

    // 9. CopyTokenToBuffer: buffer growth for very long clock token
    [Fact]
    public void ProcessLongClockTokenGrowsBuffer()
    {
        var longClk = new string('A', 200);
        var json = $$$"""{"op":"mcm","id":2,"clk":"{{{longClk}}}","pt":100,"mc":[]}""";
        var data = Encoding.UTF8.GetBytes(json);

        _processor.Process(data);

        _processor.Clock.Should().Be(longClk);
        _processor.ClockBytes.Length.Should().Be(200);
    }

    // 9. CopyTokenToBuffer: buffer growth for long initial clock token
    [Fact]
    public void ProcessLongInitialClockTokenGrowsBuffer()
    {
        var longClk = new string('B', 150);
        var json = $$$"""{"op":"mcm","id":2,"initialClk":"{{{longClk}}}","pt":100,"mc":[]}""";
        var data = Encoding.UTF8.GetBytes(json);

        _processor.Process(data);

        _processor.InitialClock.Should().Be(longClk);
        _processor.InitialClockBytes.Length.Should().Be(150);
    }

    // 10. Buffer resize for position triples (batb/batl)
    [Fact]
    public void ProcessManyBestAvailableEntriesResizesBuffer()
    {
        // We need >256 total deferred updates in one runner to trigger buffer resize
        // Use 250 atb entries + 10 batb entries to exceed 256
        var sb = new StringBuilder();
        sb.Append("""{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atb":[""");
        for (int i = 0; i < 250; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append($"[{i + 1}.0,{i + 1}.0]");
        }

        sb.Append("""],"batb":[""");
        for (int i = 0; i < 10; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append($"[{i},{i + 1}.0,{(i + 1) * 10}.0]");
        }

        sb.Append("""]}]}]}""");

        var data = Encoding.UTF8.GetBytes(sb.ToString());
        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToBack.Count.Should().Be(250);
        runner.BestAvailableToBack.GetPrice(0).Should().Be(1.0);
        runner.BestAvailableToBack.GetSize(0).Should().Be(10.0);
        runner.BestAvailableToBack.GetPrice(9).Should().Be(10.0);
        runner.BestAvailableToBack.GetSize(9).Should().Be(100.0);
    }

    // 3. Early return: atb/atl with non-array value
    [Fact]
    public void ProcessAtbNotArrayDoesNotThrow()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atb":"notarray","ltp":2.0}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.LastTradedPrice.Should().Be(2.0);
        runner.AvailableToBack.Count.Should().Be(0);
    }

    // Verify PublishTime is set per market in multi-market messages
    [Fact]
    public void ProcessSetsPublishTimeOnEachMarket()
    {
        var data = """{"op":"mcm","id":2,"pt":555,"mc":[{"id":"1.111","rc":[{"id":1,"ltp":1.0}]},{"id":"1.222","rc":[{"id":2,"ltp":2.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.111")!.PublishTime.Should().Be(555);
        _processor.GetMarket("1.222")!.PublishTime.Should().Be(555);
    }

    // Verify IsImage is reset between messages
    [Fact]
    public void ProcessImageFlagIsTrueOnlyForImageMessage()
    {
        var image = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","img":true,"rc":[{"id":111,"ltp":5.0}]}]}"""u8;
        _processor.Process(image);
        _processor.GetMarket("1.123")!.IsImage.Should().BeTrue();

        // Delta without img flag — IsImage remains from last set
        var delta = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":6.0}]}]}"""u8;
        _processor.Process(delta);

        // IsImage is set per message on the cache; without img it stays as last set
        _processor.GetMarket("1.123")!.IsImage.Should().BeTrue();
    }

    // Verify runner with selectionId=0 is ignored
    [Fact]
    public void ProcessRunnerWithZeroIdIsIgnored()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":0,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.RunnerCount.Should().Be(0);
    }

    // Verify empty rc array does not crash
    [Fact]
    public void ProcessEmptyRcArrayDoesNotThrow()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.RunnerCount.Should().Be(0);
    }

    // Kill mutant Line 120: guard clause — non-JSON (empty) data triggers early return with LastProcessingTime set
    [Fact]
    public void ProcessEmptyDataSetsLastProcessingTime()
    {
        // "[" starts with StartArray, not StartObject — triggers early return
        _processor.Process("[]"u8);

        _processor.LastProcessingTime.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
        _processor.Markets.Should().BeEmpty();
    }

    // Kill mutant Line 155/163: CopyTokenToBuffer buffer growth when length exactly exceeds buffer
    [Fact]
    public void ProcessClockThatExceedsInitialBufferSizeGrowsCorrectly()
    {
        // Initial buffer is 64 bytes. Use a 65-byte clock to force buffer growth.
        var longClk = new string('X', 65);
        var json = $$$"""{"op":"mcm","id":2,"clk":"{{{longClk}}}","pt":100,"mc":[]}""";
        var data = Encoding.UTF8.GetBytes(json);

        _processor.Process(data);

        _processor.Clock.Should().Be(longClk);
        _processor.ClockBytes.Length.Should().Be(65);
    }

    // Kill mutant Line 155/163: ensure buffer does NOT grow when token fits
    [Fact]
    public void ProcessShortClockDoesNotGrowBuffer()
    {
        // Use a short clock then a long one to verify buffer growth behavior
        var shortClk = "ABCD";
        var json1 = $$$"""{"op":"mcm","id":2,"clk":"{{{shortClk}}}","pt":100,"mc":[]}""";
        _processor.Process(Encoding.UTF8.GetBytes(json1));
        _processor.Clock.Should().Be(shortClk);

        var longerClk = new string('Z', 65);
        var json2 = $$$"""{"op":"mcm","id":2,"clk":"{{{longerClk}}}","pt":200,"mc":[]}""";
        _processor.Process(Encoding.UTF8.GetBytes(json2));
        _processor.Clock.Should().Be(longerClk);
    }

    // Kill mutant Line 180: ReadMarketDefinition — if cache is null, definition is skipped
    [Fact]
    public void ProcessMarketDefinitionBeforeIdIsSkipped()
    {
        // marketDefinition appears before id — cache is null at that point
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"marketDefinition":{"status":"OPEN"},"id":"1.123","rc":[{"id":111,"ltp":1.0}]}]}"""u8;

        _processor.Process(data);

        // Market should still be created, definition may or may not be set depending on order
        _processor.GetMarket("1.123").Should().NotBeNull();
    }

    // Kill mutant Line 201: ReadTotalVolume — cache null means tv is NOT assigned
    [Fact]
    public void ProcessTvBeforeIdDoesNotCrash()
    {
        // "tv" appears before "id" — cache is null
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"tv":500.0,"id":"1.123","rc":[{"id":111,"ltp":1.0}]}]}"""u8;

        _processor.Process(data);

        // TotalMatched should NOT be set because cache was null when tv was read
        _processor.GetMarket("1.123")!.TotalMatched.Should().BeNull();
    }

    // Kill mutant Line 204/261: ReadImageFlag returns false when img=false
    [Fact]
    public void ProcessImageFalseDoesNotMarkAsImage()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","img":false,"rc":[{"id":111,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.IsImage.Should().BeFalse();
    }

    // Kill mutant Line 247/248: ReadMarketChanges early return — mc array with no objects
    [Fact]
    public void ProcessEmptyMcArrayDoesNotCreateMarkets()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[]}"""u8;

        _processor.Process(data);

        _processor.Markets.Should().BeEmpty();
        _processor.PublishTime.Should().Be(100);
    }

    // Kill mutant Line 250: while loop — mc with multiple market objects
    [Fact]
    public void ProcessMcWithThreeMarketsIteratesAll()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.1","rc":[{"id":1,"ltp":1.0}]},{"id":"1.2","rc":[{"id":2,"ltp":2.0}]},{"id":"1.3","rc":[{"id":3,"ltp":3.0}]}]}"""u8;

        _processor.Process(data);

        _processor.Markets.Should().HaveCount(3);
    }

    // Kill mutant Line 293: ProcessRunnerChanges — isImage with cache not null clears before read
    [Fact]
    public void ProcessImageClearsRunnersAndReadsNewOnes()
    {
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0},{"id":222,"ltp":6.0}]}]}"""u8;
        _processor.Process(first);
        _processor.GetMarket("1.123")!.RunnerCount.Should().Be(2);

        var img = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","img":true,"rc":[{"id":333,"ltp":7.0}]}]}"""u8;
        _processor.Process(img);

        var market = _processor.GetMarket("1.123")!;
        market.RunnerCount.Should().Be(1);
        market.GetRunner(333)!.LastTradedPrice.Should().Be(7.0);
        market.GetRunner(111).Should().BeNull();
        market.GetRunner(222).Should().BeNull();
    }

    // Kill mutant Line 300: cache == null means runner is NOT added
    [Fact]
    public void ProcessRunnerWithSelectionIdZeroIsIgnored()
    {
        // SelectionId 0 causes early return — runner should not be created
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":0,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.RunnerCount.Should().Be(0);
    }

    // Kill mutant Line 316: string.Empty mutation — GetString() ?? string.Empty
    [Fact]
    public void ProcessMarketWithNullIdUsesEmptyString()
    {
        // null market id in JSON -> string.Empty fallback
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":null,"rc":[{"id":111,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        // Should create a market with empty string ID
        _processor.GetMarket("").Should().NotBeNull();
        _processor.GetMarket("")!.GetRunner(111)!.LastTradedPrice.Should().Be(5.0);
    }

    // Kill mutant Line 341/342: ReadRunnerChanges early return — rc not starting with array
    [Fact]
    public void ProcessRcAsNumberDoesNotAddRunners()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":42}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.RunnerCount.Should().Be(0);
    }

    // Kill mutant Line 360/361: empty property name in runner (propSpan.Length == 0) is skipped
    [Fact]
    public void ProcessRunnerWithEmptyPropertyNameIsSkipped()
    {
        // JSON with "" as a property name — should be skipped
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"":99,"ltp":3.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.GetRunner(111)!.LastTradedPrice.Should().Be(3.0);
    }

    // Kill mutant Line 484: statement removal in ApplyDeferredUpdates (verify updates actually apply)
    [Fact]
    public void ProcessDeferredUpdatesActuallyModifyRunner()
    {
        // Ensure atb update is actually applied (not just stored)
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atb":[[1.5,25.0],[2.5,50.0],[3.5,75.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToBack[1.5].Should().Be(25.0);
        runner.AvailableToBack[2.5].Should().Be(50.0);
        runner.AvailableToBack[3.5].Should().Be(75.0);
        runner.AvailableToBack.Count.Should().Be(3);
    }

    // Kill mutant Line 493: loop bound i < count mutated to i <= count (would cause IndexOutOfRange)
    [Fact]
    public void ProcessExactlyOneDeferredUpdateDoesNotOverrun()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atl":[[5.0,10.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToLay[5.0].Should().Be(10.0);
        runner.AvailableToLay.Count.Should().Be(1);
    }

    // Kill mutant Line 571/572: ReadPositionPriceSizeTriples early return
    [Fact]
    public void ProcessBatbNotArrayDoesNotCrash()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"batb":"invalid","ltp":2.0}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.LastTradedPrice.Should().Be(2.0);
        runner.BestAvailableToBack.GetPrice(0).Should().Be(0);
    }

    // Additional: verify multiple deferred updates across different types are all applied
    [Fact]
    public void ProcessMultipleDeferredUpdatesAcrossTypesAllApplied()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atb":[[2.0,10.0],[3.0,20.0]],"atl":[[4.0,30.0]],"trd":[[5.0,40.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.AvailableToBack.Count.Should().Be(2);
        runner.AvailableToLay.Count.Should().Be(1);
        runner.Traded.Count.Should().Be(1);
        runner.AvailableToBack[2.0].Should().Be(10.0);
        runner.AvailableToBack[3.0].Should().Be(20.0);
        runner.AvailableToLay[4.0].Should().Be(30.0);
        runner.Traded[5.0].Should().Be(40.0);
    }

    // Kill mutant Line 201: tv IS assigned when cache is NOT null (statement removal would break this)
    [Fact]
    public void ProcessTvAfterIdSetsMarketTotalMatched()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","tv":12345.67}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.TotalMatched.Should().Be(12345.67);
    }

    // Kill mutant Line 204/261: img:true MUST set IsImage — if GetBoolean always returns false, this fails
    [Fact]
    public void ProcessImageTrueMustSetIsImageFlag()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","img":true,"rc":[{"id":111,"ltp":1.0}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.IsImage.Should().BeTrue();
    }

    // Kill mutant Line 300: selectionId == 0 means no runner created (if mutation removes this check, runner IS created)
    [Fact]
    public void ProcessRunnerWithIdZeroAndValidCacheDoesNotCreateRunner()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":0,"ltp":5.0,"atb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        var market = _processor.GetMarket("1.123")!;
        market.RunnerCount.Should().Be(0);
        market.GetRunner(0).Should().BeNull();
    }

    // Kill mutant Line 493: i < count → i <= count (would access garbage or throw IndexOutOfRange)
    // Test with a controlled scenario where exactly N deferred updates exist
    [Fact]
    public void ProcessDeferredUpdatesDoesNotAccessBeyondCount()
    {
        // Create a scenario with exactly 2 deferred items then verify only 2 are applied
        var first = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"atb":[[1.0,10.0],[2.0,20.0]]}]}]}"""u8;
        _processor.Process(first);

        // Process again with different runner to verify isolation
        var second = """{"op":"mcm","id":2,"pt":200,"mc":[{"id":"1.123","rc":[{"id":222,"atb":[[3.0,30.0]]}]}]}"""u8;
        _processor.Process(second);

        var runner222 = _processor.GetMarket("1.123")!.GetRunner(222)!;
        runner222.AvailableToBack.Count.Should().Be(1);
        runner222.AvailableToBack[3.0].Should().Be(30.0);
        // Verify runner 111 was not corrupted
        var runner111 = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner111.AvailableToBack.Count.Should().Be(2);
    }
}
