using Betfair.Api.Betting.Endpoints.ListCurrentOrders.Responses;
using Betfair.Api.Betting.Endpoints.ListCurrentOrders.Enums;
using Betfair.Api.Betting.Enums;

namespace Betfair.Tests.Api.Requests.Orders;

public class ApiOrderFilterTests
{
    [Fact]
    public void ConstructorSetsDefaultValues()
    {
        var filter = new ApiOrderFilter();

        filter.Should().NotBeNull();
        filter.BetIds.Should().BeNull();
        filter.MarketIds.Should().BeNull();
        filter.OrderProjection.Should().BeNull();
        filter.CustomerOrderRefs.Should().BeNull();
        filter.CustomerStrategyRefs.Should().BeNull();
        filter.DateRange.Should().BeNull();
        filter.OrderByValue.Should().BeNull();
        filter.SortDir.Should().BeNull();
        filter.FromRecord.Should().Be(0);
        filter.RecordCount.Should().Be(1000);
    }

    [Fact]
    public void WithBetIdsWhenInputIsNullReturnsThis()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithBetIds(null!);

        result.Should().BeSameAs(filter);
        filter.BetIds.Should().BeNull();
    }

    [Fact]
    public void WithBetIdsWhenInputIsEmptyCreatesEmptyHashSet()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithBetIds();

        result.Should().BeSameAs(filter);
        filter.BetIds.Should().NotBeNull();
        filter.BetIds.Should().BeEmpty();
    }

    [Fact]
    public void WithBetIdsWhenInputContainsNullsFiltersOutNulls()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithBetIds("bet1", null!, "bet2", null!);

        result.Should().BeSameAs(filter);
        filter.BetIds.Should().NotBeNull();
        filter.BetIds.Should().HaveCount(2);
        filter.BetIds.Should().Contain("bet1");
        filter.BetIds.Should().Contain("bet2");
    }

    [Fact]
    public void WithBetIdsWhenInputIsValidAddsBetIds()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithBetIds("bet1", "bet2", "bet3");

        result.Should().BeSameAs(filter);
        filter.BetIds.Should().NotBeNull();
        filter.BetIds.Should().HaveCount(3);
        filter.BetIds.Should().Contain("bet1");
        filter.BetIds.Should().Contain("bet2");
        filter.BetIds.Should().Contain("bet3");
    }

    [Fact]
    public void WithBetIdsWhenCalledMultipleTimesAccumulatesBetIds()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithBetIds("bet1", "bet2")
                          .WithBetIds("bet3", "bet4");

        result.Should().BeSameAs(filter);
        filter.BetIds.Should().NotBeNull();
        filter.BetIds.Should().HaveCount(4);
        filter.BetIds.Should().Contain("bet1");
        filter.BetIds.Should().Contain("bet2");
        filter.BetIds.Should().Contain("bet3");
        filter.BetIds.Should().Contain("bet4");
    }

    [Fact]
    public void WithBetIdsWhenCalledWithDuplicatesDoesNotAddDuplicates()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithBetIds("bet1", "bet2")
                          .WithBetIds("bet2", "bet3");

        result.Should().BeSameAs(filter);
        filter.BetIds.Should().NotBeNull();
        filter.BetIds.Should().HaveCount(3);
        filter.BetIds.Should().Contain("bet1");
        filter.BetIds.Should().Contain("bet2");
        filter.BetIds.Should().Contain("bet3");
    }

    [Fact]
    public void WithMarketIdsWhenInputIsNullReturnsThis()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithMarketIds(null!);

        result.Should().BeSameAs(filter);
        filter.MarketIds.Should().BeNull();
    }

    [Fact]
    public void WithMarketIdsWhenInputIsEmptyCreatesEmptyHashSet()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithMarketIds();

        result.Should().BeSameAs(filter);
        filter.MarketIds.Should().NotBeNull();
        filter.MarketIds.Should().BeEmpty();
    }

    [Fact]
    public void WithMarketIdsWhenInputContainsNullsFiltersOutNulls()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithMarketIds("market1", null!, "market2", null!);

        result.Should().BeSameAs(filter);
        filter.MarketIds.Should().NotBeNull();
        filter.MarketIds.Should().HaveCount(2);
        filter.MarketIds.Should().Contain("market1");
        filter.MarketIds.Should().Contain("market2");
    }

    [Fact]
    public void WithMarketIdsWhenInputIsValidAddsMarketIds()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithMarketIds("market1", "market2", "market3");

        result.Should().BeSameAs(filter);
        filter.MarketIds.Should().NotBeNull();
        filter.MarketIds.Should().HaveCount(3);
        filter.MarketIds.Should().Contain("market1");
        filter.MarketIds.Should().Contain("market2");
        filter.MarketIds.Should().Contain("market3");
    }

    [Fact]
    public void WithMarketIdsWhenCalledMultipleTimesAccumulatesMarketIds()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithMarketIds("market1", "market2")
                          .WithMarketIds("market3", "market4");

        result.Should().BeSameAs(filter);
        filter.MarketIds.Should().NotBeNull();
        filter.MarketIds.Should().HaveCount(4);
        filter.MarketIds.Should().Contain("market1");
        filter.MarketIds.Should().Contain("market2");
        filter.MarketIds.Should().Contain("market3");
        filter.MarketIds.Should().Contain("market4");
    }

    [Fact]
    public void WithMarketIdsWhenCalledWithDuplicatesDoesNotAddDuplicates()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithMarketIds("market1", "market2")
                          .WithMarketIds("market2", "market3");

        result.Should().BeSameAs(filter);
        filter.MarketIds.Should().NotBeNull();
        filter.MarketIds.Should().HaveCount(3);
        filter.MarketIds.Should().Contain("market1");
        filter.MarketIds.Should().Contain("market2");
        filter.MarketIds.Should().Contain("market3");
    }

    [Theory]
    [InlineData(OrderProjection.All)]
    [InlineData(OrderProjection.Executable)]
    [InlineData(OrderProjection.ExecutionComplete)]
    public void WithOrderStatusSetsOrderProjection(OrderProjection orderStatus)
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithOrderProjection(orderStatus);

        result.Should().BeSameAs(filter);
        filter.OrderProjection.Should().Be(orderStatus);
    }

    [Fact]
    public void WithOrderStatusWhenCalledMultipleTimesOverwritesPreviousValue()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithOrderProjection(OrderProjection.Executable)
                          .WithOrderProjection(OrderProjection.ExecutionComplete);

        result.Should().BeSameAs(filter);
        filter.OrderProjection.Should().Be(OrderProjection.ExecutionComplete);
    }

    [Fact]
    public void WithCustomerOrderRefsWhenInputIsNullReturnsThis()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerOrderRefs(null!);

        result.Should().BeSameAs(filter);
        filter.CustomerOrderRefs.Should().BeNull();
    }

    [Fact]
    public void WithCustomerOrderRefsWhenInputIsEmptyCreatesEmptyHashSet()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerOrderRefs();

        result.Should().BeSameAs(filter);
        filter.CustomerOrderRefs.Should().NotBeNull();
        filter.CustomerOrderRefs.Should().BeEmpty();
    }

    [Fact]
    public void WithCustomerOrderRefsWhenInputContainsNullsFiltersOutNulls()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerOrderRefs("ref1", null!, "ref2", null!);

        result.Should().BeSameAs(filter);
        filter.CustomerOrderRefs.Should().NotBeNull();
        filter.CustomerOrderRefs.Should().HaveCount(2);
        filter.CustomerOrderRefs.Should().Contain("ref1");
        filter.CustomerOrderRefs.Should().Contain("ref2");
    }

    [Fact]
    public void WithCustomerOrderRefsWhenInputIsValidAddsCustomerOrderRefs()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerOrderRefs("ref1", "ref2", "ref3");

        result.Should().BeSameAs(filter);
        filter.CustomerOrderRefs.Should().NotBeNull();
        filter.CustomerOrderRefs.Should().HaveCount(3);
        filter.CustomerOrderRefs.Should().Contain("ref1");
        filter.CustomerOrderRefs.Should().Contain("ref2");
        filter.CustomerOrderRefs.Should().Contain("ref3");
    }

    [Fact]
    public void WithCustomerOrderRefsWhenCalledMultipleTimesAccumulatesCustomerOrderRefs()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerOrderRefs("ref1", "ref2")
                          .WithCustomerOrderRefs("ref3", "ref4");

        result.Should().BeSameAs(filter);
        filter.CustomerOrderRefs.Should().NotBeNull();
        filter.CustomerOrderRefs.Should().HaveCount(4);
        filter.CustomerOrderRefs.Should().Contain("ref1");
        filter.CustomerOrderRefs.Should().Contain("ref2");
        filter.CustomerOrderRefs.Should().Contain("ref3");
        filter.CustomerOrderRefs.Should().Contain("ref4");
    }

    [Fact]
    public void WithCustomerOrderRefsWhenCalledWithDuplicatesDoesNotAddDuplicates()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerOrderRefs("ref1", "ref2")
                          .WithCustomerOrderRefs("ref2", "ref3");

        result.Should().BeSameAs(filter);
        filter.CustomerOrderRefs.Should().NotBeNull();
        filter.CustomerOrderRefs.Should().HaveCount(3);
        filter.CustomerOrderRefs.Should().Contain("ref1");
        filter.CustomerOrderRefs.Should().Contain("ref2");
        filter.CustomerOrderRefs.Should().Contain("ref3");
    }

    [Fact]
    public void WithCustomerStrategyRefsWhenInputIsNullReturnsThis()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerStrategyRefs(null!);

        result.Should().BeSameAs(filter);
        filter.CustomerStrategyRefs.Should().BeNull();
    }

    [Fact]
    public void WithCustomerStrategyRefsWhenInputIsEmptyCreatesEmptyHashSet()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerStrategyRefs();

        result.Should().BeSameAs(filter);
        filter.CustomerStrategyRefs.Should().NotBeNull();
        filter.CustomerStrategyRefs.Should().BeEmpty();
    }

    [Fact]
    public void WithCustomerStrategyRefsWhenInputContainsNullsFiltersOutNulls()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerStrategyRefs("strategy1", null!, "strategy2", null!);

        result.Should().BeSameAs(filter);
        filter.CustomerStrategyRefs.Should().NotBeNull();
        filter.CustomerStrategyRefs.Should().HaveCount(2);
        filter.CustomerStrategyRefs.Should().Contain("strategy1");
        filter.CustomerStrategyRefs.Should().Contain("strategy2");
    }

    [Fact]
    public void WithCustomerStrategyRefsWhenInputIsValidAddsCustomerStrategyRefs()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerStrategyRefs("strategy1", "strategy2", "strategy3");

        result.Should().BeSameAs(filter);
        filter.CustomerStrategyRefs.Should().NotBeNull();
        filter.CustomerStrategyRefs.Should().HaveCount(3);
        filter.CustomerStrategyRefs.Should().Contain("strategy1");
        filter.CustomerStrategyRefs.Should().Contain("strategy2");
        filter.CustomerStrategyRefs.Should().Contain("strategy3");
    }

    [Fact]
    public void WithCustomerStrategyRefsWhenCalledMultipleTimesAccumulatesCustomerStrategyRefs()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerStrategyRefs("strategy1", "strategy2")
                          .WithCustomerStrategyRefs("strategy3", "strategy4");

        result.Should().BeSameAs(filter);
        filter.CustomerStrategyRefs.Should().NotBeNull();
        filter.CustomerStrategyRefs.Should().HaveCount(4);
        filter.CustomerStrategyRefs.Should().Contain("strategy1");
        filter.CustomerStrategyRefs.Should().Contain("strategy2");
        filter.CustomerStrategyRefs.Should().Contain("strategy3");
        filter.CustomerStrategyRefs.Should().Contain("strategy4");
    }

    [Fact]
    public void WithCustomerStrategyRefsWhenCalledWithDuplicatesDoesNotAddDuplicates()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerStrategyRefs("strategy1", "strategy2")
                          .WithCustomerStrategyRefs("strategy2", "strategy3");

        result.Should().BeSameAs(filter);
        filter.CustomerStrategyRefs.Should().NotBeNull();
        filter.CustomerStrategyRefs.Should().HaveCount(3);
        filter.CustomerStrategyRefs.Should().Contain("strategy1");
        filter.CustomerStrategyRefs.Should().Contain("strategy2");
        filter.CustomerStrategyRefs.Should().Contain("strategy3");
    }

    [Fact]
    public void WithDateRangeCreatesDateRangeAndSetsFromAndTo()
    {
        var filter = new ApiOrderFilter();
        var fromDate = new DateTimeOffset(2023, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var toDate = new DateTimeOffset(2023, 1, 2, 15, 30, 0, TimeSpan.Zero);

        var result = filter.WithDateRange(fromDate, toDate);

        result.Should().BeSameAs(filter);
        var dateRange = filter.DateRange;
        dateRange.Should().NotBeNull();

        dateRange!.From.Should().Be("2023-01-01T10:00:00Z");
        dateRange.To.Should().Be("2023-01-02T15:30:00Z");
    }

    [Fact]
    public void WithDateRangeWhenCalledMultipleTimesOverwritesPreviousValues()
    {
        var filter = new ApiOrderFilter();
        var firstFromDate = new DateTimeOffset(2023, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var firstToDate = new DateTimeOffset(2023, 1, 2, 15, 30, 0, TimeSpan.Zero);
        var secondFromDate = new DateTimeOffset(2023, 2, 1, 12, 0, 0, TimeSpan.Zero);
        var secondToDate = new DateTimeOffset(2023, 2, 2, 18, 45, 0, TimeSpan.Zero);

        var result = filter.WithDateRange(firstFromDate, firstToDate)
                          .WithDateRange(secondFromDate, secondToDate);

        result.Should().BeSameAs(filter);
        var dateRange = filter.DateRange;
        dateRange.Should().NotBeNull();

        dateRange!.From.Should().Be("2023-02-01T12:00:00Z");
        dateRange.To.Should().Be("2023-02-02T18:45:00Z");
    }

    [Fact]
    public void WithDateRangeConvertsToUtcFormat()
    {
        var filter = new ApiOrderFilter();
        var localDate = new DateTimeOffset(2023, 6, 15, 14, 30, 45, TimeSpan.FromHours(5));

        var result = filter.WithDateRange(localDate, localDate);

        result.Should().BeSameAs(filter);
        var dateRange = filter.DateRange;
        dateRange.Should().NotBeNull();

        dateRange!.From.Should().Be("2023-06-15T09:30:45Z");
        dateRange.To.Should().Be("2023-06-15T09:30:45Z");
    }

    [Theory]
    [InlineData(OrderBy.Market)]
    [InlineData(OrderBy.MatchTime)]
    [InlineData(OrderBy.PlaceTime)]
    [InlineData(OrderBy.SettledTime)]
    [InlineData(OrderBy.VoidTime)]
    public void OrderBySetsOrderByValue(OrderBy orderBy)
    {
        var filter = new ApiOrderFilter();

        var result = filter.OrderBy(orderBy);

        result.Should().BeSameAs(filter);
        filter.OrderByValue.Should().Be(orderBy);
    }

    [Fact]
    public void OrderByWhenCalledMultipleTimesOverwritesPreviousValue()
    {
        var filter = new ApiOrderFilter();

        var result = filter.OrderBy(OrderBy.PlaceTime)
                          .OrderBy(OrderBy.MatchTime);

        result.Should().BeSameAs(filter);
        filter.OrderByValue.Should().Be(OrderBy.MatchTime);
    }

    [Theory]
    [InlineData(SortDir.EarliestToLatest)]
    [InlineData(SortDir.LatestToEarliest)]
    public void SortBySetsSortDir(SortDir sortDir)
    {
        var filter = new ApiOrderFilter();

        var result = filter.SortBy(sortDir);

        result.Should().BeSameAs(filter);
        filter.SortDir.Should().Be(sortDir);
    }

    [Fact]
    public void SortByWhenCalledMultipleTimesOverwritesPreviousValue()
    {
        var filter = new ApiOrderFilter();

        var result = filter.SortBy(SortDir.EarliestToLatest)
                          .SortBy(SortDir.LatestToEarliest);

        result.Should().BeSameAs(filter);
        filter.SortDir.Should().Be(SortDir.LatestToEarliest);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void FromSetsFromRecord(int fromRecord)
    {
        var filter = new ApiOrderFilter();

        var result = filter.From(fromRecord);

        result.Should().BeSameAs(filter);
        filter.FromRecord.Should().Be(fromRecord);
    }

    [Fact]
    public void FromWhenCalledMultipleTimesOverwritesPreviousValue()
    {
        var filter = new ApiOrderFilter();

        var result = filter.From(10)
                          .From(20);

        result.Should().BeSameAs(filter);
        filter.FromRecord.Should().Be(20);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void FromAcceptsNegativeValues(int fromRecord)
    {
        var filter = new ApiOrderFilter();

        var result = filter.From(fromRecord);

        result.Should().BeSameAs(filter);
        filter.FromRecord.Should().Be(fromRecord);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(int.MaxValue)]
    public void TakeSetsRecordCount(int recordCount)
    {
        var filter = new ApiOrderFilter();

        var result = filter.Take(recordCount);

        result.Should().BeSameAs(filter);
        filter.RecordCount.Should().Be(recordCount);
    }

    [Fact]
    public void TakeWhenCalledMultipleTimesOverwritesPreviousValue()
    {
        var filter = new ApiOrderFilter();

        var result = filter.Take(100)
                          .Take(200);

        result.Should().BeSameAs(filter);
        filter.RecordCount.Should().Be(200);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void TakeAcceptsNegativeValues(int recordCount)
    {
        var filter = new ApiOrderFilter();

        var result = filter.Take(recordCount);

        result.Should().BeSameAs(filter);
        filter.RecordCount.Should().Be(recordCount);
    }

    [Fact]
    public void ExecutableOnlySetsOrderProjectionToExecutable()
    {
        var filter = new ApiOrderFilter();

        var result = filter.ExecutableOnly();

        result.Should().BeSameAs(filter);
        filter.OrderProjection.Should().Be(OrderProjection.Executable);
    }

    [Fact]
    public void ExecutableOnlyOverwritesPreviousOrderProjection()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithOrderProjection(OrderProjection.ExecutionComplete)
                          .ExecutableOnly();

        result.Should().BeSameAs(filter);
        filter.OrderProjection.Should().Be(OrderProjection.Executable);
    }

    [Fact]
    public void ExecutionCompleteOnlySetsOrderProjectionToExecutionComplete()
    {
        var filter = new ApiOrderFilter();

        var result = filter.ExecutionCompleteOnly();

        result.Should().BeSameAs(filter);
        filter.OrderProjection.Should().Be(OrderProjection.ExecutionComplete);
    }

    [Fact]
    public void ExecutionCompleteOnlyOverwritesPreviousOrderProjection()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithOrderProjection(OrderProjection.Executable)
                          .ExecutionCompleteOnly();

        result.Should().BeSameAs(filter);
        filter.OrderProjection.Should().Be(OrderProjection.ExecutionComplete);
    }

    [Fact]
    public void MostRecentFirstSetsSortDirToLatestToEarliest()
    {
        var filter = new ApiOrderFilter();

        var result = filter.MostRecentFirst();

        result.Should().BeSameAs(filter);
        filter.SortDir.Should().Be(SortDir.LatestToEarliest);
    }

    [Fact]
    public void MostRecentFirstOverwritesPreviousSortDir()
    {
        var filter = new ApiOrderFilter();

        var result = filter.SortBy(SortDir.EarliestToLatest)
                          .MostRecentFirst();

        result.Should().BeSameAs(filter);
        filter.SortDir.Should().Be(SortDir.LatestToEarliest);
    }

    [Fact]
    public void OldestFirstSetsSortDirToEarliestToLatest()
    {
        var filter = new ApiOrderFilter();

        var result = filter.OldestFirst();

        result.Should().BeSameAs(filter);
        filter.SortDir.Should().Be(SortDir.EarliestToLatest);
    }

    [Fact]
    public void OldestFirstOverwritesPreviousSortDir()
    {
        var filter = new ApiOrderFilter();

        var result = filter.SortBy(SortDir.LatestToEarliest)
                          .OldestFirst();

        result.Should().BeSameAs(filter);
        filter.SortDir.Should().Be(SortDir.EarliestToLatest);
    }

    [Fact]
    public void MethodChainingAllMethodsReturnSameInstance()
    {
        var filter = new ApiOrderFilter();

        var result = filter
            .WithBetIds("bet1")
            .WithMarketIds("market1")
            .WithOrderProjection(OrderProjection.Executable)
            .WithCustomerOrderRefs("ref1")
            .WithCustomerStrategyRefs("strategy1")
            .WithDateRange(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(1))
            .OrderBy(OrderBy.PlaceTime)
            .SortBy(SortDir.LatestToEarliest)
            .From(10)
            .Take(500)
            .ExecutableOnly()
            .MostRecentFirst();

        result.Should().BeSameAs(filter);
    }

    [Fact]
    public void MethodChainingComplexScenarioSetsAllPropertiesCorrectly()
    {
        var filter = new ApiOrderFilter();
        var fromDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var toDate = new DateTimeOffset(2023, 12, 31, 23, 59, 59, TimeSpan.Zero);

        var result = filter
            .WithBetIds("bet1", "bet2")
            .WithMarketIds("market1", "market2")
            .WithCustomerOrderRefs("ref1", "ref2")
            .WithCustomerStrategyRefs("strategy1", "strategy2")
            .WithDateRange(fromDate, toDate)
            .OrderBy(OrderBy.MatchTime)
            .OldestFirst()
            .From(50)
            .Take(250);

        result.Should().BeSameAs(filter);

        filter.BetIds.Should().NotBeNull();
        filter.BetIds.Should().HaveCount(2);
        filter.BetIds.Should().Contain("bet1");
        filter.BetIds.Should().Contain("bet2");

        filter.MarketIds.Should().NotBeNull();
        filter.MarketIds.Should().HaveCount(2);
        filter.MarketIds.Should().Contain("market1");
        filter.MarketIds.Should().Contain("market2");

        filter.CustomerOrderRefs.Should().NotBeNull();
        filter.CustomerOrderRefs.Should().HaveCount(2);
        filter.CustomerOrderRefs.Should().Contain("ref1");
        filter.CustomerOrderRefs.Should().Contain("ref2");

        filter.CustomerStrategyRefs.Should().NotBeNull();
        filter.CustomerStrategyRefs.Should().HaveCount(2);
        filter.CustomerStrategyRefs.Should().Contain("strategy1");
        filter.CustomerStrategyRefs.Should().Contain("strategy2");

        var dateRange = filter.DateRange;
        dateRange.Should().NotBeNull();

        dateRange!.From.Should().Be("2023-01-01T00:00:00Z");
        dateRange!.To.Should().Be("2023-12-31T23:59:59Z");

        filter.OrderByValue.Should().Be(OrderBy.MatchTime);
        filter.SortDir.Should().Be(SortDir.EarliestToLatest);
        filter.FromRecord.Should().Be(50);
        filter.RecordCount.Should().Be(250);
    }

    [Fact]
    public void WithBetIdsEmptyStringInArrayAddsEmptyString()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithBetIds("bet1", string.Empty, "bet2");

        result.Should().BeSameAs(filter);
        filter.BetIds.Should().NotBeNull();
        filter.BetIds.Should().HaveCount(3);
        filter.BetIds.Should().Contain("bet1");
        filter.BetIds.Should().Contain(string.Empty);
        filter.BetIds.Should().Contain("bet2");
    }

    [Fact]
    public void WithMarketIdsEmptyStringInArrayAddsEmptyString()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithMarketIds("market1", string.Empty, "market2");

        result.Should().BeSameAs(filter);
        filter.MarketIds.Should().NotBeNull();
        filter.MarketIds.Should().HaveCount(3);
        filter.MarketIds.Should().Contain("market1");
        filter.MarketIds.Should().Contain(string.Empty);
        filter.MarketIds.Should().Contain("market2");
    }

    [Fact]
    public void WithCustomerOrderRefsEmptyStringInArrayAddsEmptyString()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerOrderRefs("ref1", string.Empty, "ref2");

        result.Should().BeSameAs(filter);
        filter.CustomerOrderRefs.Should().NotBeNull();
        filter.CustomerOrderRefs.Should().HaveCount(3);
        filter.CustomerOrderRefs.Should().Contain("ref1");
        filter.CustomerOrderRefs.Should().Contain(string.Empty);
        filter.CustomerOrderRefs.Should().Contain("ref2");
    }

    [Fact]
    public void WithCustomerStrategyRefsEmptyStringInArrayAddsEmptyString()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithCustomerStrategyRefs("strategy1", string.Empty, "strategy2");

        result.Should().BeSameAs(filter);
        filter.CustomerStrategyRefs.Should().NotBeNull();
        filter.CustomerStrategyRefs.Should().HaveCount(3);
        filter.CustomerStrategyRefs.Should().Contain("strategy1");
        filter.CustomerStrategyRefs.Should().Contain(string.Empty);
        filter.CustomerStrategyRefs.Should().Contain("strategy2");
    }

    [Fact]
    public void WithDateRangeMinAndMaxDateTimeOffsetHandlesExtremeValues()
    {
        var filter = new ApiOrderFilter();

        var result = filter.WithDateRange(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);

        result.Should().BeSameAs(filter);
        var dateRange = filter.DateRange;
        dateRange.Should().NotBeNull();

        dateRange!.From.Should().Be("0001-01-01T00:00:00Z");
        dateRange.To.Should().Be("9999-12-31T23:59:59Z");
    }
}
