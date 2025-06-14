using Betfair.Api.Requests.Orders.Queries;
using Betfair.Core.Enums;

namespace Betfair.Tests.Api.Requests.Orders.Queries;

public class ClearedOrdersQueryTests
{
    [Fact]
    public void ConstructorSetsDefaultValues()
    {
        var query = new ClearedOrdersQuery();

        query.BetStatus.Should().Be(BetStatus.Settled);
        query.EventTypeIds.Should().BeNull();
        query.EventIds.Should().BeNull();
        query.MarketIds.Should().BeNull();
        query.RunnerIds.Should().BeNull();
        query.BetIds.Should().BeNull();
        query.CustomerOrderRefs.Should().BeNull();
        query.CustomerStrategyRefs.Should().BeNull();
        query.Side.Should().BeNull();
        query.SettledDateRange.Should().BeNull();
        query.GroupByOption.Should().BeNull();
        query.IncludeItemDescription.Should().BeFalse();
        query.Locale.Should().BeNull();
        query.FromRecord.Should().Be(0);
        query.RecordCount.Should().Be(1000);
    }

    [Fact]
    public void WithBetStatusSetsBetStatus()
    {
        var query = new ClearedOrdersQuery();

        var result = query.WithBetStatus(BetStatus.Cancelled);

        result.Should().BeSameAs(query);
        query.BetStatus.Should().Be(BetStatus.Cancelled);
    }

    [Fact]
    public void WithEventTypesAddsEventTypeIds()
    {
        var query = new ClearedOrdersQuery();

        var result = query.WithEventTypes("1", "2", "3");

        result.Should().BeSameAs(query);
        query.EventTypeIds.Should().NotBeNull();
        query.EventTypeIds.Should().HaveCount(3);
        query.EventTypeIds.Should().Contain("1");
        query.EventTypeIds.Should().Contain("2");
        query.EventTypeIds.Should().Contain("3");
    }

    [Fact]
    public void WithEventTypesIgnoresNullAndEmptyValues()
    {
        var query = new ClearedOrdersQuery();

        var result = query.WithEventTypes("1", null!, string.Empty, "  ", "2");

        result.Should().BeSameAs(query);
        query.EventTypeIds.Should().NotBeNull();
        query.EventTypeIds.Should().HaveCount(2);
        query.EventTypeIds.Should().Contain("1");
        query.EventTypeIds.Should().Contain("2");
    }

    [Fact]
    public void WithEventsAddsEventIds()
    {
        var query = new ClearedOrdersQuery();

        var result = query.WithEvents("event1", "event2");

        result.Should().BeSameAs(query);
        query.EventIds.Should().NotBeNull();
        query.EventIds.Should().HaveCount(2);
        query.EventIds.Should().Contain("event1");
        query.EventIds.Should().Contain("event2");
    }

    [Fact]
    public void WithMarketsAddsMarketIds()
    {
        var query = new ClearedOrdersQuery();

        var result = query.WithMarkets("1.123", "1.456");

        result.Should().BeSameAs(query);
        query.MarketIds.Should().NotBeNull();
        query.MarketIds.Should().HaveCount(2);
        query.MarketIds.Should().Contain("1.123");
        query.MarketIds.Should().Contain("1.456");
    }

    [Fact]
    public void WithRunnersAddsRunnerIds()
    {
        var query = new ClearedOrdersQuery();

        var result = query.WithRunners(123L, 456L);

        result.Should().BeSameAs(query);
        query.RunnerIds.Should().NotBeNull();
        query.RunnerIds.Should().HaveCount(2);
        query.RunnerIds.Should().Contain(123L);
        query.RunnerIds.Should().Contain(456L);
    }

    [Fact]
    public void WithBetsAddsBetIds()
    {
        var query = new ClearedOrdersQuery();

        var result = query.WithBets("bet1", "bet2");

        result.Should().BeSameAs(query);
        query.BetIds.Should().NotBeNull();
        query.BetIds.Should().HaveCount(2);
        query.BetIds.Should().Contain("bet1");
        query.BetIds.Should().Contain("bet2");
    }

    [Fact]
    public void WithSideSetsSide()
    {
        var query = new ClearedOrdersQuery();

        var result = query.WithSide(Side.Back);

        result.Should().BeSameAs(query);
        query.Side.Should().Be(Side.Back);
    }

