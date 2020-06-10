namespace Betfair.Extensions.Tests
{
    using System.Collections.Generic;
    using Betfair.Extensions;
    using Betfair.Extensions.Tests.TestDoubles;
    using Betfair.Stream.Responses;
    using Xunit;

    public class RunnerCacheTests
    {
        private readonly RunnerCache runnerCache;

        public RunnerCacheTests()
        {
            this.runnerCache = new RunnerCache(12345);
        }

        [Fact]
        public void SetTheSelectionId()
        {
            Assert.Equal(12345, this.runnerCache.SelectionId);
        }

        [Fact]
        public void PublishTimeIsSet()
        {
            const long ExpectedTimeStamp = 98765;
            var runnerChange = new RunnerChangeStub();
            this.runnerCache.OnRunnerChange(runnerChange, ExpectedTimeStamp);

            Assert.Equal(98765, this.runnerCache.LastPublishTime);
        }

        [Fact]
        public void LastPublishTimeIsSetInTradeLadder()
        {
            const long ExpectedTimeStamp = 98765;
            var runnerChange = new RunnerChangeStub();
            this.runnerCache.OnRunnerChange(runnerChange, ExpectedTimeStamp);

            Assert.Equal(98765, this.runnerCache.TradedLadder.LastPublishTime);
        }

        [Fact]
        public void SetLastTradedPrice()
        {
            const int ExpectedLastTradedPrice = 10;
            var runnerChange = new RunnerChangeStub().WithLastTradedPrice(ExpectedLastTradedPrice);
            this.runnerCache.OnRunnerChange(runnerChange, 0);

            Assert.Equal(ExpectedLastTradedPrice, this.runnerCache.LastTradedPrice);
        }

        [Fact]
        public void OnlySetLastTradedPriceIfNotNull()
        {
            const int ExpectedLastTradedPrice = 10;

            var runnerChange1 = new RunnerChangeStub().WithLastTradedPrice(ExpectedLastTradedPrice);
            this.runnerCache.OnRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub();
            this.runnerCache.OnRunnerChange(runnerChange2, 0);

            Assert.Equal(ExpectedLastTradedPrice, this.runnerCache.LastTradedPrice);
        }

        [Fact]
        public void OnlyProcessMessageForCorrectRunner()
        {
            var runnerChange = new RunnerChangeStub()
                .WithSelectionId(54321)
                .WithLastTradedPrice(10);

            this.runnerCache.OnRunnerChange(runnerChange, 0);

            Assert.Null(this.runnerCache.LastTradedPrice);
        }

        [Fact]
        public void SetTotalMatched()
        {
            const int ExpectedTotalMatched = 10;
            var runnerChange = new RunnerChangeStub().WithTotalMatched(ExpectedTotalMatched);
            this.runnerCache.OnRunnerChange(runnerChange, 0);

            Assert.Equal(ExpectedTotalMatched, this.runnerCache.TotalMatched);
        }

        [Fact]
        public void OnlySetTotalMatchedIfNotZero()
        {
            const int ExpectedTotalMatched = 10;
            this.runnerCache.OnRunnerChange(new RunnerChangeStub().WithTotalMatched(ExpectedTotalMatched), 0);
            this.runnerCache.OnRunnerChange(new RunnerChangeStub(), 0);

            Assert.Equal(ExpectedTotalMatched, this.runnerCache.TotalMatched);
        }

        [Fact]
        public void SetBestAvailableToBack()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToBack(0, 2, 3)
                .WithBestAvailableToBack(1, 3, 4)
                .WithBestAvailableToBack(2, 4, 5);

            this.runnerCache.OnRunnerChange(runnerChange, 0);

            this.AssertBestAvailableToBackContains(0, 2, 3);
            this.AssertBestAvailableToBackContains(1, 3, 4);
            this.AssertBestAvailableToBackContains(2, 4, 5);
        }

        [Fact]
        public void UpdateBestAvailableToBack()
        {
            var runnerChange1 = new RunnerChangeStub()
                .WithBestAvailableToBack(0, 2, 3)
                .WithBestAvailableToBack(1, 3, 4)
                .WithBestAvailableToBack(2, 4, 5);
            this.runnerCache.OnRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub()
                .WithBestAvailableToBack(1, 5, 6);
            this.runnerCache.OnRunnerChange(runnerChange2, 0);

            this.AssertBestAvailableToBackContains(1, 5, 6);
        }

        [Fact]
        public void SetBestAvailableToLay()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToLay(0, 2, 3)
                .WithBestAvailableToLay(1, 3, 4)
                .WithBestAvailableToLay(2, 4, 5);

            this.runnerCache.OnRunnerChange(runnerChange, 0);

            this.AssertBestAvailableToLayContains(0, 2, 3);
            this.AssertBestAvailableToLayContains(1, 3, 4);
            this.AssertBestAvailableToLayContains(2, 4, 5);
        }

        [Fact]
        public void UpdateBestAvailableToLay()
        {
            var runnerChange1 = new RunnerChangeStub()
                .WithBestAvailableToLay(0, 2, 3)
                .WithBestAvailableToLay(1, 3, 4)
                .WithBestAvailableToLay(2, 4, 5);
            this.runnerCache.OnRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub().WithBestAvailableToLay(1, 5, 6);
            this.runnerCache.OnRunnerChange(runnerChange2, 0);

            this.AssertBestAvailableToLayContains(1, 5, 6);
        }

        [Fact]
        public void ProcessTraded()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(10, 100)
                .WithTraded(11, 10);
            this.runnerCache.OnRunnerChange(runnerChange, 0);

            Assert.Equal(100, this.runnerCache.TradedLadder.GetSizeForPrice(10));
            Assert.Equal(10, this.runnerCache.TradedLadder.GetSizeForPrice(11));
        }

        [Fact]
        public void UpdateTraded()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(10, 100)
                .WithTraded(11, 10);
            this.runnerCache.OnRunnerChange(runnerChange, 0);

            var runnerChange2 = new RunnerChangeStub().WithTraded(10, 50);
            this.runnerCache.OnRunnerChange(runnerChange2, 0);

            Assert.Equal(50, this.runnerCache.TradedLadder.GetSizeForPrice(10));
            Assert.Equal(10, this.runnerCache.TradedLadder.GetSizeForPrice(11));
        }

        [Fact]
        public void HandleNullTradedList()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(null, null);

            this.runnerCache.OnRunnerChange(runnerChange, 0);

            Assert.Equal(0, this.runnerCache.TradedLadder.GetSizeForPrice(10));
        }

        [Fact]
        public void HandleNullRunnerChange()
        {
            this.runnerCache.OnRunnerChange(null, 0);
        }

        [Fact]
        public void BestBackReturnsZeroIfLevelDoesNotExist()
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(1, 2.5, 100);
            this.runnerCache.OnRunnerChange(rc, 0);
            Assert.Equal(0, this.runnerCache.BestAvailableToBack.Price(0));
            Assert.Equal(0, this.runnerCache.BestAvailableToBack.Size(0));
        }

        [Fact]
        public void BestLayReturnsZeroIfLevelDoesNotExist()
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToLay(1, 2.5, 100);
            this.runnerCache.OnRunnerChange(rc, 0);
            Assert.Equal(0, this.runnerCache.BestAvailableToLay.Price(0));
            Assert.Equal(0, this.runnerCache.BestAvailableToLay.Size(0));
        }

        [Fact]
        public void SetAdjustmentFactorToZeroIfNull()
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToLay(1, 2.5, 100);
            this.runnerCache.OnRunnerChange(rc, 0);
            var d = new RunnerDefinition
            {
                SelectionId = 1,
            };

            this.runnerCache.SetDefinition(d);
            Assert.Equal(0, this.runnerCache.AdjustmentFactor);
        }

        [Fact]
        public void SetAdjustmentFactorOnRunnerDefinition()
        {
            var d = new RunnerDefinition
            {
                SelectionId = 12345,
                AdjustmentFactor = 23.45,
            };

            this.runnerCache.SetDefinition(d);
            Assert.Equal(23.45, this.runnerCache.AdjustmentFactor);
        }

        [Fact]
        public void HandleNullRunnerDefinition()
        {
            this.runnerCache.SetDefinition(null);
        }

        [Fact]
        public void HandleNullAdjustmentFactor()
        {
            var d = new RunnerDefinition
            {
                SelectionId = 12345,
            };

            this.runnerCache.SetDefinition(d);
            Assert.Equal(0, this.runnerCache.AdjustmentFactor);
        }

        [Fact]
        public void DoNotSetAdjustmentFactorIfWrongRunner()
        {
            var d = new RunnerDefinition
            {
                SelectionId = 2,
                AdjustmentFactor = 23.45,
            };

            this.runnerCache.SetDefinition(d);
            Assert.Equal(0, this.runnerCache.AdjustmentFactor);
        }

        [Theory]
        [InlineData(1.2, 10.99)]
        public void CalculateIfWinOnMatchedBacks(double price, double size)
        {
            var orc = new OrderRunnerChange
            {
                SelectionId = 12345,
                MatchedBacks = new List<List<double?>>
                {
                    new List<double?> { price, size },
                    new List<double?> { 10.5, 99.99 },
                },
            };

            this.runnerCache.OnOrderChange(orc);

            var ifWin = (price * size) - size;
            ifWin += (10.5 * 99.99) - 99.99;

            Assert.Equal(ifWin, this.runnerCache.IfWin);
        }

        private void AssertBestAvailableToBackContains(int level, double price, double size)
        {
            Assert.Equal(price, this.runnerCache.BestAvailableToBack.Price(level), 0);
            Assert.Equal(size, this.runnerCache.BestAvailableToBack.Size(level), 0);
        }

        private void AssertBestAvailableToLayContains(int level, double price, double size)
        {
            Assert.Equal(price, this.runnerCache.BestAvailableToLay.Price(level), 0);
            Assert.Equal(size, this.runnerCache.BestAvailableToLay.Size(level), 0);
        }
    }
}
