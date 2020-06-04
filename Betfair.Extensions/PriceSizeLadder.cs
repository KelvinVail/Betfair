namespace Betfair.Extensions
{
    using System.Collections.Generic;

    public class PriceSizeLadder
    {
        private readonly Dictionary<double, double> ladder = new Dictionary<double, double>();

        public long? LastPublishTime { get; private set; }

        public void Update(List<List<double>> priceSizes, long? unixMilliseconds)
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
        }
    }
}
