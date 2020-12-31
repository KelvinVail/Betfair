using System;
using System.Linq;
using Betfair.Extensions.Tests.TestDoubles;
using Betfair.Stream.Responses;
using Xunit;

namespace Betfair.Extensions.Tests
{
    public class RunnerCacheTests
    {
        private readonly RunnerCache _runner;

        public RunnerCacheTests()
        {
            _runner = new RunnerCache(12345);
        }

        [Fact]
        public void SetTheSelectionId()
        {
            Assert.Equal(12345, _runner.SelectionId);
        }

        [Fact]
        public void PublishTimeIsSet()
        {
            const long expectedTimeStamp = 98765;
            var runnerChange = new RunnerChangeStub();
            _runner.OnRunnerChange(runnerChange, expectedTimeStamp);

            Assert.Equal(98765, _runner.LastPublishTime);
        }

        [Fact]
        public void LastPublishTimeIsSetInTradeLadder()
        {
            const long expectedTimeStamp = 98765;
            var runnerChange = new RunnerChangeStub();
            _runner.OnRunnerChange(runnerChange, expectedTimeStamp);

            Assert.Equal(98765, _runner.TradedLadder.LastPublishTime);
        }

        [Fact]
        public void SetLastTradedPrice()
        {
            const int expectedLastTradedPrice = 10;
            var runnerChange = new RunnerChangeStub()
                .WithLastTradedPrice(expectedLastTradedPrice);
            _runner.OnRunnerChange(runnerChange, 0);

            Assert.Equal(expectedLastTradedPrice, _runner.LastTradedPrice);
        }

        [Fact]
        public void OnlySetLastTradedPriceIfNotNull()
        {
            const int expectedLastTradedPrice = 10;

            var runnerChange1 = new RunnerChangeStub()
                .WithLastTradedPrice(expectedLastTradedPrice);
            _runner.OnRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub();
            _runner.OnRunnerChange(runnerChange2, 0);

            Assert.Equal(expectedLastTradedPrice, _runner.LastTradedPrice);
        }

        [Fact]
        public void OnlyProcessMessageForCorrectRunner()
        {
            var runnerChange = new RunnerChangeStub()
                .WithSelectionId(54321)
                .WithLastTradedPrice(10);

            _runner.OnRunnerChange(runnerChange, 0);

            Assert.Null(_runner.LastTradedPrice);
        }

        [Fact]
        public void SetTotalMatched()
        {
            const int expectedTotalMatched = 10;
            var runnerChange = new RunnerChangeStub()
                .WithTotalMatched(expectedTotalMatched);
            _runner.OnRunnerChange(runnerChange, 0);

            Assert.Equal(expectedTotalMatched, _runner.TotalMatched);
        }

        [Fact]
        public void OnlySetTotalMatchedIfNotZero()
        {
            const int expectedTotalMatched = 10;
            _runner.OnRunnerChange(
                new RunnerChangeStub().WithTotalMatched(expectedTotalMatched), 0);
            _runner.OnRunnerChange(new RunnerChangeStub(), 0);

            Assert.Equal(expectedTotalMatched, _runner.TotalMatched);
        }

        [Fact]
        public void SetBestAvailableToBack()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToBack(0, 2, 3)
                .WithBestAvailableToBack(1, 3, 4)
                .WithBestAvailableToBack(2, 4, 5);

            _runner.OnRunnerChange(runnerChange, 0);

            AssertBestAvailableToBackContains(0, 2, 3);
            AssertBestAvailableToBackContains(1, 3, 4);
            AssertBestAvailableToBackContains(2, 4, 5);
        }

        [Fact]
        public void UpdateBestAvailableToBack()
        {
            var runnerChange1 = new RunnerChangeStub()
                .WithBestAvailableToBack(0, 2, 3)
                .WithBestAvailableToBack(1, 3, 4)
                .WithBestAvailableToBack(2, 4, 5);
            _runner.OnRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub()
                .WithBestAvailableToBack(1, 5, 6);
            _runner.OnRunnerChange(runnerChange2, 0);

            AssertBestAvailableToBackContains(1, 5, 6);
        }

        [Fact]
        public void SetBestAvailableToLay()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToLay(0, 2, 3)
                .WithBestAvailableToLay(1, 3, 4)
                .WithBestAvailableToLay(2, 4, 5);

            _runner.OnRunnerChange(runnerChange, 0);

