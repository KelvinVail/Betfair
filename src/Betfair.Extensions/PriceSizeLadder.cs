using System;
using System.Collections.Generic;
using System.Linq;

namespace Betfair.Extensions
{
    public class PriceSizeLadder
    {
        private readonly Dictionary<double, double> _ladder =
            new Dictionary<double, double>();

        public long? LastPublishTime { get; private set; }

        public void Update(List<List<double?>> priceSizes, long? unixMilliseconds)
        {
            LastPublishTime = unixMilliseconds;
            priceSizes?.ForEach(p => UpdatePrice(p[0], p[1]));
        }

        public double GetSizeForPrice(double price)
        {
            if (!_ladder.ContainsKey(price)) return 0;
            return _ladder[price];
        }

        public double TotalReturn()
        {
            return _ladder.Sum(
                price => Math.Round((price.Key * price.Value) - price.Value, 2));
        }

        public double TotalSize()
        {
            return _ladder.Sum(p => p.Value);
        }

        private void UpdatePrice(double? price, double? size)
        {
            if (price is null) return;
            if (size is null) return;

            var p = (double)price;
            var s = (double)size;

            if (!_ladder.ContainsKey(p))
                _ladder.Add(p, 0);

            _ladder[p] = s;
        }
    }
}
