#pragma warning disable SA1009 // StyleCop false positive with C# 11 UTF-8 string literals (u8 suffix)

using Betfair.Core.Enums;
using Betfair.Stream.OrderCache;

namespace Betfair.Tests.Stream.OrderCache;

public class OrderCacheProcessorTests
{
    private readonly OrderCacheProcessor _processor = new ();

    [Fact]
    public void ProcessEmptyObjectDoesNotThrow()
    {
        _processor.Process("{}"u8);

        _processor.Markets.Should().BeEmpty();
    }

    [Fact]
    public void ProcessNonOcmOperationIsIgnored()
    {
        var data = """{"op":"connection","connectionId":"abc"}"""u8;

        _processor.Process(data);

        _processor.Markets.Should().BeEmpty();
    }

    [Fact]
    public void ProcessMcmOperationIsIgnored()
    {
        var data = """{"op":"mcm","id":2,"pt":100,"mc":[{"id":"1.123","rc":[{"id":111,"ltp":5.0}]}]}"""u8;

        _processor.Process(data);

        _processor.Markets.Should().BeEmpty();
    }

    [Fact]
    public void ProcessExtractsPublishTime()
    {
        var data = """{"op":"ocm","id":2,"pt":1743511672980,"oc":[]}"""u8;

        _processor.Process(data);

        _processor.PublishTime.Should().Be(1743511672980);
    }

    [Fact]
    public void ProcessExtractsClockToken()
    {
        var data = """{"op":"ocm","id":2,"clk":"AAMABAAE","pt":100,"oc":[]}"""u8;

        _processor.Process(data);

        _processor.Clock.Should().Be("AAMABAAE");
    }

    [Fact]
    public void ProcessExtractsInitialClockToken()
    {
        var data = """{"op":"ocm","id":2,"initialClk":"ic123","clk":"c456","pt":100,"oc":[]}"""u8;

        _processor.Process(data);

        _processor.InitialClock.Should().Be("ic123");
        _processor.Clock.Should().Be("c456");
    }

    [Fact]
    public void ClockBytesReturnsUtf8EncodedClock()
    {
        var data = """{"op":"ocm","id":2,"clk":"AAMABAAE","pt":100,"oc":[]}"""u8;

        _processor.Process(data);

        _processor.ClockBytes.ToArray().Should().BeEquivalentTo(System.Text.Encoding.UTF8.GetBytes("AAMABAAE"));
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
    public void ProcessCreatesMarketFromOcArray()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123").Should().NotBeNull();
    }

    [Fact]
    public void ProcessSetsMarketPublishTime()
    {
        var data = """{"op":"ocm","id":2,"pt":9999,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.PublishTime.Should().Be(9999);
    }

    [Fact]
    public void GetMarketReturnsNullForUnknownMarket()
    {
        _processor.GetMarket("1.999").Should().BeNull();
    }

    [Fact]
    public void ProcessSetsAccountId()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","accountId":12345,"orc":[{"id":111,"mb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.AccountId.Should().Be(12345);
    }

    [Fact]
    public void ProcessSetsClosedFlag()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","closed":true,"orc":[{"id":111,"mb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.Closed.Should().BeTrue();
    }

    [Fact]
    public void ProcessPopulatesMatchedBacksLadder()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.5,100.0],[3.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.MatchedBacks[2.5].Should().Be(100.0);
        runner.MatchedBacks[3.0].Should().Be(50.0);
        runner.MatchedBacks.Count.Should().Be(2);
    }

    [Fact]
    public void ProcessPopulatesMatchedLaysLadder()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"ml":[[4.0,200.0],[5.0,75.0]]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.MatchedLays[4.0].Should().Be(200.0);
        runner.MatchedLays[5.0].Should().Be(75.0);
    }

    [Fact]
    public void ProcessAppliesDeltaToMatchedBacks()
    {
        var initial = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.5,100.0]]}]}]}"""u8;
        var delta = """{"op":"ocm","id":2,"pt":101,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.5,150.0],[3.0,50.0]]}]}]}"""u8;

        _processor.Process(initial);
        _processor.Process(delta);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.MatchedBacks[2.5].Should().Be(150.0);
        runner.MatchedBacks[3.0].Should().Be(50.0);
    }

