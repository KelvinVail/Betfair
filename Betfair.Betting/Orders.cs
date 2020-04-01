namespace Betfair.Betting
{
    using System;

    public class Orders
    {
        public Orders()
        {
        }

        public Orders(string strategyRef)
        {
            if (string.IsNullOrEmpty(strategyRef)) return;
            ValidateStrategyReference(strategyRef);
            this.StrategyRef = strategyRef;
        }

        public string StrategyRef { get; }

        private static void ValidateStrategyReference(string strategyRef)
        {
            if (strategyRef.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(strategyRef), $"{strategyRef} must be less than 15 characters.");
        }
    }
}
