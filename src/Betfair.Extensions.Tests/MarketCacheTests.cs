using System;
using System.Collections.Generic;
using Betfair.Extensions.Tests.TestDoubles;
using Betfair.Stream.Responses;
using Xunit;

namespace Betfair.Extensions.Tests
{
    public class MarketCacheTests
    {
        private readonly MarketCache _market;
        private readonly ChangeMessageStub _change = new ChangeMessageStub();

        public MarketCacheTests()
        {
            _market = new MarketCache(DefaultMarketId);
        }

        private static string DefaultMarketId => "1.2345";

        private static int DefaultSelectionId => 12345;

        [Fact]
        public void WhenInitializedMarketIdIsSet()
        {
            Assert.Equal(DefaultMarketId, _market.MarketId);
        }

        [Fact]
        public void HandleNullChange()
        {
            _market.OnChange(null);
        }

        [Fact]
        public void HandleNullMarketChange()
        {
            _market.OnChange(_change.WithMarketChange(null).Build());
        }

        [Fact]
        public void OnMarketChangeTheChangeIsProcessed()
        {
            _market.OnChange(_change.WithMarketChange(new MarketChange()).Build());
        }

        [Fact]
        public void IfChangeContainMarketDefinitionTheMarketDefinitionIsSet()
        {
            var mc = new MarketChangeStub().WithMarketDefinition(new MarketDefinition());
            _market.OnChange(_change.WithMarketChange(mc).Build());
            Assert.NotNull(_market.MarketDefinition);
        }

        [Fact]
        public void IfChangeDoesNotContainMarketDefinitionAnyPreviousMarketDefinitionShouldNotBeCleared()
        {
            var mc1 = new MarketChangeStub().WithMarketDefinition(new MarketDefinition { Venue = "Test Venue" });
            var c1 = new ChangeMessageStub().WithMarketChange(mc1).Build();
            _market.OnChange(c1);

            var mc = new MarketChange { MarketId = "1.2345" };
            var c2 = _change.WithMarketChange(mc).Build();
            _market.OnChange(c2);

            Assert.Equal("Test Venue", _market.MarketDefinition.Venue);
        }

        [Fact]
        public void IfChangeIsForDifferentMarketDoNotProcessTheChange()
        {
            var mc = new MarketChangeStub()
                .WithMarketId("Different MarketId")
                .WithMarketDefinition(new MarketDefinition());
            _market.OnChange(_change.WithMarketChange(mc).Build());

            Assert.Null(_market.MarketDefinition);
        }

