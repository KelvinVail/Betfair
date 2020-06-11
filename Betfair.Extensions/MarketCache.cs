namespace Betfair.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public double Liability =>
            Math.Round(
                this.Runners.Min(r => r.Value.Profit - r.Value.UnmatchedLiability), 2);

        public Dictionary<long, RunnerCache> Runners { get; private set; }
            = new Dictionary<long, RunnerCache>();

        public long? LastPublishedTime { get; private set; }

        public void OnChange(ChangeMessage change)
        {
            if (change is null) return;
            change.MarketChanges?.ForEach(mc => this.OnMarketChange(mc, change.PublishTime));
            change.OrderChanges?.ForEach(this.OnOrderChange);
        }

        private static bool ClearCache(MarketChange marketChange)
        {
            return marketChange.ReplaceCache != null && (bool)marketChange.ReplaceCache;
        }

        private void OnMarketChange(MarketChange marketChange, long? publishTime)
        {
            if (marketChange?.MarketId != this.MarketId) return;

            this.LastPublishedTime = publishTime;
            this.ProcessMarketChange(marketChange);
        }

        private void OnOrderChange(OrderChange orderChange)
        {
            if (orderChange is null) return;
            if (orderChange.MarketId != this.MarketId) return;

            orderChange.OrderRunnerChanges.ForEach(this.ProcessOrderRunnerChange);
            this.UpdateRunnerProfits();
        }

        private void UpdateRunnerProfits()
        {
            var totalIfLose = this.Runners.Sum(r => r.Value.IfLose);
            foreach (var (_, runner) in this.Runners)
            {
                runner.Profit =
                    Math.Round(
                        runner.IfWin + totalIfLose - runner.IfLose, 2);
            }
        }

        private void ProcessMarketChange(MarketChange marketChange)
        {
            if (ClearCache(marketChange)) this.NewCache();

            if (marketChange.MarketDefinition != null)
                this.MarketDefinition = marketChange.MarketDefinition;

            if (marketChange.TotalAmountMatched > 0)
                this.TotalAmountMatched = marketChange.TotalAmountMatched;

            this.ProcessRunnerChanges(marketChange.RunnerChanges);
            this.ProcessRunnerDefinitions(marketChange.MarketDefinition?.Runners);
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

            this.Runners[selectionId]
                .OnRunnerChange(runnerChange, this.LastPublishedTime);
        }

        private void ProcessRunnerDefinitions(List<RunnerDefinition> runners)
        {
            runners?.ForEach(this.ProcessRunnerDefinition);
        }

        private void ProcessRunnerDefinition(RunnerDefinition runner)
        {
            if (runner.SelectionId is null) return;
            if (this.Runners.ContainsKey((long)runner.SelectionId))
                this.Runners[(long)runner.SelectionId].SetDefinition(runner);
        }

        private void ProcessOrderRunnerChange(OrderRunnerChange orc)
        {
            if (orc.SelectionId is null) return;
            var selectionId = (long)orc.SelectionId;
            if (!this.Runners.ContainsKey(selectionId))
                this.Runners.Add(selectionId, new RunnerCache(selectionId));

            this.Runners[selectionId].OnOrderChange(orc);
        }

        private void NewCache()
        {
            this.Runners = new Dictionary<long, RunnerCache>();
            this.MarketDefinition = null;
            this.TotalAmountMatched = 0;
        }
    }
}
