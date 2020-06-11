namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using System.Threading;
    using Betfair.Betting;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public class StrategySpy : StrategyBase
    {
        private List<LimitOrder> orders;

        public override MarketDataFilter DataFilter { get; } = new MarketDataFilter();

        public override int RatioOfBankToUse { get; } = 1;

        public int MarketUpdateCount { get; private set; }

        public MarketChange LastMarketChange { get; private set; }

        public override List<LimitOrder> GetOrders(MarketChange marketChange, double stake)
        {
            this.MarketUpdateCount += 1;
            this.LastMarketChange = marketChange;
            return this.orders;
        }

        public StrategySpy WithOrder(LimitOrder o)
        {
            this.orders ??= new List<LimitOrder>();
            this.orders.Add(o);
            return this;
        }

        public string LinkedMarketId()
        {
            return this.Market.MarketId;
        }

        public int RunnerCount()
        {
            return this.Market.Runners.Count;
        }

        public long? LastPublishedTime()
        {
            return this.Market.LastPublishedTime;
        }

        public CancellationToken Token()
        {
            return this.CancellationToken;
        }
    }
}
