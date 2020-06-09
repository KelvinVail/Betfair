namespace Betfair.Extensions.Tests
{
    using System.Collections.Generic;
    using Betfair.Extensions;
    using Betfair.Extensions.Tests.TestDoubles;
    using Betfair.Stream.Responses;
    using Xunit;

    public class MarketCacheTests
    {
        private readonly MarketCache marketCache;

        public MarketCacheTests()
        {
            this.marketCache = new MarketCache(DefaultMarketId);
        }

        private static string DefaultMarketId => "1.2345";

        private static int DefaultSelectionId => 12345;

        [Fact]
        public void WhenInitializedMarketIdIsSet()
        {
            Assert.Equal(DefaultMarketId, this.marketCache.MarketId);
        }

        [Fact]
        public void HandleNullMarketChange()
        {
            this.marketCache.OnMarketChange(null, 0);
        }

        [Fact]
        public void HandleNullRunnerChange()
        {
            var change = new MarketChangeStub().WithRunnerChange(null);
        }

        [Fact]
        public void OnMarketChangeTheChangeIsProcessed()
        {
            var marketChange = new MarketChange();
            this.marketCache.OnMarketChange(marketChange, 0);
        }

        [Fact]
        public void IfChangeContainMarketDefinitionTheMarketDefinitionIsSet()
        {
            var changeMessage = new MarketChangeStub().WithMarketDefinition(new MarketDefinition());
            this.marketCache.OnMarketChange(changeMessage, 0);

            Assert.NotNull(this.marketCache.MarketDefinition);
        }

        [Fact]
        public void IfChangeDoesNotContainMarketDefinitionAnyPreviousMarketDefinitionShouldNotBeCleared()
        {
            var marketDefinition = new MarketDefinition { Venue = "Test Venue" };
            var marketChange1 = new MarketChangeStub().WithMarketDefinition(marketDefinition);
            this.marketCache.OnMarketChange(marketChange1, 0);

            var marketChange2 = new MarketChangeStub();
            this.marketCache.OnMarketChange(marketChange2, 0);

            Assert.Equal("Test Venue", this.marketCache.MarketDefinition.Venue);
        }

        [Fact]
        public void IfChangeIsForDifferentMarketDoNotProcessTheChange()
        {
            this.marketCache.OnMarketChange(
                new MarketChangeStub()
                    .WithMarketId("Different MarketId")
                    .WithMarketDefinition(new MarketDefinition()), 0);

            Assert.Null(this.marketCache.MarketDefinition);
        }

        [Fact]
        public void IfChangeContainsTotalMatchedUpdateTotalMatched()
        {
            const int TotalMatched = 20;
            this.marketCache.OnMarketChange(
                new MarketChangeStub()
                    .WithTotalMatched(TotalMatched), 0);

            Assert.Equal(TotalMatched, this.marketCache.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeDoesNotContainTotalMatchedDoNotClearTotalMatched()
        {
            const int TotalMatched = 20;
            this.marketCache.OnMarketChange(new MarketChangeStub().WithTotalMatched(TotalMatched), 0);
            this.marketCache.OnMarketChange(new MarketChangeStub(), 0);

            Assert.Equal(TotalMatched, this.marketCache.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeContainsRunnersAddThemToTheCache()
        {
            var changeMessage = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(1))
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(2));
            this.marketCache.OnMarketChange(changeMessage, 0);

            const int ExpectedRunnerCount = 2;
            Assert.Equal(ExpectedRunnerCount, this.marketCache.Runners.Count);
        }

        [Fact]
        public void DoNotProcessRunnerWithNullSelectionId()
        {
            var changeMessage = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(null))
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(2));
            this.marketCache.OnMarketChange(changeMessage, 0);

            Assert.Single(this.marketCache.Runners);
        }

        [Fact]
        public void IfChangeContainsRunnerUpdateRunnerCache()
        {
            const int TotalMatched = 20;
            var changeMessage = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub().WithTotalMatched(TotalMatched));
            this.marketCache.OnMarketChange(changeMessage, 0);

            Assert.Equal(TotalMatched, this.marketCache.Runners[DefaultSelectionId].TotalMatched);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedMarketDefinitionIsCleared()
        {
            var changeMessage = new MarketChangeStub().WithMarketDefinition(new MarketDefinition());
            this.marketCache.OnMarketChange(changeMessage, 0);

            var changeMessage2 = new MarketChangeStub().WithReplaceCache();
            this.marketCache.OnMarketChange(changeMessage2, 0);

            Assert.Null(this.marketCache.MarketDefinition);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedMarketDefinitionIsReplaced()
        {
            var changeMessage = new MarketChangeStub().WithMarketDefinition(new MarketDefinition { Version = 1 });
            this.marketCache.OnMarketChange(changeMessage, 0);

            var changeMessage2 = new MarketChangeStub().WithReplaceCache().WithMarketDefinition(new MarketDefinition { Version = 2 });
            this.marketCache.OnMarketChange(changeMessage2, 0);

            Assert.Equal(2, this.marketCache.MarketDefinition.Version);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedTotalAmountMatchedIsCleared()
        {
            var changeMessage = new MarketChangeStub().WithTotalMatched(20);
            this.marketCache.OnMarketChange(changeMessage, 0);

            var changeMessage2 = new MarketChangeStub().WithReplaceCache();
            this.marketCache.OnMarketChange(changeMessage2, 0);

            Assert.Equal(0, this.marketCache.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedTotalAmountMatchedIsReplaced()
        {
            var changeMessage = new MarketChangeStub().WithTotalMatched(20);
            this.marketCache.OnMarketChange(changeMessage, 0);

            var changeMessage2 = new MarketChangeStub().WithReplaceCache().WithTotalMatched(30);
            this.marketCache.OnMarketChange(changeMessage2, 0);

            Assert.Equal(30, this.marketCache.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedRunnersAreCleared()
        {
            var changeMessage = new MarketChangeStub().WithRunnerChange(new RunnerChangeStub());
            this.marketCache.OnMarketChange(changeMessage, 0);

            var changeMessage2 = new MarketChangeStub().WithReplaceCache();
            this.marketCache.OnMarketChange(changeMessage2, 0);

            Assert.Empty(this.marketCache.Runners);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedRunnersAreReplaced()
        {
            var changeMessage = new MarketChangeStub().WithRunnerChange(new RunnerChangeStub());
            this.marketCache.OnMarketChange(changeMessage, 0);

            const int TotalMatched = 20;
            var changeMessage2 = new MarketChangeStub().WithReplaceCache().WithRunnerChange(new RunnerChangeStub().WithTotalMatched(TotalMatched));
            this.marketCache.OnMarketChange(changeMessage2, 0);

            Assert.Equal(TotalMatched, this.marketCache.Runners[DefaultSelectionId].TotalMatched);
        }

        [Fact]
        public void OnMarketChangeThePublishedTimeOfTheMessageIsRecorded()
        {
            const int PublishTime = 123;
            var changeMessage = new MarketChangeStub();
            this.marketCache.OnMarketChange(changeMessage, PublishTime);

            Assert.Equal(PublishTime, this.marketCache.LastPublishedTime);
        }

        [Fact]
        public void WhenMarketChangeIsForWrongMarketThePublishedTimeOfTheMessageIsNotUpdated()
        {
            const int PublishTime = 123;
            var changeMessage = new MarketChangeStub();
            this.marketCache.OnMarketChange(changeMessage, PublishTime);

            var changeMessage2 = new MarketChangeStub().WithMarketId("WrongId");
            this.marketCache.OnMarketChange(changeMessage2, 456);

            Assert.Equal(PublishTime, this.marketCache.LastPublishedTime);
        }

        [Fact]
        public void OnRunnerChangePublishTimeIsSet()
        {
            var runnerChange = new RunnerChangeStub();
            var marketChange = new MarketChangeStub().WithRunnerChange(runnerChange);
            this.marketCache.OnMarketChange(marketChange, 98765);

            Assert.Equal(98765, this.marketCache.Runners[12345].LastPublishTime);
        }

        [Fact]
        public void OnMarketDefinitionAdjustmentFactorIsSet()
        {
            var rc = new RunnerChangeStub();
            var rd = new RunnerDefinition { SelectionId = 12345, AdjustmentFactor = 54.32 };
            var md = new MarketDefinition { Runners = new List<RunnerDefinition> { rd } };
            var mc = new MarketChangeStub().WithMarketDefinition(md).WithRunnerChange(rc);

            this.marketCache.OnMarketChange(mc, 0);
            Assert.Equal(54.32, this.marketCache.Runners[12345].AdjustmentFactor);
        }

        [Fact]
        public void HandleNullSelectionIdInRunnerDefinition()
        {
            var rd = new RunnerDefinition { AdjustmentFactor = 54.32 };
            var md = new MarketDefinition { Runners = new List<RunnerDefinition> { rd } };
            var mc = new MarketChangeStub().WithMarketDefinition(md);

            this.marketCache.OnMarketChange(mc, 0);
        }

        [Fact]
        public void HandleRunnerDefinitionForRunnerThatDoesNotExist()
        {
            var rc = new RunnerChangeStub();
            var rd = new RunnerDefinition { SelectionId = 1, AdjustmentFactor = 54.32 };
            var md = new MarketDefinition { Runners = new List<RunnerDefinition> { rd } };
            var mc = new MarketChangeStub().WithMarketDefinition(md).WithRunnerChange(rc);

            this.marketCache.OnMarketChange(mc, 0);
        }
    }
}
