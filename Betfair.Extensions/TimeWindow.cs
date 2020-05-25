namespace Betfair.Extensions
{
    using System.Linq;

    public class TimeWindow
    {
        private readonly TimeBuffer buffer;

        private double total;

        private double mean;

        public TimeWindow(int secondsToLive)
        {
            this.buffer = new TimeBuffer(secondsToLive);
        }

        public void Update(long unixMilliseconds, double value)
        {
            var popped = this.buffer.Push(unixMilliseconds, value);
            this.total -= popped.Sum(s => s) - value;
            this.mean = this.total / this.buffer.Size;
        }

        public double Mean()
        {
            return this.mean;
        }

        public double Total()
        {
            return this.total;
        }
    }
}