        [Fact]
        public void IfChangeContainsTotalMatchedUpdateTotalMatched()
        {
            const int TotalMatched = 20;
            var mc = new MarketChangeStub()
                .WithTotalMatched(TotalMatched);
            _market.OnChange(_change.WithMarketChange(mc).Build());

            Assert.Equal(TotalMatched, _market.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeDoesNotContainTotalMatchedDoNotClearTotalMatched()
        {
            const int TotalMatched = 20;
            var mc = new MarketChangeStub()
                .WithTotalMatched(TotalMatched);
            _market.OnChange(_change.WithMarketChange(mc).Build());

            _market.OnChange(_change.WithMarketChange(new MarketChangeStub()).Build());

            Assert.Equal(TotalMatched, _market.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeContainsRunnersAddThemToTheCache()
        {
            var mc = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(1))
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(2));
            _market.OnChange(_change.WithMarketChange(mc).Build());

            const int ExpectedRunnerCount = 2;
            Assert.Equal(ExpectedRunnerCount, _market.Runners.Count);
        }

        [Fact]
        public void DoNotProcessRunnerWithNullSelectionId()
        {
            var mc = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(null))
                .WithRunnerChange(new RunnerChangeStub().WithSelectionId(2));
            _market.OnChange(_change.WithMarketChange(mc).Build());

            Assert.Single(_market.Runners);
        }

        [Fact]
        public void IfChangeContainsRunnerUpdateRunnerCache()
        {
            const int TotalMatched = 20;
            var mc = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub()
                    .WithTotalMatched(TotalMatched));
            _market.OnChange(_change.WithMarketChange(mc).Build());

            Assert.Equal(
                TotalMatched, _market.Runners[DefaultSelectionId].TotalMatched);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedMarketDefinitionIsCleared()
        {
            var mc1 = new MarketChangeStub()
                .WithMarketDefinition(new MarketDefinition());
            _market.OnChange(_change.WithMarketChange(mc1).Build());

            var mc2 = new MarketChangeStub().WithReplaceCache();
            _market.OnChange(_change.WithMarketChange(mc2).Build());

            Assert.Null(_market.MarketDefinition);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedMarketDefinitionIsReplaced()
        {
            var mc1 = new MarketChangeStub()
                .WithMarketDefinition(new MarketDefinition { Version = 1 });
            _market.OnChange(_change.WithMarketChange(mc1).Build());

            var mc2 = new MarketChangeStub()
                .WithReplaceCache()
                .WithMarketDefinition(new MarketDefinition { Version = 2 });
            _market.OnChange(_change.WithMarketChange(mc2).Build());

            Assert.Equal(2, _market.MarketDefinition.Version);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedTotalAmountMatchedIsCleared()
        {
            var mc1 = new MarketChangeStub().WithTotalMatched(20);
            _market.OnChange(_change.WithMarketChange(mc1).Build());

            var mc2 = new MarketChangeStub().WithReplaceCache();
            _market.OnChange(_change.WithMarketChange(mc2).Build());

            Assert.Equal(0, _market.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedTotalAmountMatchedIsReplaced()
        {
            var mc1 = new MarketChangeStub().WithTotalMatched(20);
            _market.OnChange(_change.WithMarketChange(mc1).Build());

            var mc2 = new MarketChangeStub()
                .WithReplaceCache()
                .WithTotalMatched(30);
            _market.OnChange(_change.WithMarketChange(mc2).Build());

            Assert.Equal(30, _market.TotalAmountMatched);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedRunnersAreCleared()
        {
            var mc1 = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub());
            _market.OnChange(_change.WithMarketChange(mc1).Build());

            var mc2 = new MarketChangeStub().WithReplaceCache();
            _market.OnChange(_change.WithMarketChange(mc2).Build());

            Assert.Empty(_market.Runners);
        }

        [Fact]
        public void IfChangeAsksForCacheToBeReplacedRunnersAreReplaced()
        {
            var mc1 = new MarketChangeStub()
                .WithRunnerChange(new RunnerChangeStub());
            _market.OnChange(_change.WithMarketChange(mc1).Build());

            const int TotalMatched = 20;
            var mc2 = new MarketChangeStub()
                .WithReplaceCache()
                .WithRunnerChange(new RunnerChangeStub()
                    .WithTotalMatched(TotalMatched));
            _market.OnChange(_change.WithMarketChange(mc2).Build());

            Assert.Equal(
                TotalMatched, _market.Runners[DefaultSelectionId].TotalMatched);
        }

        [Fact]
        public void OnMarketChangeThePublishedTimeOfTheMessageIsRecorded()
        {
            const int PublishTime = 123;
            var mc = new MarketChangeStub();
            _market.OnChange(_change.WithMarketChange(mc).WithPublishTime(PublishTime).Build());

            Assert.Equal(PublishTime, _market.LastPublishedTime);
        }

        [Fact]
        public void WhenMarketChangeIsForWrongMarketThePublishedTimeOfTheMessageIsNotUpdated()
        {
            const int PublishTime = 123;
            var mc = new MarketChangeStub();
            _market.OnChange(_change.WithMarketChange(mc).WithPublishTime(PublishTime).Build());

            var mc2 = new MarketChangeStub().WithMarketId("WrongId");
            _market.OnChange(_change.New().WithMarketChange(mc2).WithPublishTime(456).Build());

            Assert.Equal(PublishTime, _market.LastPublishedTime);
        }

        [Fact]
        public void OnRunnerChangePublishTimeIsSet()
        {
            var runnerChange = new RunnerChangeStub();
            var mc = new MarketChangeStub().WithRunnerChange(runnerChange);
            _market.OnChange(_change.WithMarketChange(mc).WithPublishTime(98765).Build());

            Assert.Equal(98765, _market.Runners[12345].LastPublishTime);
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

            _market.OnChange(_change.WithMarketChange(mc).Build());
            Assert.Equal(54.32, _market.Runners[12345].AdjustmentFactor);
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

            _market.OnChange(_change.WithMarketChange(mc).Build());
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

            _market.OnChange(_change.WithMarketChange(mc).Build());
        }

        [Fact]
        public void ProcessOrderChange()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(3.5, 10.99);
            var oc = new OrderChangeStub()
                .WithOrderRunnerChange(orc);
            _market.OnChange(_change.WithOrderChange(oc).Build());

            Assert.Equal(-10.99, _market.Runners[12345].IfLose);
        }

        [Fact]
        public void HandleNullSelectionIdOnOrderChange()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(3.5, 10.99);
            orc.SelectionId = null;
            var oc = new OrderChangeStub()
                .WithOrderRunnerChange(orc);
            _market.OnChange(_change.WithOrderChange(oc).Build());

            Assert.Empty(_market.Runners);
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
            _market.OnChange(_change.WithOrderChange(oc).Build());

            Assert.Equal(33.18, _market.Runners[1].Profit);
            Assert.Equal(9.71, _market.Runners[2].Profit);
            Assert.Equal(-73.49, _market.Runners[3].Profit);
            Assert.Equal(-15.69, _market.Runners[4].Profit);
        }

        [Fact]
        public void HandleNullOrderChange()
        {
            _market.OnChange(_change.WithOrderChange(null).Build());
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
            _market.OnChange(_change.WithOrderChange(oc).Build());

            Assert.Equal(45.99, _market.Runners[1].Profit);
            Assert.Equal(-167.58, _market.Runners[2].Profit);

            var expected = -167.58 - Math.Round(size, 2);
            Assert.Equal(Math.Round(expected, 2), _market.Liability);
        }

        [Fact]
        public void DoNotProcessOrderChangeForWrongMarket()
        {
            var orc = new OrderRunnerChangeStub(1).WithMatchedBack(2.5, 10.99);
            var oc = new OrderChangeStub("WrongMarketId").WithOrderRunnerChange(orc);
            var c = _change.WithOrderChange(oc).Build();
            _market.OnChange(c);

            Assert.Empty(_market.Runners);
        }

        [Fact]
        public void LiabilityIsZeroIfNoRunners()
        {
            Assert.Equal(0, _market.Liability);
        }
    }
}
