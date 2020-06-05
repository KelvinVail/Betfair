namespace Betfair.Extensions
{
    using System.Linq;

    public static class OverroundExtension
    {
        public static double Overround(this MarketCache market)
        {
            if (market is null) return 0;
            if (!market.Runners.Any()) return 0;
            return market.Runners.Sum(r =>
            {
                if (r.Value.BestAvailableToBack.Price(0) <= 0) return 0;
                return 1 / r.Value.BestAvailableToBack.Price(0);
            });
        }
    }
}
