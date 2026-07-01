using Betfair.Api.Betting.Endpoints.ListClearedOrders.Enums;
using Betfair.Api.Betting.Endpoints.ListClearedOrders.Requests;
using Betfair.Api.Betting.Enums;
using Betfair.Core.Enums;

namespace Betfair.Tests.Api;

public class ClearedOrdersQueryTests
{
    private readonly ClearedOrdersQuery _query = new ();

    [Fact]
    public void DefaultBetStatusIsSettled()
    {
        _query.BetStatus.Should().Be(BetStatus.Settled);
    }

    [Fact]
    public void DefaultRecordCountIs1000()
    {
        _query.RecordCount.Should().Be(1000);
    }

    [Fact]
    public void WithEventTypesAddsEventTypeIds()
    {
        _query.WithEventTypes("7", "1");

        _query.EventTypeIds.Should().Contain("7");
        _query.EventTypeIds.Should().Contain("1");
    }

    [Fact]
    public void WithEventTypesNullHasNoEffect()
    {
        _query.WithEventTypes(null!);

        _query.EventTypeIds.Should().BeNull();
    }

    [Fact]
    public void WithEventTypesEmptyHasNoEffect()
    {
        _query.WithEventTypes();

        _query.EventTypeIds.Should().BeNull();
    }

    [Fact]
    public void WithEventTypesFiltersWhitespace()
    {
        _query.WithEventTypes("7", string.Empty, "  ");

        _query.EventTypeIds.Should().ContainSingle().Which.Should().Be("7");
    }

    [Fact]
    public void WithEventsAddsEventIds()
    {
        _query.WithEvents("E1", "E2");

        _query.EventIds.Should().Contain("E1");
        _query.EventIds.Should().Contain("E2");
    }

    [Fact]
    public void WithEventsNullHasNoEffect()
    {
        _query.WithEvents(null!);

        _query.EventIds.Should().BeNull();
    }

    [Fact]
    public void WithMarketsAddsMarketIds()
    {
        _query.WithMarkets("1.123", "1.456");

        _query.MarketIds.Should().Contain("1.123");
        _query.MarketIds.Should().Contain("1.456");
    }

    [Fact]
    public void WithMarketsNullHasNoEffect()
    {
        _query.WithMarkets(null!);

        _query.MarketIds.Should().BeNull();
    }

    [Fact]
    public void WithRunnersAddsRunnerIds()
    {
        _query.WithRunners(12345, 67890);

        _query.RunnerIds.Should().Contain(12345);
        _query.RunnerIds.Should().Contain(67890);
    }

    [Fact]
    public void WithRunnersNullHasNoEffect()
    {
        _query.WithRunners(null!);

        _query.RunnerIds.Should().BeNull();
    }

    [Fact]
    public void WithBetsAddsBetIds()
    {
        _query.WithBets("B1", "B2");

        _query.BetIds.Should().Contain("B1");
        _query.BetIds.Should().Contain("B2");
    }

    [Fact]
    public void WithBetsNullHasNoEffect()
    {
        _query.WithBets(null!);

        _query.BetIds.Should().BeNull();
    }

    [Fact]
    public void WithCustomerOrderRefsAddsRefs()
    {
        _query.WithCustomerOrderRefs("ref1", "ref2");

        _query.CustomerOrderRefs.Should().Contain("ref1");
        _query.CustomerOrderRefs.Should().Contain("ref2");
    }

    [Fact]
    public void WithCustomerOrderRefsNullHasNoEffect()
    {
        _query.WithCustomerOrderRefs(null!);

        _query.CustomerOrderRefs.Should().BeNull();
    }

    [Fact]
    public void WithCustomerStrategyRefsAddsRefs()
    {
        _query.WithCustomerStrategyRefs("strat1", "strat2");

        _query.CustomerStrategyRefs.Should().Contain("strat1");
        _query.CustomerStrategyRefs.Should().Contain("strat2");
    }

    [Fact]
    public void WithCustomerStrategyRefsNullHasNoEffect()
    {
        _query.WithCustomerStrategyRefs(null!);

        _query.CustomerStrategyRefs.Should().BeNull();
    }

    [Fact]
    public void WithSideSetsSide()
    {
        _query.WithSide(Side.Back);

        _query.Side.Should().Be(Side.Back);
    }

    [Fact]
    public void WithSettledDateRangeSetsRange()
    {
        var from = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero);

        _query.WithSettledDateRange(from, to);

        _query.SettledDateRange.Should().NotBeNull();
    }

    [Fact]
    public void GroupBySetsGroupByOption()
    {
        _query.GroupBy(Betfair.Api.Betting.Endpoints.ListClearedOrders.Enums.GroupBy.Market);

        _query.GroupByOption.Should().Be(Betfair.Api.Betting.Endpoints.ListClearedOrders.Enums.GroupBy.Market);
    }

    [Fact]
    public void IncludeItemDescriptionsSetsFlag()
    {
        _query.IncludeItemDescriptions();

        _query.IncludeItemDescription.Should().BeTrue();
    }

    [Fact]
    public void WithLocaleSetsLocale()
    {
        _query.WithLocale("en");

        _query.Locale.Should().Be("en");
    }

    [Fact]
    public void FromSetsFromRecord()
    {
        _query.From(50);

        _query.FromRecord.Should().Be(50);
    }

    [Fact]
    public void FromClampsNegativeToZero()
    {
        _query.From(-5);

        _query.FromRecord.Should().Be(0);
    }

    [Fact]
    public void TakeSetsRecordCount()
    {
        _query.Take(500);

        _query.RecordCount.Should().Be(500);
    }

    [Fact]
    public void TakeClampsToMinimumOne()
    {
        _query.Take(0);

        _query.RecordCount.Should().Be(1);
    }

    [Fact]
    public void TakeClampsToMaximum1000()
    {
        _query.Take(5000);

        _query.RecordCount.Should().Be(1000);
    }

    [Fact]
    public void BackBetsOnlySetsSideToBack()
    {
        _query.BackBetsOnly();

        _query.Side.Should().Be(Side.Back);
    }

    [Fact]
    public void LayBetsOnlySetsSideToLay()
    {
        _query.LayBetsOnly();

        _query.Side.Should().Be(Side.Lay);
    }

    [Fact]
    public void CancelledOnlySetsBetStatusToCancelled()
    {
        _query.CancelledOnly();

        _query.BetStatus.Should().Be(BetStatus.Cancelled);
    }

    [Fact]
    public void LastWeekSetsSettledDateRange()
    {
        _query.LastWeek();

        _query.SettledDateRange.Should().NotBeNull();
    }

    [Fact]
    public void LastMonthSetsSettledDateRange()
    {
        _query.LastMonth();

        _query.SettledDateRange.Should().NotBeNull();
    }

    [Fact]
    public void TodaySetsSettledDateRange()
    {
        _query.Today();

        _query.SettledDateRange.Should().NotBeNull();
    }

    [Fact]
    public void YesterdaySetsSettledDateRange()
    {
        _query.Yesterday();

        _query.SettledDateRange.Should().NotBeNull();
    }
}
