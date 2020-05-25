namespace Betfair.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TimeBuffer
    {
        private readonly int millisSecondsToLive;

        private readonly Dictionary<long, double> buffer = new Dictionary<long, double>();

        public TimeBuffer(int secondsToLive)
        {
            this.millisSecondsToLive = secondsToLive * 1000;
        }

        public int Size { get; private set; }

        public List<double> Push(long unixMilliseconds, double value)
        {
            if (!this.buffer.ContainsKey(unixMilliseconds))
            {
                this.buffer.Add(unixMilliseconds, 0);
                this.Size++;
            }

            this.buffer[unixMilliseconds] += value;

            return this.RemoveExpiredValues(unixMilliseconds);
        }

        public bool Contains(long unixMilliseconds, double value)
        {
            return this.buffer.ContainsKey(unixMilliseconds) && Math.Abs(this.buffer[unixMilliseconds] - value) <= 0;
        }

        private List<double> RemoveExpiredValues(long unixMilliseconds)
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
