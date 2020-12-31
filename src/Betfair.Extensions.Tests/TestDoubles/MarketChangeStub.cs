using System.Collections.Generic;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class MarketChangeStub : MarketChange
    {
        public MarketChangeStub()
        {
            MarketId = "1.2345";
        }

        public MarketChangeStub WithMarketId(string marketId)
        {
            MarketId = marketId;
            return this;
        }

        public MarketChangeStub WithMarketDefinition(MarketDefinition marketDefinition)
        {
            MarketDefinition = marketDefinition;
            return this;
        }

        public MarketChangeStub WithTotalMatched(double totalMatched)
        {
            TotalAmountMatched = totalMatched;
            return this;
        }

        public MarketChangeStub WithRunnerChange(RunnerChange runnerChange)
        {
            RunnerChanges ??= new List<RunnerChange>();
            RunnerChanges.Add(runnerChange);
            return this;
        }

        public MarketChangeStub WithReplaceCache()
        {
            ReplaceCache = true;
            return this;
        }
    }
}
