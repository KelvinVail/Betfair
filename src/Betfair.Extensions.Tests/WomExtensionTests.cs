namespace Betfair.Extensions.Tests
{
    using Betfair.Extensions.Tests.TestDoubles;
    using Xunit;

    public class WomExtensionTests
    {
        private readonly RunnerCache runner = new RunnerCache(1);

        [Fact]
        public void ReturnZeroIfRunnerIsEmpty()
        {
            Assert.Equal(0, this.runner.Wom());
        }

        [Fact]
        public void ReturnZeroIfNull()
        {
            Assert.Equal(0, WomExtension.Wom(null));
        }

        [Theory]
        [InlineData(100, 10.99)]
        [InlineData(2.75, 20567.01)]
        [InlineData(98.2, 1)]
        public void CalculateWom(double backSize, double laySize)
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1)
                .WithBestAvailableToBack(0, 2.5, backSize)
                .WithBestAvailableToLay(0, 3, laySize);
            this.runner.OnRunnerChange(rc, 0);

            var expected = backSize / (backSize + laySize);

            Assert.Equal(expected, this.runner.Wom());
        }

        [Fact]
        public void CalculateWomWhenThereIsNoLaySize()
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 10.99);
            this.runner.OnRunnerChange(rc, 0);

            Assert.Equal(1, this.runner.Wom());
        }

        [Theory]
        [InlineData(100, 37, 10.99, 100)]
        [InlineData(2.75, 12, 20567.01, 18)]
        [InlineData(98.2, 99, 1, 1000)]
        public void CalculateWomForTwoLevels(double backSize1, double backSize2, double laySize1, double laySize2)
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1)
                .WithBestAvailableToBack(0, 2.5, backSize1)
                .WithBestAvailableToBack(1, 2.5, backSize2)
                .WithBestAvailableToLay(0, 3, laySize1)
                .WithBestAvailableToLay(1, 3, laySize2);
            this.runner.OnRunnerChange(rc, 0);

            var expected = (backSize1 + backSize2) / (backSize1 + backSize2 + laySize1 + laySize2);

            Assert.Equal(expected, this.runner.Wom(2));
        }

        [Theory]
        [InlineData(100, 37, 10.99, 100)]
        [InlineData(2.75, 12, 20567.01, 18)]
        [InlineData(98.2, 99, 1, 1000)]
        public void CalculateWomForOneLevel(double backSize1, double backSize2, double laySize1, double laySize2)
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1)
                .WithBestAvailableToBack(0, 2.5, backSize1)
                .WithBestAvailableToBack(1, 2.5, backSize2)
                .WithBestAvailableToLay(0, 3, laySize1)
                .WithBestAvailableToLay(1, 3, laySize2);
            this.runner.OnRunnerChange(rc, 0);

            var expected = backSize1 / (backSize1 + laySize1);

            Assert.Equal(expected, this.runner.Wom());
        }

        [Theory]
        [InlineData(100, 37, 19, 10.99, 100, 20)]
        [InlineData(2.75, 12, 20, 20567.01, 18, 1)]
        [InlineData(98.2, 99, 87, 1, 1000, 77)]
        public void CalculateWomForThreeLevels(double backSize1, double backSize2, double backSize3, double laySize1, double laySize2, double laySize3)
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1)
                .WithBestAvailableToBack(0, 2.5, backSize1)
                .WithBestAvailableToBack(1, 2.5, backSize2)
                .WithBestAvailableToBack(2, 2.5, backSize3)
                .WithBestAvailableToLay(0, 3, laySize1)
                .WithBestAvailableToLay(1, 3, laySize2)
                .WithBestAvailableToLay(2, 3, laySize3);
            this.runner.OnRunnerChange(rc, 0);

            var totalBackSize = backSize1 + backSize2 + backSize3;
            var expected = totalBackSize / (totalBackSize + laySize1 + laySize2 + laySize3);

            Assert.Equal(expected, this.runner.Wom(3));
        }
    }
}
