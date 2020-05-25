namespace Betfair.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PriceSizeLadder
    {
        private readonly Dictionary<double, double> ladder = new Dictionary<double, double>();

        public long LastPublishTime { get; private set; }

        public double TotalSize => this.ladder.Sum(s => s.Value);

        public double Vwap => this.ladder.Sum(s => s.Key * s.Value) / this.ladder.Sum(s => s.Value);

        public double MostTradedPrice => this.ladder.Where(w => Math.Abs(w.Value - this.ladder.Max(m => m.Value)) <= 0).Average(s => s.Key);

        public PriceSizeTimeWindow TenSecondWindow { get; } = new PriceSizeTimeWindow(10);

        public PriceSizeTimeWindow TwentySecondWindow { get; } = new PriceSizeTimeWindow(20);

        public PriceSizeTimeWindow ThirtySecondWindow { get; } = new PriceSizeTimeWindow(30);

        public void Update(List<List<double>> priceSizes, long unixMilliseconds)
        {
            this.LastPublishTime = unixMilliseconds;
            priceSizes?.ForEach(p => this.UpdatePrice(p[0], p[1]));
        }

        public double GetSizeForPrice(double price)
        {
            if (!this.ladder.ContainsKey(price)) return 0;
            return this.ladder[price];
        }

        private void UpdatePrice(double price, double size)
        {
            if (!this.ladder.ContainsKey(price))
                this.ladder.Add(price, 0);

            this.ladder[price] = size;
            this.TenSecondWindow.Update(price, size, this.LastPublishTime);
            this.TwentySecondWindow.Update(price, size, this.LastPublishTime);
            this.ThirtySecondWindow.Update(price, size, this.LastPublishTime);
        }
    }
}
