namespace Betfair.Extensions.Tests
{
    using Betfair.Extensions;
    using Betfair.Extensions.Tests.TestDoubles;
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
            this.runnerCache.ProcessRunnerChange(runnerChange, ExpectedTimeStamp);

            Assert.Equal(98765, this.runnerCache.LastPublishTime);
        }

        [Fact]
        public void LastPublishTimeIsSetInTradeLadder()
        {
            const long ExpectedTimeStamp = 98765;
            var runnerChange = new RunnerChangeStub();
            this.runnerCache.ProcessRunnerChange(runnerChange, ExpectedTimeStamp);

            Assert.Equal(98765, this.runnerCache.TradedLadder.LastPublishTime);
        }

        [Fact]
        public void SetLastTradedPrice()
        {
            const int ExpectedLastTradedPrice = 10;
            var runnerChange = new RunnerChangeStub().WithLastTradedPrice(ExpectedLastTradedPrice);
            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Equal(ExpectedLastTradedPrice, this.runnerCache.LastTradedPrice);
        }

        [Fact]
        public void OnlySetLastTradedPriceIfNotNull()
        {
            const int ExpectedLastTradedPrice = 10;

            var runnerChange1 = new RunnerChangeStub().WithLastTradedPrice(ExpectedLastTradedPrice);
            this.runnerCache.ProcessRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub();
            this.runnerCache.ProcessRunnerChange(runnerChange2, 0);

            Assert.Equal(ExpectedLastTradedPrice, this.runnerCache.LastTradedPrice);
        }

        [Fact]
        public void OnlyProcessMessageForCorrectRunner()
        {
            var runnerChange = new RunnerChangeStub()
                .WithSelectionId(54321)
                .WithLastTradedPrice(10);

            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Null(this.runnerCache.LastTradedPrice);
        }

        [Fact]
        public void SetTotalMatched()
        {
            const int ExpectedTotalMatched = 10;
            var runnerChange = new RunnerChangeStub().WithTotalMatched(ExpectedTotalMatched);
            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Equal(ExpectedTotalMatched, this.runnerCache.TotalMatched);
        }

        [Fact]
        public void OnlySetTotalMatchedIfNotZero()
        {
            const int ExpectedTotalMatched = 10;
            this.runnerCache.ProcessRunnerChange(new RunnerChangeStub().WithTotalMatched(ExpectedTotalMatched), 0);
            this.runnerCache.ProcessRunnerChange(new RunnerChangeStub(), 0);

            Assert.Equal(ExpectedTotalMatched, this.runnerCache.TotalMatched);
        }

        [Fact]
        public void SetBestAvailableToBack()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToBack(0, 2, 3)
                .WithBestAvailableToBack(1, 3, 4)
                .WithBestAvailableToBack(2, 4, 5);

            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            this.AssertBestAvailableToBackContains(1, 2, 3);
            this.AssertBestAvailableToBackContains(2, 3, 4);
            this.AssertBestAvailableToBackContains(3, 4, 5);
        }

        [Fact]
        public void UpdateBestAvailableToBack()
        {
            var runnerChange1 = new RunnerChangeStub()
                .WithBestAvailableToBack(0, 2, 3)
                .WithBestAvailableToBack(1, 3, 4)
                .WithBestAvailableToBack(2, 4, 5);
            this.runnerCache.ProcessRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub()
                .WithBestAvailableToBack(1, 5, 6);
            this.runnerCache.ProcessRunnerChange(runnerChange2, 0);

            this.AssertBestAvailableToBackContains(2, 5, 6);
        }

        [Fact]
        public void SetBestAvailableToLay()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToLay(0, 2, 3)
                .WithBestAvailableToLay(1, 3, 4)
                .WithBestAvailableToLay(2, 4, 5);

            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            this.AssertBestAvailableToLayContains(1, 2, 3);
            this.AssertBestAvailableToLayContains(2, 3, 4);
            this.AssertBestAvailableToLayContains(3, 4, 5);
        }

        [Fact]
        public void UpdateBestAvailableToLay()
        {
            var runnerChange1 = new RunnerChangeStub()
                .WithBestAvailableToLay(0, 2, 3)
                .WithBestAvailableToLay(1, 3, 4)
                .WithBestAvailableToLay(2, 4, 5);
            this.runnerCache.ProcessRunnerChange(runnerChange1, 0);

            var runnerChange2 = new RunnerChangeStub().WithBestAvailableToLay(1, 5, 6);
            this.runnerCache.ProcessRunnerChange(runnerChange2, 0);

            this.AssertBestAvailableToLayContains(2, 5, 6);
        }

        [Fact]
        public void CalculateTotalSizeAvailableToBack()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToBack(0, 2, 4)
                .WithBestAvailableToBack(1, 3, 4)
                .WithBestAvailableToBack(2, 4, 5);

            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Equal(13, this.runnerCache.TotalSizeAvailableToBack);
        }

        [Fact]
        public void CalculateTotalSizeAvailableToLay()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToLay(0, 2, 3)
                .WithBestAvailableToLay(1, 3, 4)
                .WithBestAvailableToLay(2, 4, 5);

            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Equal(12, this.runnerCache.TotalSizeAvailableToLay);
        }

        [Fact]
        public void CalculateInnerWom()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToBack(0, 2, 10)
                .WithBestAvailableToLay(0, 2, 90);

            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Equal(0.1, this.runnerCache.InnerWom);
        }

        [Fact]
        public void CalculateOuterWom()
        {
            var runnerChange = new RunnerChangeStub()
                .WithBestAvailableToBack(0, 2, 10)
                .WithBestAvailableToBack(1, 2, 11)
                .WithBestAvailableToBack(2, 2, 12)
                .WithBestAvailableToLay(0, 2, 90)
                .WithBestAvailableToLay(1, 2, 95)
                .WithBestAvailableToLay(2, 3, 100);

            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Equal(0.10377, this.runnerCache.OuterWom, 5);
        }

        [Fact]
        public void ProcessTraded()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(10, 100)
                .WithTraded(11, 10);
            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Equal(110.0, this.runnerCache.TradedLadder.TotalSize);
        }

        [Fact]
        public void UpdateTraded()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(10, 100)
                .WithTraded(11, 10);
            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            var runnerChange2 = new RunnerChangeStub().WithTraded(10, 50);
            this.runnerCache.ProcessRunnerChange(runnerChange2, 0);

            Assert.Equal(60.0, this.runnerCache.TradedLadder.TotalSize);
        }

        [Fact]
        public void CalculateVwap()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(10, 100)
                .WithTraded(11, 10);
            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Equal(10.09, this.runnerCache.TradedLadder.Vwap, 2);
        }

        [Fact]
        public void CalculateMostTradedPrice()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(10, 100)
                .WithTraded(11, 10);

            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Equal(10, this.runnerCache.TradedLadder.PriceWithMostSize);
        }

        [Fact]
        public void HandleNullTradedList()
        {
            var runnerChange = new RunnerChangeStub()
                .WithTraded(null, null);

            this.runnerCache.ProcessRunnerChange(runnerChange, 0);

            Assert.Equal(0, this.runnerCache.TradedLadder.TotalSize);
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
