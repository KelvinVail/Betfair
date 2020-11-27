namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using Betfair.Stream.Responses;

    public class MarketChangeStub : MarketChange
    {
        public MarketChangeStub()
        {
            this.MarketId = "1.2345";
        }

        public MarketChangeStub WithMarketId(string marketId)
        {
            this.MarketId = marketId;
            return this;
        }

        public MarketChangeStub WithMarketDefinition(MarketDefinition marketDefinition)
        {
            this.MarketDefinition = marketDefinition;
            return this;
        }

        public MarketChangeStub WithTotalMatched(double totalMatched)
        {
            this.TotalAmountMatched = totalMatched;
            return this;
        }

        public MarketChangeStub WithRunnerChange(RunnerChange runnerChange)
        {
            this.RunnerChanges ??= new List<RunnerChange>();
            this.RunnerChanges.Add(runnerChange);
            return this;
        }

        public MarketChangeStub WithReplaceCache()
        {
            this.ReplaceCache = true;
            return this;
        }
    }
}