            AssertBestAvailableToLayContains(0, 2, 3);
            AssertBestAvailableToLayContains(1, 3, 4);
            AssertBestAvailableToLayContains(2, 4, 5);
        }

        [Fact]
        public void UpdateBestAvailableToLay()
        {
            var runnerChange1 = new RunnerChangeStub()
                .WithBestAvailableToLay(0, 2, 3)
                .WithBestAvailableToLay(1, 3, 4)
                .WithBestAvailableToLay(2, 4, 5);
            _runner.OnRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub()
                .WithBestAvailableToLay(1, 5, 6);
            _runner.OnRunnerChange(runnerChange2, 0);

            AssertBestAvailableToLayContains(1, 5, 6);
        }

        [Fact]
        public void ProcessTraded()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(10, 100)
                .WithTraded(11, 10);
            _runner.OnRunnerChange(runnerChange, 0);

            Assert.Equal(100, _runner.TradedLadder.GetSizeForPrice(10));
            Assert.Equal(10, _runner.TradedLadder.GetSizeForPrice(11));
        }

        [Fact]
        public void UpdateTraded()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(10, 100)
                .WithTraded(11, 10);
            _runner.OnRunnerChange(runnerChange, 0);

            var runnerChange2 = new RunnerChangeStub().WithTraded(10, 50);
            _runner.OnRunnerChange(runnerChange2, 0);

            Assert.Equal(50, _runner.TradedLadder.GetSizeForPrice(10));
            Assert.Equal(10, _runner.TradedLadder.GetSizeForPrice(11));
        }

        [Fact]
        public void HandleNullTradedList()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(null, null);

            _runner.OnRunnerChange(runnerChange, 0);

            Assert.Equal(0, _runner.TradedLadder.GetSizeForPrice(10));
        }

        [Fact]
        public void HandleNullRunnerChange()
        {
            _runner.OnRunnerChange(null, 0);
        }

        [Fact]
        public void BestBackReturnsZeroIfLevelDoesNotExist()
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1).WithBestAvailableToBack(1, 2.5, 100);
            _runner.OnRunnerChange(rc, 0);
            Assert.Equal(0, _runner.BestAvailableToBack.Price(0));
            Assert.Equal(0, _runner.BestAvailableToBack.Size(0));
        }

        [Fact]
        public void BestLayReturnsZeroIfLevelDoesNotExist()
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1).WithBestAvailableToLay(1, 2.5, 100);
            _runner.OnRunnerChange(rc, 0);
            Assert.Equal(0, _runner.BestAvailableToLay.Price(0));
            Assert.Equal(0, _runner.BestAvailableToLay.Size(0));
        }

        [Fact]
        public void SetAdjustmentFactorToZeroIfNull()
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1).WithBestAvailableToLay(1, 2.5, 100);
            _runner.OnRunnerChange(rc, 0);
            var d = new RunnerDefinition
            {
                SelectionId = 1,
            };

            _runner.SetDefinition(d);
            Assert.Equal(0, _runner.AdjustmentFactor);
        }

        [Fact]
        public void SetAdjustmentFactorOnRunnerDefinition()
        {
            var d = new RunnerDefinition
            {
                SelectionId = 12345,
                AdjustmentFactor = 23.45,
            };

            _runner.SetDefinition(d);
            Assert.Equal(23.45, _runner.AdjustmentFactor);
        }

        [Fact]
        public void HandleNullRunnerDefinition()
        {
            _runner.SetDefinition(null);
        }

        [Fact]
        public void HandleNullAdjustmentFactor()
        {
            var d = new RunnerDefinition
            {
                SelectionId = 12345,
            };

            _runner.SetDefinition(d);
            Assert.Equal(0, _runner.AdjustmentFactor);
        }

        [Fact]
        public void DoNotSetAdjustmentFactorIfWrongRunner()
        {
            var d = new RunnerDefinition
            {
                SelectionId = 2,
                AdjustmentFactor = 23.45,
            };

            _runner.SetDefinition(d);
            Assert.Equal(0, _runner.AdjustmentFactor);
        }

        [Fact]
        public void HandleNullOrderChange()
        {
            _runner.OnOrderChange(null);
        }

        [Fact]
        public void HandleNullsInMatchedBacks()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(null, 10.99)
                .WithMatchedBack(1.01, null)
                .WithMatchedBack(null, null);
            _runner.OnOrderChange(orc);
            Assert.Equal(0, _runner.IfWin);
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
            _runner.OnOrderChange(orc);

            var ifWin = Math.Round((price * size) - size, 2);
            ifWin += Math.Round((10.5 * 99.99) - 99.99, 2);

            Assert.Equal(Math.Round(ifWin, 2), _runner.IfWin);
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
            _runner.OnOrderChange(orc);

            var ifLose = -Math.Round(size + 99.99, 2);

            Assert.Equal(ifLose, _runner.IfLose);
        }

        [Fact]
        public void HandleNullsInMatchedLays()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedLay(null, 10.99)
                .WithMatchedLay(1.01, null)
                .WithMatchedLay(null, null);
            _runner.OnOrderChange(orc);
            Assert.Equal(0, _runner.IfWin);
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
            _runner.OnOrderChange(orc);

            var ifWin = -Math.Round((price * size) - size, 2);
            ifWin += -Math.Round((10.5 * 99.99) - 99.99, 2);

            Assert.Equal(Math.Round(ifWin, 2), _runner.IfWin);
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
            _runner.OnOrderChange(orc);

            var ifLose = Math.Round(size + 99.99, 2);

            Assert.Equal(ifLose, _runner.IfLose);
        }

        [Fact]
        public void CalculateIfWinAndIfLoseForBacksAndLays()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(8, 2)
                .WithMatchedLay(8.8, 3);
            _runner.OnOrderChange(orc);

            Assert.Equal(-9.4, _runner.IfWin);
            Assert.Equal(1, _runner.IfLose);
        }

        [Fact]
        public void DoNotProcessOrdersForOtherRunners()
        {
            var orc = new OrderRunnerChangeStub()
                .WithMatchedBack(10.5, 99.99)
                .WithMatchedLay(10.5, 99.99);
            orc.SelectionId = 999;
            _runner.OnOrderChange(orc);

            Assert.Equal(0, _runner.IfWin);
            Assert.Equal(0, _runner.IfLose);
        }

        [Fact]
        public void HandleNullUnmatchedBackSize()
        {
            var orc = new OrderRunnerChangeStub()
                .WithUnmatchedBack(null, 10.99, "2")
                .WithUnmatchedBack(1.01, null, "3")
                .WithUnmatchedBack(null, null, "4");
            _runner.OnOrderChange(orc);
        }

        [Theory]
        [InlineData(2.5, 10.99)]
        [InlineData(2.5, 2)]
        public void CalculateUnmatchedLiabilityForBacks(double price, double size)
        {
            var orc = new OrderRunnerChangeStub()
                .WithUnmatchedBack(price, size, "2")
                .WithUnmatchedBack(1.01, 10.99, "3");
            _runner.OnOrderChange(orc);
            Assert.Equal(Math.Round(size + 10.99, 2), _runner.UnmatchedLiability);
        }

        [Theory]
        [InlineData(2.5, 10.99)]
        [InlineData(2.5, 2)]
        public void CalculateUnmatchedLiabilityForLays(double price, double size)
        {
            var orc = new OrderRunnerChangeStub()
                .WithUnmatchedLay(price, size, "2")
                .WithUnmatchedLay(1.01, 10.99, "3");
            _runner.OnOrderChange(orc);

            var expected = Math.Round((price * size) - size, 2);
            expected += Math.Round((1.01 * 10.99) - 10.99, 2);
            Assert.Equal(Math.Round(expected, 2), _runner.UnmatchedLiability);
        }

        [Fact]
        public void ClearUnmatchedLiabilityBeforeEachUpdate()
        {
            var orc1 = new OrderRunnerChangeStub()
                .WithUnmatchedBack(2.5, 201.87);
            _runner.OnOrderChange(orc1);
            var orc2 = new OrderRunnerChangeStub()
                .WithUnmatchedBack(1.01, 10.99);
            _runner.OnOrderChange(orc2);
            Assert.Equal(10.99, _runner.UnmatchedLiability);
        }

        [Theory]
        [InlineData("ABC", 12345, 2.5, 10.99)]
        [InlineData("123", 999, 1.01, 1.99)]
        [InlineData("XYZ", 024843, 1000, 0.01)]
        public void AddUnmatchedOrders(string betId, long? placedDate, double? price, double? sizeRemaining)
        {
            var orc = new OrderRunnerChangeStub()
                .WithUnmatchedBack(price, sizeRemaining, betId, placedDate);
            _runner.OnOrderChange(orc);

            var uo = _runner.UnmatchedOrders.First(o => o.BetId == betId);
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
            _runner.OnOrderChange(orc);

            var orc2 = new OrderRunnerChangeStub()
                .WithUnmatchedBack(price, sizeRemaining, betId, placedDate);
            _runner.OnOrderChange(orc2);

            Assert.Single(_runner.UnmatchedOrders.Select(o => o.BetId == betId));
            var uo = _runner.UnmatchedOrders.First(o => o.BetId == betId);
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
            _runner.OnOrderChange(orc);

            var orc2 = new OrderRunnerChangeStub()
                .WithUnmatchedBack(price, sizeRemaining + 10, betId, placedDate, "EC");
            _runner.OnOrderChange(orc2);

            Assert.Empty(_runner.UnmatchedOrders);
        }

        private void AssertBestAvailableToBackContains(
            int level, double price, double size)
        {
            Assert.Equal(price, _runner.BestAvailableToBack.Price(level), 0);
            Assert.Equal(size, _runner.BestAvailableToBack.Size(level), 0);
        }

        private void AssertBestAvailableToLayContains(
            int level, double price, double size)
        {
            Assert.Equal(price, _runner.BestAvailableToLay.Price(level), 0);
            Assert.Equal(size, _runner.BestAvailableToLay.Size(level), 0);
        }
    }
}
