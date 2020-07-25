namespace Betfair.Extensions.Tests
{
    using System;
    using System.Linq;
    using Betfair.Extensions;
    using Betfair.Extensions.Tests.TestDoubles;
    using Betfair.Stream.Responses;
    using Xunit;

    public class RunnerCacheTests
    {
        private readonly RunnerCache runner;

        public RunnerCacheTests()
        {
            this.runner = new RunnerCache(12345);
        }

        [Fact]
        public void SetTheSelectionId()
        {
            Assert.Equal(12345, this.runner.SelectionId);
        }

        [Fact]
        public void PublishTimeIsSet()
        {
            const long ExpectedTimeStamp = 98765;
            var runnerChange = new RunnerChangeStub();
            this.runner.OnRunnerChange(runnerChange, ExpectedTimeStamp);

            Assert.Equal(98765, this.runner.LastPublishTime);
        }

        [Fact]
        public void LastPublishTimeIsSetInTradeLadder()
        {
            const long ExpectedTimeStamp = 98765;
            var runnerChange = new RunnerChangeStub();
            this.runner.OnRunnerChange(runnerChange, ExpectedTimeStamp);

            Assert.Equal(98765, this.runner.TradedLadder.LastPublishTime);
        }

        [Fact]
        public void SetLastTradedPrice()
        {
            const int ExpectedLastTradedPrice = 10;
            var runnerChange = new RunnerChangeStub()
                .WithLastTradedPrice(ExpectedLastTradedPrice);
            this.runner.OnRunnerChange(runnerChange, 0);

            Assert.Equal(ExpectedLastTradedPrice, this.runner.LastTradedPrice);
        }

        [Fact]
        public void OnlySetLastTradedPriceIfNotNull()
        {
            const int ExpectedLastTradedPrice = 10;

            var runnerChange1 = new RunnerChangeStub()
                .WithLastTradedPrice(ExpectedLastTradedPrice);
            this.runner.OnRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub();
            this.runner.OnRunnerChange(runnerChange2, 0);

            Assert.Equal(ExpectedLastTradedPrice, this.runner.LastTradedPrice);
        }

        [Fact]
        public void OnlyProcessMessageForCorrectRunner()
        {
            var runnerChange = new RunnerChangeStub()
                .WithSelectionId(54321)
                .WithLastTradedPrice(10);

            this.runner.OnRunnerChange(runnerChange, 0);

            Assert.Null(this.runner.LastTradedPrice);
        }

        [Fact]
        public void SetTotalMatched()
        {
            const int ExpectedTotalMatched = 10;
            var runnerChange = new RunnerChangeStub()
                .WithTotalMatched(ExpectedTotalMatched);
            this.runner.OnRunnerChange(runnerChange, 0);

            Assert.Equal(ExpectedTotalMatched, this.runner.TotalMatched);
        }

        [Fact]
        public void OnlySetTotalMatchedIfNotZero()
        {
            const int ExpectedTotalMatched = 10;
            this.runner.OnRunnerChange(
                new RunnerChangeStub().WithTotalMatched(ExpectedTotalMatched), 0);
            this.runner.OnRunnerChange(new RunnerChangeStub(), 0);

            Assert.Equal(ExpectedTotalMatched, this.runner.TotalMatched);
        }

        [Fact]
        public void SetBestAvailableToBack()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToBack(0, 2, 3)
                .WithBestAvailableToBack(1, 3, 4)
                .WithBestAvailableToBack(2, 4, 5);

            this.runner.OnRunnerChange(runnerChange, 0);

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
            this.runner.OnRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub()
                .WithBestAvailableToBack(1, 5, 6);
            this.runner.OnRunnerChange(runnerChange2, 0);

            this.AssertBestAvailableToBackContains(1, 5, 6);
        }

        [Fact]
        public void SetBestAvailableToLay()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToLay(0, 2, 3)
                .WithBestAvailableToLay(1, 3, 4)
                .WithBestAvailableToLay(2, 4, 5);

            this.runner.OnRunnerChange(runnerChange, 0);

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
            this.runner.OnRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub()
                .WithBestAvailableToLay(1, 5, 6);
            this.runner.OnRunnerChange(runnerChange2, 0);

            this.AssertBestAvailableToLayContains(1, 5, 6);
        }

        [Fact]
        public void ProcessTraded()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(10, 100)
                .WithTraded(11, 10);
            this.runner.OnRunnerChange(runnerChange, 0);

            Assert.Equal(100, this.runner.TradedLadder.GetSizeForPrice(10));
            Assert.Equal(10, this.runner.TradedLadder.GetSizeForPrice(11));
        }

        [Fact]
        public void UpdateTraded()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(10, 100)
                .WithTraded(11, 10);
            this.runner.OnRunnerChange(runnerChange, 0);

            var runnerChange2 = new RunnerChangeStub().WithTraded(10, 50);
            this.runner.OnRunnerChange(runnerChange2, 0);

            Assert.Equal(50, this.runner.TradedLadder.GetSizeForPrice(10));
            Assert.Equal(10, this.runner.TradedLadder.GetSizeForPrice(11));
        }

        [Fact]
        public void HandleNullTradedList()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(null, null);

            this.runner.OnRunnerChange(runnerChange, 0);

            Assert.Equal(0, this.runner.TradedLadder.GetSizeForPrice(10));
        }

        [Fact]
        public void HandleNullRunnerChange()
        {
            this.runner.OnRunnerChange(null, 0);
        }

        [Fact]
        public void BestBackReturnsZeroIfLevelDoesNotExist()
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1).WithBestAvailableToBack(1, 2.5, 100);
            this.runner.OnRunnerChange(rc, 0);
            Assert.Equal(0, this.runner.BestAvailableToBack.Price(0));
            Assert.Equal(0, this.runner.BestAvailableToBack.Size(0));
        }

        [Fact]
        public void BestLayReturnsZeroIfLevelDoesNotExist()
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1).WithBestAvailableToLay(1, 2.5, 100);
            this.runner.OnRunnerChange(rc, 0);
            Assert.Equal(0, this.runner.BestAvailableToLay.Price(0));
            Assert.Equal(0, this.runner.BestAvailableToLay.Size(0));
        }

        [Fact]
        public void SetAdjustmentFactorToZeroIfNull()
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1).WithBestAvailableToLay(1, 2.5, 100);
            this.runner.OnRunnerChange(rc, 0);
            var d = new RunnerDefinition
            {
                SelectionId = 1,
            };

            this.runner.SetDefinition(d);
            Assert.Equal(0, this.runner.AdjustmentFactor);
        }

        [Fact]
        public void SetAdjustmentFactorOnRunnerDefinition()
        {
            var d = new RunnerDefinition
            {
                SelectionId = 12345,
                AdjustmentFactor = 23.45,
            };

            this.runner.SetDefinition(d);
            Assert.Equal(23.45, this.runner.AdjustmentFactor);
        }

        [Fact]
        public void HandleNullRunnerDefinition()
        {
            this.runner.SetDefinition(null);
        }

        [Fact]
        public void HandleNullAdjustmentFactor()
        {
            var d = new RunnerDefinition
            {
                SelectionId = 12345,
            };

            this.runner.SetDefinition(d);
            Assert.Equal(0, this.runner.AdjustmentFactor);
        }

        [Fact]
        public void DoNotSetAdjustmentFactorIfWrongRunner()
        {
            var d = new RunnerDefinition
            {
                SelectionId = 2,
                AdjustmentFactor = 23.45,
            };

            this.runner.SetDefinition(d);
            Assert.Equal(0, this.runner.AdjustmentFactor);
        }

        [Fact]
        public void HandleNullOrderChange()
        {
            this.runner.OnOrderChange(null);
        }

        [Fact]
        public void HandleNullsInMatchedBacks()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(null, 10.99)
                .WithMatchedBack(1.01, null)
                .WithMatchedBack(null, null);
            this.runner.OnOrderChange(orc);
            Assert.Equal(0, this.runner.IfWin);
        }

        [Theory]
        [InlineData(1.2, 10.99)]
        [InlineData(20, 99.99)]
        [InlineData(1000, 0.10)]
        public void CalculateIfWinOnMatchedBacks(double price, double size)
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(price, size)
                .WithMatchedBack(10.5, 99.99);
            this.runner.OnOrderChange(orc);

            var ifWin = Math.Round((price * size) - size, 2);
            ifWin += Math.Round((10.5 * 99.99) - 99.99, 2);

            Assert.Equal(Math.Round(ifWin, 2), this.runner.IfWin);
        }

        [Theory]
        [InlineData(1.2, 10.99)]
        [InlineData(20, 99.99)]
        [InlineData(1000, 0.10)]
        public void CalculateIfLose(double price, double size)
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(price, size)
                .WithMatchedBack(10.5, 99.99);
            this.runner.OnOrderChange(orc);

            var ifLose = -Math.Round(size + 99.99, 2);

            Assert.Equal(ifLose, this.runner.IfLose);
        }

        [Fact]
        public void HandleNullsInMatchedLays()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedLay(null, 10.99)
                .WithMatchedLay(1.01, null)
                .WithMatchedLay(null, null);
            this.runner.OnOrderChange(orc);
            Assert.Equal(0, this.runner.IfWin);
        }

        [Theory]
        [InlineData(1.2, 10.99)]
        [InlineData(20, 99.99)]
        [InlineData(1000, 0.10)]
        public void CalculateIfWinOnMatchedLays(double price, double size)
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedLay(price, size)
                .WithMatchedLay(10.5, 99.99);
            this.runner.OnOrderChange(orc);

            var ifWin = -Math.Round((price * size) - size, 2);
            ifWin += -Math.Round((10.5 * 99.99) - 99.99, 2);

            Assert.Equal(Math.Round(ifWin, 2), this.runner.IfWin);
        }

        [Theory]
        [InlineData(1.2, 10.99)]
        [InlineData(20, 99.99)]
        [InlineData(1000, 0.10)]
        public void CalculateIfLoseOnMatchedLays(double price, double size)
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedLay(price, size)
                .WithMatchedLay(10.5, 99.99);
            this.runner.OnOrderChange(orc);

            var ifLose = Math.Round(size + 99.99, 2);

            Assert.Equal(ifLose, this.runner.IfLose);
        }

        [Fact]
        public void CalculateIfWinAndIfLoseForBacksAndLays()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(8, 2)
                .WithMatchedLay(8.8, 3);
            this.runner.OnOrderChange(orc);

            Assert.Equal(-9.4, this.runner.IfWin);
            Assert.Equal(1, this.runner.IfLose);
        }

        [Fact]
        public void DoNotProcessOrdersForOtherRunners()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(10.5, 99.99)
                .WithMatchedLay(10.5, 99.99);
            orc.SelectionId = 999;
            this.runner.OnOrderChange(orc);

            Assert.Equal(0, this.runner.IfWin);
            Assert.Equal(0, this.runner.IfLose);
        }

        [Fact]
        public void HandleNullUnmatchedBackSize()
        {
            var orc = new OrderRunnerChangeStub()
                .WithUnmatchedBack(null, 10.99)
                .WithUnmatchedBack(1.01, null)
                .WithUnmatchedBack(null, null);
            this.runner.OnOrderChange(orc);
        }

        [Theory]
        [InlineData(2.5, 10.99)]
        [InlineData(2.5, 2)]
        public void CalculateUnmatchedLiabilityForBacks(double price, double size)
        {
            var orc = new OrderRunnerChangeStub()
                .WithUnmatchedBack(price, size, "2")
                .WithUnmatchedBack(1.01, 10.99, "3");
            this.runner.OnOrderChange(orc);
            Assert.Equal(Math.Round(size + 10.99, 2), this.runner.UnmatchedLiability);
        }

        [Theory]
        [InlineData(2.5, 10.99)]
        [InlineData(2.5, 2)]
        public void CalculateUnmatchedLiabilityForLays(double price, double size)
        {
            var orc = new OrderRunnerChangeStub()
                .WithUnmatchedLay(price, size, "2")
                .WithUnmatchedLay(1.01, 10.99, "3");
            this.runner.OnOrderChange(orc);

            var expected = Math.Round((price * size) - size, 2);
            expected += Math.Round((1.01 * 10.99) - 10.99, 2);
            Assert.Equal(Math.Round(expected, 2), this.runner.UnmatchedLiability);
        }

        [Fact]
        public void ClearUnmatchedLiabilityBeforeEachUpdate()
        {
            var orc1 = new OrderRunnerChangeStub()
                .WithUnmatchedBack(2.5, 201.87);
            this.runner.OnOrderChange(orc1);
            var orc2 = new OrderRunnerChangeStub()
                .WithUnmatchedBack(1.01, 10.99);
            this.runner.OnOrderChange(orc2);
            Assert.Equal(10.99, this.runner.UnmatchedLiability);
        }

        [Theory]
        [InlineData("ABC", 12345, 2.5, 10.99)]
        [InlineData("123", 999, 1.01, 1.99)]
        [InlineData("XYZ", 024843, 1000, 0.01)]
        public void AddUnmatchedOrders(string betId, long? placedDate, double? price, double? sizeRemaining)
        {
            var orc = new OrderRunnerChangeStub()
                .WithUnmatchedBack(price, sizeRemaining, betId, placedDate);
            this.runner.OnOrderChange(orc);

            var uo = this.runner.UnmatchedOrders.First(o => o.BetId == betId);
            Assert.Equal(placedDate, uo.PlacedDate);
            Assert.Equal(price, uo.Price);
            Assert.Equal(sizeRemaining, uo.SizeRemaining);
        }

        [Theory]
        [InlineData("ABC", 12345, 2.5, 10.99)]
        [InlineData("123", 999, 1.01, 1.99)]
        [InlineData("XYZ", 024843, 1000, 0.01)]
        public void UnmatchedOrdersAreReplaced(string betId, long? placedDate, double? price, double? sizeRemaining)
        {
            var orc = new OrderRunnerChangeStub()
                .WithUnmatchedBack(price, sizeRemaining + 10, betId, placedDate);
            this.runner.OnOrderChange(orc);

            var orc2 = new OrderRunnerChangeStub()
                .WithUnmatchedBack(price, sizeRemaining, betId, placedDate);
            this.runner.OnOrderChange(orc2);

            Assert.Single(this.runner.UnmatchedOrders.Select(o => o.BetId == betId));
            var uo = this.runner.UnmatchedOrders.First(o => o.BetId == betId);
            Assert.Equal(sizeRemaining, uo.SizeRemaining);
        }

        [Theory]
        [InlineData("ABC", 12345, 2.5, 10.99)]
        [InlineData("123", 999, 1.01, 1.99)]
        [InlineData("XYZ", 024843, 1000, 0.01)]
        public void UnmatchedOrdersAreClearedIfComplete(string betId, long? placedDate, double? price, double? sizeRemaining)
        {
            var orc = new OrderRunnerChangeStub()
                .WithUnmatchedBack(price, sizeRemaining + 10, betId, placedDate);
            this.runner.OnOrderChange(orc);

            var orc2 = new OrderRunnerChangeStub()
                .WithUnmatchedBack(price, sizeRemaining + 10, betId, placedDate, "EC");
            this.runner.OnOrderChange(orc2);

            Assert.Empty(this.runner.UnmatchedOrders);
        }

        private void AssertBestAvailableToBackContains(
            int level, double price, double size)
        {
            Assert.Equal(price, this.runner.BestAvailableToBack.Price(level), 0);
            Assert.Equal(size, this.runner.BestAvailableToBack.Size(level), 0);
        }

        private void AssertBestAvailableToLayContains(
            int level, double price, double size)
        {
            Assert.Equal(price, this.runner.BestAvailableToLay.Price(level), 0);
            Assert.Equal(size, this.runner.BestAvailableToLay.Size(level), 0);
        }
    }
}
