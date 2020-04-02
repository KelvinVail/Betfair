namespace Betfair.Betting
{
    using System;

    public class Orders
    {
        public Orders(string marketId)
        {
            this.MarketId = ValidateMarketId(marketId);
        }

        public Orders(string marketId, string strategyRef)
        {
            this.MarketId = ValidateMarketId(marketId);
            if (string.IsNullOrEmpty(strategyRef)) return;
            ValidateStrategyReference(strategyRef);
            this.StrategyRef = strategyRef;
        }

        public string StrategyRef { get; }

        public string MarketId { get; }

        private static void ValidateStrategyReference(string strategyRef)
        {
            if (strategyRef.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(strategyRef), $"{strategyRef} must be less than 15 characters.");
        }

        private static string ValidateMarketId(string marketId)
        {
            if (marketId is null) throw new ArgumentNullException(nameof(marketId), "MarketId should not be null or empty.");
            return marketId;
        }
    }
}
