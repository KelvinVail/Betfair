namespace Betfair.Betting
{
    using System;
    using System.Collections.Generic;

    public class Orders
    {
        private readonly List<LimitOrder> orders = new List<LimitOrder>();

        public Orders(string marketId)
        {
            this.MarketId = ValidateMarketId(marketId);
        }

        public Orders(string marketId, string strategyRef)
        {
            this.MarketId = ValidateMarketId(marketId);
            if (string.IsNullOrEmpty(strategyRef)) return;
            this.StrategyRef = ValidateStrategyReference(strategyRef);
        }

        public string StrategyRef { get; }

        public string MarketId { get; }

        public string ToParams()
        {
            return $"{{\"marketId\":\"{this.MarketId}\",{this.Reference()}\"instructions\":[{this.Instructions()}]}}";
        }

        public void Add(LimitOrder limitOrder)
        {
            this.orders.Add(limitOrder);
        }

        private static string ValidateStrategyReference(string strategyRef)
        {
            if (strategyRef.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(strategyRef), $"{strategyRef} must be less than 15 characters.");
            return strategyRef;
        }

        private static string ValidateMarketId(string marketId)
        {
            if (string.IsNullOrEmpty(marketId)) throw new ArgumentNullException(nameof(marketId), "MarketId should not be null or empty.");
            return marketId;
        }

        private string Instructions()
        {
            if (this.orders.Count == 0) return null;
            var instructions = string.Empty;
            this.orders.ForEach(i => instructions += i.ToInstruction() + ",");
            return instructions.Remove(instructions.Length - 1, 1);
        }

        private string Reference()
        {
            return this.StrategyRef is null ? null : $"\"customerStrategyRef\":\"{this.StrategyRef}\",";
        }
    }
}
