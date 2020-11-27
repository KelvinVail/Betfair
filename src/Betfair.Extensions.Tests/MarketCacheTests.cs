namespace Betfair.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    using Betfair.Extensions;
    using Betfair.Extensions.Tests.TestDoubles;
    using Betfair.Stream.Responses;
    using Xunit;

    public class MarketCacheTests
    {
        private readonly MarketCache market;

        private readonly ChangeMessageStub change = new ChangeMessageStub();

        public MarketCacheTests()
        {
            this.market = new MarketCache(DefaultMarketId);
        }

        private static string DefaultMarketId => "1.2345";

        private static int DefaultSelectionId => 12345;

        [Fact]
        public void WhenInitializedMarketIdIsSet()
        {
            Assert.Equal(DefaultMarketId, this.market.MarketId);
        }

        [Fact]
        public void HandleNullChange()
        {
            this.market.OnChange(null);
        }

        [Fact]
        public void HandleNullMarketChange()
        {
            this.market.OnChange(this.change.WithMarketChange(null).Build());
        }

        [Fact]
        public void OnMarketChangeTheChangeIsProcessed()
        {
            this.market.OnChange(this.change.WithMarketChange(new MarketChange()).Build());
        }

        [Fact]
        public void IfChangeContainMarketDefinitionTheMarketDefinitionIsSet()
        {
            var mc = new MarketChangeStub().WithMarketDefinition(new MarketDefinition());
            this.market.OnChange(this.change.WithMarketChange(mc).Build());
            Assert.NotNull(this.market.MarketDefinition);
        }

        [Fact]
        public void IfChangeDoesNotContainMarketDefinitionAnyPreviousMarketDefinitionShouldNotBeCleared()
        {
            var mc1 = new MarketChangeStub().WithMarketDefinition(new MarketDefinition { Venue = "Test Venue" });
            var c1 = new ChangeMessageStub().WithMarketChange(mc1).Build();
            this.market.OnChange(c1);

            var mc = new MarketChange { MarketId = "1.2345" };
            var c2 = this.change.WithMarketChange(mc).Build();
            this.market.OnChange(c2);

            Assert.Equal("Test Venue", this.market.MarketDefinition.Venue);
        }

        [Fact]
        public void IfChangeIsForDifferentMarketDoNotProcessTheChange()
        {
            var mc = new MarketChangeStub()
                .WithMarketId("Different MarketId")
                .WithMarketDefinition(new MarketDefinition());
            this.market.OnChange(this.change.WithMarketChange(mc).Build());

            Assert.Null(this.market.MarketDefinition);
        }

        [Fact]
        public void IfChangeContainsTotalMatchedUpdateTotalMatched()
        {
            const int TotalMatched = 20;
            var mc = new MarketChangeStub()
                .WithTotalMatched(TotalMatched);
            this.market.OnChange(this.change.WithMarketChange(mc).Build());

            Assert.Equal(TotalMatched, this.market.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeDoesNotContainTotalMatchedDoNotClearTotalMatched()
        {
            const int TotalMatched = 20;
            var mc = new MarketChangeStub()
                .WithTotalMatched(TotalMatched);
            this.market.OnChange(this.change.WithMarketChange(mc).Build());

            this.market.OnChange(this.change.WithMarketChange(new MarketChangeStub()).Build());

            Assert.Equal(TotalMatched, this.market.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeContainsRunnersAddThemToTheCache()
        {
            var mc = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(1))
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(2));
            this.market.OnChange(this.change.WithMarketChange(mc).Build());

            const int ExpectedRunnerCount = 2;
            Assert.Equal(ExpectedRunnerCount, this.market.Runners.Count);
        }

        [Fact]
        public void DoNotProcessRunnerWithNullSelectionId()
        {
            var mc = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(null))
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(2));
            this.market.OnChange(this.change.WithMarketChange(mc).Build());

            Assert.Single(this.market.Runners);
        }

        [Fact]
        public void IfChangeContainsRunnerUpdateRunnerCache()
        {
            const int TotalMatched = 20;
            var mc = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub()
                    .WithTotalMatched(TotalMatched));
            this.market.OnChange(this.change.WithMarketChange(mc).Build());

            Assert.Equal(
                TotalMatched, this.market.Runners[DefaultSelectionId].TotalMatched);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedMarketDefinitionIsCleared()
        {
            var mc1 = new MarketChangeStub()
                .WithMarketDefinition(new MarketDefinition());
            this.market.OnChange(this.change.WithMarketChange(mc1).Build());

            var mc2 = new MarketChangeStub().WithReplaceCache();
            this.market.OnChange(this.change.WithMarketChange(mc2).Build());

            Assert.Null(this.market.MarketDefinition);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedMarketDefinitionIsReplaced()
        {
            var mc1 = new MarketChangeStub()
                .WithMarketDefinition(new MarketDefinition { Version = 1 });
            this.market.OnChange(this.change.WithMarketChange(mc1).Build());

            var mc2 = new MarketChangeStub()
                .WithReplaceCache()
                .WithMarketDefinition(new MarketDefinition { Version = 2 });
            this.market.OnChange(this.change.WithMarketChange(mc2).Build());

            Assert.Equal(2, this.market.MarketDefinition.Version);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedTotalAmountMatchedIsCleared()
        {
            var mc1 = new MarketChangeStub().WithTotalMatched(20);
            this.market.OnChange(this.change.WithMarketChange(mc1).Build());

            var mc2 = new MarketChangeStub().WithReplaceCache();
            this.market.OnChange(this.change.WithMarketChange(mc2).Build());

            Assert.Equal(0, this.market.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedTotalAmountMatchedIsReplaced()
        {
            var mc1 = new MarketChangeStub().WithTotalMatched(20);
            this.market.OnChange(this.change.WithMarketChange(mc1).Build());

            var mc2 = new MarketChangeStub()
                .WithReplaceCache()
                .WithTotalMatched(30);
            this.market.OnChange(this.change.WithMarketChange(mc2).Build());

            Assert.Equal(30, this.market.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedRunnersAreCleared()
        {
            var mc1 = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub());
            this.market.OnChange(this.change.WithMarketChange(mc1).Build());

            var mc2 = new MarketChangeStub().WithReplaceCache();
            this.market.OnChange(this.change.WithMarketChange(mc2).Build());

            Assert.Empty(this.market.Runners);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedRunnersAreReplaced()
        {
            var mc1 = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub());
            this.market.OnChange(this.change.WithMarketChange(mc1).Build());

            const int TotalMatched = 20;
            var mc2 = new MarketChangeStub()
                .WithReplaceCache()
                .WithRunnerChange(new RunnerChangeStub()
                    .WithTotalMatched(TotalMatched));
            this.market.OnChange(this.change.WithMarketChange(mc2).Build());

            Assert.Equal(
                TotalMatched, this.market.Runners[DefaultSelectionId].TotalMatched);
        }

        [Fact]
        public void OnMarketChangeThePublishedTimeOfTheMessageIsRecorded()
        {
            const int PublishTime = 123;
            var mc = new MarketChangeStub();
            this.market.OnChange(this.change.WithMarketChange(mc).WithPublishTime(PublishTime).Build());

            Assert.Equal(PublishTime, this.market.LastPublishedTime);
        }

        [Fact]
        public void WhenMarketChangeIsForWrongMarketThePublishedTimeOfTheMessageIsNotUpdated()
        {
            const int PublishTime = 123;
            var mc = new MarketChangeStub();
            this.market.OnChange(this.change.WithMarketChange(mc).WithPublishTime(PublishTime).Build());

            var mc2 = new MarketChangeStub().WithMarketId("WrongId");
            this.market.OnChange(this.change.New().WithMarketChange(mc2).WithPublishTime(456).Build());

            Assert.Equal(PublishTime, this.market.LastPublishedTime);
        }

        [Fact]
        public void OnRunnerChangePublishTimeIsSet()
        {
            var runnerChange = new RunnerChangeStub();
            var mc = new MarketChangeStub().WithRunnerChange(runnerChange);
            this.market.OnChange(this.change.WithMarketChange(mc).WithPublishTime(98765).Build());

            Assert.Equal(98765, this.market.Runners[12345].LastPublishTime);
        }

        [Fact]
        public void OnMarketDefinitionAdjustmentFactorIsSet()
        {
            var rc = new RunnerChangeStub();
            var rd = new RunnerDefinition
            {
                SelectionId = 12345, AdjustmentFactor = 54.32,
            };
            var md = new MarketDefinition
            {
                Runners = new List<RunnerDefinition> { rd },
            };
            var mc = new MarketChangeStub()
                .WithMarketDefinition(md).WithRunnerChange(rc);

            this.market.OnChange(this.change.WithMarketChange(mc).Build());
            Assert.Equal(54.32, this.market.Runners[12345].AdjustmentFactor);
        }

        [Fact]
        public void HandleNullSelectionIdInRunnerDefinition()
        {
            var rd = new RunnerDefinition { AdjustmentFactor = 54.32 };
            var md = new MarketDefinition
            {
                Runners = new List<RunnerDefinition> { rd },
            };
            var mc = new MarketChangeStub().WithMarketDefinition(md);

            this.market.OnChange(this.change.WithMarketChange(mc).Build());
        }

        [Fact]
        public void HandleRunnerDefinitionForRunnerThatDoesNotExist()
        {
            var rc = new RunnerChangeStub();
            var rd = new RunnerDefinition
            {
                SelectionId = 1, AdjustmentFactor = 54.32,
            };
            var md = new MarketDefinition
            {
                Runners = new List<RunnerDefinition> { rd },
            };
            var mc = new MarketChangeStub()
                .WithMarketDefinition(md)
                .WithRunnerChange(rc);

            this.market.OnChange(this.change.WithMarketChange(mc).Build());
        }

        [Fact]
        public void ProcessOrderChange()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(3.5, 10.99);
            var oc = new OrderChangeStub()
                .WithOrderRunnerChange(orc);
            this.market.OnChange(this.change.WithOrderChange(oc).Build());

            Assert.Equal(-10.99, this.market.Runners[12345].IfLose);
        }

        [Fact]
        public void HandleNullSelectionIdOnOrderChange()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(3.5, 10.99);
            orc.SelectionId = null;
            var oc = new OrderChangeStub()
                .WithOrderRunnerChange(orc);
            this.market.OnChange(this.change.WithOrderChange(oc).Build());

            Assert.Empty(this.market.Runners);
        }

        [Fact]
        public void CalculateRunnerProfitForTwoRunners()
        {
            var orc1 = new OrderRunnerChangeStub(1)
                .WithMatchedBack(3.5, 10.99);

            var orc2 = new OrderRunnerChangeStub(2)
                .WithMatchedBack(10, 1.5);

            var orc3 = new OrderRunnerChangeStub(3)
                .WithMatchedLay(11, 6.2);

            var orc4 = new OrderRunnerChangeStub(4)
                .WithMatchedBack(8, 2)
                .WithMatchedLay(8.8, 3);

            var oc = new OrderChangeStub()
                .WithOrderRunnerChange(orc1)
                .WithOrderRunnerChange(orc2)
                .WithOrderRunnerChange(orc3)
                .WithOrderRunnerChange(orc4);
            this.market.OnChange(this.change.WithOrderChange(oc).Build());

            Assert.Equal(33.18, this.market.Runners[1].Profit);
            Assert.Equal(9.71, this.market.Runners[2].Profit);
            Assert.Equal(-73.49, this.market.Runners[3].Profit);
            Assert.Equal(-15.69, this.market.Runners[4].Profit);
        }

        [Fact]
        public void HandleNullOrderChange()
        {
            this.market.OnChange(this.change.WithOrderChange(null).Build());
        }

        [Theory]
        [InlineData(37.9201)]
        [InlineData(12.01)]
        [InlineData(8.8)]
        public void CalculateTotalLiability(double size)
        {
            var orc1 = new OrderRunnerChangeStub(1)
                .WithMatchedBack(3.5, 10.99);

            var orc2 = new OrderRunnerChangeStub(2)
                .WithMatchedBack(10, 1.5)
                .WithMatchedLay(9.5, 20.01)
                .WithUnmatchedBack(11, size);

            var oc = new OrderChangeStub()
                .WithOrderRunnerChange(orc1)
                .WithOrderRunnerChange(orc2);
            this.market.OnChange(this.change.WithOrderChange(oc).Build());

            Assert.Equal(45.99, this.market.Runners[1].Profit);
            Assert.Equal(-167.58, this.market.Runners[2].Profit);

            var expected = -167.58 - Math.Round(size, 2);
            Assert.Equal(Math.Round(expected, 2), this.market.Liability);
        }

        [Fact]
        public void DoNotProcessOrderChangeForWrongMarket()
        {
            var orc = new OrderRunnerChangeStub(1).WithMatchedBack(2.5, 10.99);
            var oc = new OrderChangeStub("WrongMarketId").WithOrderRunnerChange(orc);
            var c = this.change.WithOrderChange(oc).Build();
            this.market.OnChange(c);

            Assert.Empty(this.market.Runners);
        }

        [Fact]
        public void LiabilityIsZeroIfNoRunners()
        {
            Assert.Equal(0, this.market.Liability);
        }
    }
}
