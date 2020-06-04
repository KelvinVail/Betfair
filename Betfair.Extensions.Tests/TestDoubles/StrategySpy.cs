namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public class StrategySpy : StrategyBase
    {
        public override MarketDataFilter DataFilter { get; } = new MarketDataFilter();

        public int MarketUpdateCount { get; private set; }

        public MarketChange LastMarketChange { get; private set; }

        public override async Task OnMarketUpdate(MarketChange marketChange)
        {
            this.MarketUpdateCount += 1;
            this.LastMarketChange = marketChange;
            await Task.CompletedTask;
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

        public CancellationToken GetCancellationToken()
        {
            return this.CancellationToken;
        }
    }
}
