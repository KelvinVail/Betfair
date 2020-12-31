using System;
using System.Collections.Generic;
using System.Linq;
using Betfair.Stream.Responses;

namespace Betfair.Extensions
{
    public sealed class MarketCache
    {
        public MarketCache(string marketId)
        {
            MarketId = marketId;
        }

        public string MarketId { get; }

        public MarketDefinition MarketDefinition { get; private set; }

        public double? TotalAmountMatched { get; private set; }

        public double Liability => Runners.Count == 0 ? 0 :
            Math.Round(
                Runners.Min(r => r.Value.Profit - r.Value.UnmatchedLiability), 2);

        public Dictionary<long, RunnerCache> Runners { get; private set; }
            = new Dictionary<long, RunnerCache>();

        public long? LastPublishedTime { get; private set; }

        public void OnChange(ChangeMessage change)
        {
            if (change is null) return;
            change.MarketChanges?.ForEach(mc => OnMarketChange(mc, change.PublishTime));
            change.OrderChanges?.ForEach(OnOrderChange);
        }

        private static bool ClearCache(MarketChange marketChange)
        {
            return marketChange.ReplaceCache != null && (bool)marketChange.ReplaceCache;
        }

        private void OnMarketChange(MarketChange marketChange, long? publishTime)
        {
            if (marketChange?.MarketId != MarketId) return;

            LastPublishedTime = publishTime;
            ProcessMarketChange(marketChange);
        }

        private void OnOrderChange(OrderChange orderChange)
        {
            if (orderChange is null) return;
            if (orderChange.MarketId != MarketId) return;

            orderChange.OrderRunnerChanges.ForEach(ProcessOrderRunnerChange);
            UpdateRunnerProfits();
        }

        private void UpdateRunnerProfits()
        {
            var totalIfLose = Runners.Sum(r => r.Value.IfLose);
            foreach (var (_, runner) in Runners)
            {
                runner.Profit =
                    Math.Round(
                        runner.IfWin + totalIfLose - runner.IfLose, 2);
            }
        }

        private void ProcessMarketChange(MarketChange marketChange)
        {
            if (ClearCache(marketChange)) NewCache();

            if (marketChange.MarketDefinition != null)
                MarketDefinition = marketChange.MarketDefinition;

            if (marketChange.TotalAmountMatched > 0)
                TotalAmountMatched = marketChange.TotalAmountMatched;

            ProcessRunnerChanges(marketChange.RunnerChanges);
            ProcessRunnerDefinitions(marketChange.MarketDefinition?.Runners);
        }

        private void ProcessRunnerChanges(List<RunnerChange> runnerChanges)
        {
            runnerChanges?.ForEach(ProcessRunnerChange);
        }

        private void ProcessRunnerChange(RunnerChange runnerChange)
        {
            if (runnerChange.SelectionId is null) return;
            var selectionId = (long)runnerChange.SelectionId;
            if (!Runners.ContainsKey(selectionId))
                Runners.Add(selectionId, new RunnerCache(selectionId));

            Runners[selectionId]
                .OnRunnerChange(runnerChange, LastPublishedTime);
        }

        private void ProcessRunnerDefinitions(List<RunnerDefinition> runners)
        {
            runners?.ForEach(ProcessRunnerDefinition);
        }

        private void ProcessRunnerDefinition(RunnerDefinition runner)
        {
            if (runner.SelectionId is null) return;
            if (Runners.ContainsKey((long)runner.SelectionId))
                Runners[(long)runner.SelectionId].SetDefinition(runner);
        }

        private void ProcessOrderRunnerChange(OrderRunnerChange orc)
        {
            if (orc.SelectionId is null) return;
            var selectionId = (long)orc.SelectionId;
            if (!Runners.ContainsKey(selectionId))
                Runners.Add(selectionId, new RunnerCache(selectionId));

            Runners[selectionId].OnOrderChange(orc);
        }

        private void NewCache()
        {
            Runners = new Dictionary<long, RunnerCache>();
            MarketDefinition = null;
            TotalAmountMatched = 0;
        }
    }
}