    [Fact]
    public void WithSettledDateRangeSetsDateRange()
    {
        var query = new ClearedOrdersQuery();
        var from = new DateTimeOffset(2023, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2023, 1, 2, 15, 30, 0, TimeSpan.Zero);

        var result = query.WithSettledDateRange(from, to);

        result.Should().BeSameAs(query);
        query.SettledDateRange.Should().NotBeNull();
        query.SettledDateRange!.From.Should().Be("2023-01-01T10:00:00Z");
        query.SettledDateRange.To.Should().Be("2023-01-02T15:30:00Z");
    }

    [Fact]
    public void GroupBySetsGroupByOption()
    {
        var query = new ClearedOrdersQuery();

        var result = query.GroupBy(GroupBy.Market);

        result.Should().BeSameAs(query);
        query.GroupByOption.Should().Be(GroupBy.Market);
    }

    [Fact]
    public void IncludeItemDescriptionsEnablesIncludeItemDescription()
    {
        var query = new ClearedOrdersQuery();

        var result = query.IncludeItemDescriptions();

        result.Should().BeSameAs(query);
        query.IncludeItemDescription.Should().BeTrue();
    }

    [Fact]
    public void WithLocaleSetsLocale()
    {
        var query = new ClearedOrdersQuery();

        var result = query.WithLocale("en-GB");

        result.Should().BeSameAs(query);
        query.Locale.Should().Be("en-GB");
    }

    [Fact]
    public void FromSetsFromRecord()
    {
        var query = new ClearedOrdersQuery();

        var result = query.From(50);

        result.Should().BeSameAs(query);
        query.FromRecord.Should().Be(50);
    }

    [Fact]
    public void FromWithNegativeValueSetsToZero()
    {
        var query = new ClearedOrdersQuery();

        var result = query.From(-10);

        result.Should().BeSameAs(query);
        query.FromRecord.Should().Be(0);
    }

    [Fact]
    public void TakeSetsRecordCount()
    {
        var query = new ClearedOrdersQuery();

        var result = query.Take(500);

        result.Should().BeSameAs(query);
        query.RecordCount.Should().Be(500);
    }

    [Fact]
    public void TakeWithValueAbove1000SetsTo1000()
    {
        var query = new ClearedOrdersQuery();

        var result = query.Take(1500);

        result.Should().BeSameAs(query);
        query.RecordCount.Should().Be(1000);
    }

    [Fact]
    public void TakeWithValueBelow1SetsTo1()
    {
        var query = new ClearedOrdersQuery();

        var result = query.Take(0);

        result.Should().BeSameAs(query);
        query.RecordCount.Should().Be(1);
    }

    [Fact]
    public void BackBetsOnlySetsSideToBack()
    {
        var query = new ClearedOrdersQuery();

        var result = query.BackBetsOnly();

        result.Should().BeSameAs(query);
        query.Side.Should().Be(Side.Back);
    }

    [Fact]
    public void LayBetsOnlySetsSideToLay()
    {
        var query = new ClearedOrdersQuery();

        var result = query.LayBetsOnly();

        result.Should().BeSameAs(query);
        query.Side.Should().Be(Side.Lay);
    }

    [Fact]
    public void SettledOnlySetsBetStatusToSettled()
    {
        var query = new ClearedOrdersQuery();

        var result = query.SettledOnly();

        result.Should().BeSameAs(query);
        query.BetStatus.Should().Be(BetStatus.Settled);
    }

    [Fact]
    public void CancelledOnlySetsBetStatusToCancelled()
    {
        var query = new ClearedOrdersQuery();

        var result = query.CancelledOnly();

        result.Should().BeSameAs(query);
        query.BetStatus.Should().Be(BetStatus.Cancelled);
    }

    [Fact]
    public void FluentChainingWorksCorrectly()
    {
        var from = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2023, 1, 31, 23, 59, 59, TimeSpan.Zero);

        var query = new ClearedOrdersQuery()
            .WithBetStatus(BetStatus.Settled)
            .WithMarkets("1.123", "1.456")
            .WithSettledDateRange(from, to)
            .BackBetsOnly()
            .GroupBy(GroupBy.Market)
            .IncludeItemDescriptions()
            .From(10)
            .Take(100);

        query.BetStatus.Should().Be(BetStatus.Settled);
        query.MarketIds.Should().HaveCount(2);
        query.Side.Should().Be(Side.Back);
        query.GroupByOption.Should().Be(GroupBy.Market);
        query.IncludeItemDescription.Should().BeTrue();
        query.FromRecord.Should().Be(10);
        query.RecordCount.Should().Be(100);
        query.SettledDateRange.Should().NotBeNull();
    }
}
