using System.Collections.Generic;
using System.Threading;
using Betfair.Betting;
using Betfair.Stream;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class StrategySpy : StrategyBase
    {
        private List<LimitOrder> _orders;

        public override MarketDataFilter DataFilter { get; } = new MarketDataFilter();

        public int MarketUpdateCount { get; private set; }

        public MarketChange LastMarketChange { get; private set; }

        public double Stake { get; private set; }

        public override List<LimitOrder> GetOrders(
            MarketChange marketChange, double stake)
        {
            MarketUpdateCount += 1;
            LastMarketChange = marketChange;
            Stake = stake;
            return _orders;
        }

        public StrategySpy WithOrder(LimitOrder o)
        {
            _orders ??= new List<LimitOrder>();
            _orders.Add(o);
            return this;
        }

        public string LinkedMarketId()
        {
            return Market.MarketId;
        }

        public int RunnerCount()
        {
            return Market.Runners.Count;
        }

        public long? LastPublishedTime()
        {
            return Market.LastPublishedTime;
        }

        public CancellationToken Token()
        {
            return CancellationToken;
        }
    }
}