    [Fact]
    public void ProcessRemovesLadderEntryOnZeroSize()
    {
        var initial = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.5,100.0],[3.0,50.0]]}]}]}"""u8;
        var removal = """{"op":"ocm","id":2,"pt":101,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.5,0]]}]}]}"""u8;

        _processor.Process(initial);
        _processor.Process(removal);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.MatchedBacks[2.5].Should().Be(0);
        runner.MatchedBacks.Count.Should().Be(1);
    }

    [Fact]
    public void ProcessPopulatesUnmatchedOrder()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","p":2.5,"s":10.0,"side":"B","status":"E","ot":"L","pt":"P","pd":1700000000000}]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        var order = runner.GetOrder("bet1")!;
        order.Price.Should().Be(2.5);
        order.Size.Should().Be(10.0);
        order.Side.Should().Be(Side.Back);
        order.Status.Should().Be(OrderStatus.Executable);
        order.OrderType.Should().Be(OrderType.Limit);
        order.PersistenceType.Should().Be(PersistenceType.Persist);
        order.PlacedDate.Should().Be(1700000000000);
    }

    [Fact]
    public void ProcessUpdatesExistingUnmatchedOrder()
    {
        var initial = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","p":2.5,"s":10.0,"side":"B","status":"E","sr":10.0}]}]}]}"""u8;
        var update = """{"op":"ocm","id":2,"pt":101,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","sm":5.0,"sr":5.0}]}]}]}"""u8;

        _processor.Process(initial);
        _processor.Process(update);

        var order = _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!;
        order.SizeMatched.Should().Be(5.0);
        order.SizeRemaining.Should().Be(5.0);
        order.Price.Should().Be(2.5);
        order.Side.Should().Be(Side.Back);
    }

    [Fact]
    public void ProcessPopulatesMultipleOrders()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","p":2.5,"s":10.0,"side":"B"},{"id":"bet2","p":3.0,"s":20.0,"side":"L"}]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.OrderCount.Should().Be(2);
        runner.GetOrder("bet1")!.Side.Should().Be(Side.Back);
        runner.GetOrder("bet2")!.Side.Should().Be(Side.Lay);
    }

    [Fact]
    public void ProcessPopulatesAllOrderSizeFields()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","sm":5.0,"sr":3.0,"sl":1.0,"sc":0.5,"sv":0.5}]}]}]}"""u8;

        _processor.Process(data);

        var order = _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!;
        order.SizeMatched.Should().Be(5.0);
        order.SizeRemaining.Should().Be(3.0);
        order.SizeLapsed.Should().Be(1.0);
        order.SizeCancelled.Should().Be(0.5);
        order.SizeVoided.Should().Be(0.5);
    }

    [Fact]
    public void ProcessPopulatesAllOrderDateFields()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","pd":1700000000000,"md":1700000001000,"cd":1700000002000,"ld":1700000003000}]}]}]}"""u8;

        _processor.Process(data);

        var order = _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!;
        order.PlacedDate.Should().Be(1700000000000);
        order.MatchedDate.Should().Be(1700000001000);
        order.CancelledDate.Should().Be(1700000002000);
        order.LapsedDate.Should().Be(1700000003000);
    }

    [Fact]
    public void ProcessPopulatesOrderStringFields()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","rac":"AUTH123","rc":"REG1","rfo":"orderRef1","rfs":"stratRef1","lsrc":"REASON1"}]}]}]}"""u8;

        _processor.Process(data);

        var order = _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!;
        order.RegulatorAuthCode.Should().Be("AUTH123");
        order.RegulatorCode.Should().Be("REG1");
        order.OrderReference.Should().Be("orderRef1");
        order.StrategyReference.Should().Be("stratRef1");
        order.LapsedStatusReasonCode.Should().Be("REASON1");
    }

    [Fact]
    public void ProcessPopulatesAveragePriceMatched()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","avp":2.75}]}]}]}"""u8;

        _processor.Process(data);

        var order = _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!;
        order.AveragePriceMatched.Should().Be(2.75);
    }

    [Fact]
    public void ProcessPopulatesBspLiability()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","bsp":100.5}]}]}]}"""u8;

        _processor.Process(data);

        var order = _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!;
        order.BspLiability.Should().Be(100.5);
    }

    [Fact]
    public void FullImageClearsRunnerState()
    {
        var initial = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.5,100.0]],"uo":[{"id":"bet1","p":2.5,"s":10.0}]}]}]}"""u8;
        var image = """{"op":"ocm","id":2,"pt":101,"oc":[{"id":"1.123","orc":[{"id":111,"fullImage":true,"mb":[[3.0,50.0]],"uo":[{"id":"bet2","p":3.0,"s":20.0}]}]}]}"""u8;

        _processor.Process(initial);
        _processor.Process(image);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.MatchedBacks[2.5].Should().Be(0);
        runner.MatchedBacks[3.0].Should().Be(50.0);
        runner.GetOrder("bet1").Should().BeNull();
        runner.GetOrder("bet2").Should().NotBeNull();
    }

    [Fact]
    public void ProcessHandlesMultipleRunners()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.0,50.0]]},{"id":222,"ml":[[3.0,75.0]]}]}]}"""u8;

        _processor.Process(data);

        var market = _processor.GetMarket("1.123")!;
        market.RunnerCount.Should().Be(2);
        market.GetRunner(111)!.MatchedBacks[2.0].Should().Be(50.0);
        market.GetRunner(222)!.MatchedLays[3.0].Should().Be(75.0);
    }

    [Fact]
    public void ProcessHandlesMultipleMarkets()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.111","orc":[{"id":1,"mb":[[2.0,10.0]]}]},{"id":"1.222","orc":[{"id":2,"mb":[[3.0,20.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.111")!.GetRunner(1)!.MatchedBacks[2.0].Should().Be(10.0);
        _processor.GetMarket("1.222")!.GetRunner(2)!.MatchedBacks[3.0].Should().Be(20.0);
    }

    [Fact]
    public void ProcessHandlesStrategyMatchChanges()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"smc":{"myStrat":{"mb":[[2.0,50.0],[3.0,25.0]],"ml":[[4.0,30.0]]}}}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        var strat = runner.StrategyMatches["myStrat"];
        strat.MatchedBacks[2.0].Should().Be(50.0);
        strat.MatchedBacks[3.0].Should().Be(25.0);
        strat.MatchedLays[4.0].Should().Be(30.0);
    }

    [Fact]
    public void ProcessHandlesMultipleStrategies()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"smc":{"stratA":{"mb":[[2.0,10.0]]},"stratB":{"ml":[[3.0,20.0]]}}}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StrategyMatches["stratA"].MatchedBacks[2.0].Should().Be(10.0);
        runner.StrategyMatches["stratB"].MatchedLays[3.0].Should().Be(20.0);
    }

    [Fact]
    public void ProcessAppliesDeltaToStrategyMatchChanges()
    {
        var initial = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"smc":{"myStrat":{"mb":[[2.0,50.0]]}}}]}]}"""u8;
        var delta = """{"op":"ocm","id":2,"pt":101,"oc":[{"id":"1.123","orc":[{"id":111,"smc":{"myStrat":{"mb":[[2.0,75.0],[3.0,25.0]]}}}]}]}"""u8;

        _processor.Process(initial);
        _processor.Process(delta);

        var strat = _processor.GetMarket("1.123")!.GetRunner(111)!.StrategyMatches["myStrat"];
        strat.MatchedBacks[2.0].Should().Be(75.0);
        strat.MatchedBacks[3.0].Should().Be(25.0);
    }

    [Fact]
    public void FullImageClearsStrategyMatchChanges()
    {
        var initial = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"smc":{"oldStrat":{"mb":[[2.0,50.0]]}}}]}]}"""u8;
        var image = """{"op":"ocm","id":2,"pt":101,"oc":[{"id":"1.123","orc":[{"id":111,"fullImage":true,"smc":{"newStrat":{"mb":[[3.0,25.0]]}}}]}]}"""u8;

        _processor.Process(initial);
        _processor.Process(image);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.StrategyMatches.Should().NotContainKey("oldStrat");
        runner.StrategyMatches["newStrat"].MatchedBacks[3.0].Should().Be(25.0);
    }

    [Fact]
    public void ProcessSetsRunnerHandicap()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"hc":1.5,"id":111,"mb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.GetRunner(111)!.Handicap.Should().Be(1.5);
    }

    [Fact]
    public void ProcessSetsLastProcessingTime()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.LastProcessingTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public void ProcessSkipsUnknownTopLevelProperties()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"unknownField":"value","oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.GetRunner(111)!.MatchedBacks[2.0].Should().Be(50.0);
    }

    [Fact]
    public void ProcessSkipsUnknownOrderChangeProperties()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","unknownField":true,"orc":[{"id":111,"mb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.GetRunner(111)!.MatchedBacks[2.0].Should().Be(50.0);
    }

    [Fact]
    public void ProcessSkipsUnknownRunnerProperties()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"unknownField":[1,2,3],"mb":[[2.0,50.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.GetRunner(111)!.MatchedBacks[2.0].Should().Be(50.0);
    }

    [Fact]
    public void ProcessSkipsUnknownOrderProperties()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","unknownProp":"xyz","p":2.5}]}]}]}"""u8;

        _processor.Process(data);

        _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!.Price.Should().Be(2.5);
    }

    [Fact]
    public void RunnerSpanAllowsZeroAllocationIteration()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.0,50.0]]},{"id":222,"mb":[[3.0,25.0]]}]}]}"""u8;

        _processor.Process(data);

        var market = _processor.GetMarket("1.123")!;
        var span = market.RunnerSpan;
        span.Length.Should().Be(2);
        span[0].SelectionId.Should().Be(111);
        span[1].SelectionId.Should().Be(222);
    }

    [Fact]
    public void MarketsPropertyReturnsAllMarkets()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.111","orc":[{"id":1,"mb":[[2.0,10.0]]}]},{"id":"1.222","orc":[{"id":2,"mb":[[3.0,20.0]]}]}]}"""u8;

        _processor.Process(data);

        _processor.Markets.Should().HaveCount(2);
        _processor.Markets.Should().ContainKey("1.111");
        _processor.Markets.Should().ContainKey("1.222");
    }

    [Fact]
    public void SingleMarketFastPathReturnsSameInstance()
    {
        var data1 = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.0,50.0]]}]}]}"""u8;
        var data2 = """{"op":"ocm","id":2,"pt":101,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.0,75.0]]}]}]}"""u8;

        _processor.Process(data1);
        var first = _processor.GetMarket("1.123");
        _processor.Process(data2);
        var second = _processor.GetMarket("1.123");

        first.Should().BeSameAs(second);
    }

    [Fact]
    public void ProcessInvalidJsonDoesNotThrow()
    {
        // A JSON value that is not an object — reader.Read() returns true but TokenType != StartObject
        byte[] data = "[]"u8.ToArray();

        var act = () => _processor.Process(data);

        act.Should().NotThrow();
    }

    [Fact]
    public void ProcessEmptyArrayDoesNotThrow()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[]}"""u8;

        _processor.Process(data);

        _processor.Markets.Should().BeEmpty();
    }

    [Fact]
    public void ProcessCombinedAggregatedAndDetailedPositions()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.5,100.0]],"ml":[[3.0,50.0]],"uo":[{"id":"bet1","p":2.5,"s":10.0,"side":"B","status":"E"}]}]}]}"""u8;

        _processor.Process(data);

        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.MatchedBacks[2.5].Should().Be(100.0);
        runner.MatchedLays[3.0].Should().Be(50.0);
        runner.GetOrder("bet1")!.Price.Should().Be(2.5);
    }

    [Fact]
    public void ClockUpdatesAcrossMultipleMessages()
    {
        var data1 = """{"op":"ocm","id":2,"clk":"clock1","pt":100,"oc":[]}"""u8;
        var data2 = """{"op":"ocm","id":2,"clk":"clock2","pt":101,"oc":[]}"""u8;

        _processor.Process(data1);
        _processor.Clock.Should().Be("clock1");

        _processor.Process(data2);
        _processor.Clock.Should().Be("clock2");
    }

    [Fact]
    public void InitialClockBytesReturnsUtf8EncodedInitialClock()
    {
        var data = """{"op":"ocm","id":2,"initialClk":"INIT123","pt":100,"oc":[]}"""u8;
        _processor.Process(data);
        _processor.InitialClockBytes.ToArray().Should().BeEquivalentTo(System.Text.Encoding.UTF8.GetBytes("INIT123"));
    }

    [Fact]
    public void ProcessParsesOrderSideLay()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","side":"L"}]}]}]}"""u8;
        _processor.Process(data);
        _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!.Side.Should().Be(Side.Lay);
    }

    [Fact]
    public void ProcessParsesOrderStatusExecutionComplete()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","status":"EC"}]}]}]}"""u8;
        _processor.Process(data);
        _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!.Status.Should().Be(OrderStatus.ExecutionComplete);
    }

    [Fact]
    public void ProcessParsesPersistenceTypeLapse()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","pt":"L"}]}]}]}"""u8;
        _processor.Process(data);
        _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!.PersistenceType.Should().Be(PersistenceType.Lapse);
    }

    [Fact]
    public void ProcessParsesPersistenceTypeMarketOnClose()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","pt":"MOC"}]}]}]}"""u8;
        _processor.Process(data);
        _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!.PersistenceType.Should().Be(PersistenceType.MarketOnClose);
    }

    [Fact]
    public void ProcessParsesOrderTypeLimitOnClose()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","ot":"LOC"}]}]}]}"""u8;
        _processor.Process(data);
        _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!.OrderType.Should().Be(OrderType.LimitOnClose);
    }

    [Fact]
    public void ProcessParsesOrderTypeMarketOnClose()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","ot":"MOC"}]}]}]}"""u8;
        _processor.Process(data);
        _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!.OrderType.Should().Be(OrderType.MarketOnClose);
    }

    [Fact]
    public void ProcessUnknownSideReturnsUnknown()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","side":"X"}]}]}]}"""u8;
        _processor.Process(data);
        _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!.Side.Should().Be(Side.Unknown);
    }

    [Fact]
    public void ProcessUnknownStatusReturnsUnknown()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","status":"ZZ"}]}]}]}"""u8;
        _processor.Process(data);
        _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!.Status.Should().Be(OrderStatus.Unknown);
    }

    [Fact]
    public void ProcessFullImageBeforeIdClearsRunner()
    {
        var initial = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"mb":[[2.0,50.0]],"uo":[{"id":"bet1","p":5.0}]}]}]}"""u8;
        var image = """{"op":"ocm","id":2,"pt":101,"oc":[{"id":"1.123","orc":[{"fullImage":true,"id":111,"mb":[[3.0,25.0]]}]}]}"""u8;
        _processor.Process(initial);
        _processor.Process(image);
        var runner = _processor.GetMarket("1.123")!.GetRunner(111)!;
        runner.MatchedBacks[2.0].Should().Be(0);
        runner.MatchedBacks[3.0].Should().Be(25.0);
        runner.GetOrder("bet1").Should().BeNull();
    }

    [Fact]
    public void ProcessStringFieldsUnchangedDoesNotReallocate()
    {
        var data1 = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","rfo":"myRef","rfs":"myStrat"}]}]}]}"""u8;
        var data2 = """{"op":"ocm","id":2,"pt":101,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[{"id":"bet1","rfo":"myRef","rfs":"myStrat"}]}]}]}"""u8;
        _processor.Process(data1);
        var order = _processor.GetMarket("1.123")!.GetRunner(111)!.GetOrder("bet1")!;
        var ref1 = order.OrderReference;
        _processor.Process(data2);
        var ref2 = order.OrderReference;

        // Same string instance because bytes didn't change
        ReferenceEquals(ref1, ref2).Should().BeTrue();
    }

    [Fact]
    public void ProcessEmptyUoArrayDoesNotThrow()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"uo":[]}]}]}"""u8;
        _processor.Process(data);
        _processor.GetMarket("1.123")!.GetRunner(111)!.OrderCount.Should().Be(0);
    }

    [Fact]
    public void ProcessEmptySmcObjectDoesNotThrow()
    {
        var data = """{"op":"ocm","id":2,"pt":100,"oc":[{"id":"1.123","orc":[{"id":111,"smc":{}}]}]}"""u8;
        _processor.Process(data);
        _processor.GetMarket("1.123")!.GetRunner(111)!.StrategyMatches.Should().BeEmpty();
    }
}
