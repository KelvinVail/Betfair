namespace Betfair.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PriceSizeLadder
    {
        private readonly Dictionary<double, double> ladder = 
            new Dictionary<double, double>();

        public long? LastPublishTime { get; private set; }

        public void Update(List<List<double?>> priceSizes, long? unixMilliseconds)
        {
            this.LastPublishTime = unixMilliseconds;
            priceSizes?.ForEach(p => this.UpdatePrice(p[0], p[1]));
        }

        public double GetSizeForPrice(double price)
        {
            if (!this.ladder.ContainsKey(price)) return 0;
            return this.ladder[price];
        }

        public double TotalReturn()
        {
            return this.ladder.Sum(
                price => Math.Round((price.Key * price.Value) - price.Value, 2));
        }

        public double TotalSize()
        {
            return this.ladder.Sum(p => p.Value);
        }

        private void UpdatePrice(double? price, double? size)
        {
            if (price is null) return;
            if (size is null) return;

            var p = (double)price;
            var s = (double)size;

            if (!this.ladder.ContainsKey(p))
                this.ladder.Add(p, 0);

            this.ladder[p] = s;
        }
    }
}
