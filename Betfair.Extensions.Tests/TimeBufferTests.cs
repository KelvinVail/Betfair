namespace Betfair.Extensions.Tests
{
    using Xunit;

    public class TimeBufferTests
    {
        private readonly TimeBuffer timeBuffer = new TimeBuffer(60);

        [Fact]
        public void AddValue()
        {
            this.timeBuffer.Push(1000, 1.23);
            Assert.True(this.timeBuffer.Contains(1000, 1.23));
            Assert.False(this.timeBuffer.Contains(1001, 1.23));
        }

        [Fact]
        public void ValuesRemovedWhenExpired()
        {
            this.timeBuffer.Push(1000, 1);
            this.timeBuffer.Push(70000, 2);
            Assert.False(this.timeBuffer.Contains(1000, 1));
        }

        [Fact]
        public void ReturnPoppedValues()
        {
            this.timeBuffer.Push(1, 1);
            this.timeBuffer.Push(2, 2);
            this.timeBuffer.Push(6, 6);
            var popped = this.timeBuffer.Push(60005, 2);
            Assert.Contains(1, popped);
            Assert.Contains(2, popped);
            Assert.True(this.timeBuffer.Contains(6, 6));
        }

        [Fact]
        public void KeepsCountOfValues()
        {
            this.timeBuffer.Push(1, 1);
            this.timeBuffer.Push(2, 2);
            this.timeBuffer.Push(6, 6);
            this.timeBuffer.Push(60005, 2);

            Assert.Equal(2, this.timeBuffer.Size);
        }
    }
}
