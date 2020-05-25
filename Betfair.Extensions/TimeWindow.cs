namespace Betfair.Extensions
{
    using System.Collections.Generic;
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

        private class TimeBuffer
        {
            private readonly int millisSecondsToLive;

            private readonly Dictionary<long, double> buffer = new Dictionary<long, double>();

            public TimeBuffer(int secondsToLive)
            {
                this.millisSecondsToLive = secondsToLive * 1000;
            }

            public int Size { get; private set; }

            public IEnumerable<double> Push(long unixMilliseconds, double value)
            {
                if (!this.buffer.ContainsKey(unixMilliseconds))
                {
                    this.buffer.Add(unixMilliseconds, 0);
                    this.Size++;
                }

                this.buffer[unixMilliseconds] += value;

                return this.RemoveExpiredValues(unixMilliseconds);
            }

            private IEnumerable<double> RemoveExpiredValues(long unixMilliseconds)
            {
                var valuesRemoved = this.buffer
                    .Where(w => w.Key < unixMilliseconds - this.millisSecondsToLive)
                    .Select(s => s.Value)
                    .ToList();
                var keysToRemove = this.buffer
                    .Where(w => w.Key < unixMilliseconds - this.millisSecondsToLive)
                    .Select(s => s.Key)
                    .ToList();
                keysToRemove.ForEach(x => this.buffer.Remove(x));
                this.Size -= keysToRemove.Count;

                return valuesRemoved;
            }
        }
    }
}
