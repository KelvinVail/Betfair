﻿namespace Betfair.Extensions
{
    using System.Collections.Generic;
    using Betfair.Stream.Responses;

    public sealed class MarketCache
    {
        public MarketCache(string marketId)
        {
            this.MarketId = marketId;
        }

        public string MarketId { get; }

        public MarketDefinition MarketDefinition { get; private set; }

        public double? TotalAmountMatched { get; private set; }

        public Dictionary<long, RunnerCache> Runners { get; private set; } = new Dictionary<long, RunnerCache>();

        public long? LastPublishedTime { get; private set; }

        public void OnMarketChange(MarketChange change, long? publishTime)
        {
            if (change?.MarketId != this.MarketId) return;

            this.LastPublishedTime = publishTime;
            this.ProcessMarketChange(change);
        }

        private static bool ClearCache(MarketChange marketChange)
        {
            return marketChange.ReplaceCache != null && (bool)marketChange.ReplaceCache;
        }

        private void ProcessMarketChange(MarketChange marketChange)
        {
            if (ClearCache(marketChange)) this.NewCache();

            if (marketChange.MarketDefinition != null) this.MarketDefinition = marketChange.MarketDefinition;

            if (marketChange.TotalAmountMatched > 0) this.TotalAmountMatched = marketChange.TotalAmountMatched;

            this.ProcessRunnerChanges(marketChange.RunnerChanges);
        }

        private void ProcessRunnerChanges(List<RunnerChange> runnerChanges)
        {
            runnerChanges?.ForEach(this.ProcessRunnerChange);
        }

        private void ProcessRunnerChange(RunnerChange runnerChange)
        {
            if (runnerChange.SelectionId is null) return;
            var selectionId = (long)runnerChange.SelectionId;
            if (!this.Runners.ContainsKey(selectionId))
                this.Runners.Add(selectionId, new RunnerCache(selectionId));

            this.Runners[selectionId].OnRunnerChange(runnerChange, this.LastPublishedTime);
        }

        private void NewCache()
        {
            this.Runners = new Dictionary<long, RunnerCache>();
            this.MarketDefinition = null;
            this.TotalAmountMatched = 0;
        }
    }
}