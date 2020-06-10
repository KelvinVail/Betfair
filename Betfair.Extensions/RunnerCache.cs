namespace Betfair.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Betfair.Stream.Responses;

    public sealed class RunnerCache
    {
        private readonly PriceSizeLadder matchedBacks = new PriceSizeLadder();

        private readonly PriceSizeLadder matchedLays = new PriceSizeLadder();

        public RunnerCache(long selectionId)
        {
            this.SelectionId = selectionId;
        }

        public long SelectionId { get; }

        public double? LastTradedPrice { get; private set; }

        public double? TotalMatched { get; private set; }

        public LevelLadder BestAvailableToBack { get; } = new LevelLadder();

        public LevelLadder BestAvailableToLay { get; } = new LevelLadder();

        public PriceSizeLadder TradedLadder { get; } = new PriceSizeLadder();

        public long? LastPublishTime { get; private set; }

        public double AdjustmentFactor { get; private set; }

        public double IfWin =>
            Math.Round(
                this.matchedBacks.TotalReturn() - this.matchedLays.TotalReturn(), 2);

        public double IfLose =>
            Math.Round(
                this.matchedLays.TotalSize() - this.matchedBacks.TotalSize(), 2);

        public double Profit { get; internal set; }

        public double UnmatchedLiability { get; private set; }

        public void OnRunnerChange(RunnerChange runnerChange, long? lastUpdated)
        {
            if (runnerChange?.SelectionId != this.SelectionId) return;

            this.LastPublishTime = lastUpdated;
            this.SetLastTradedPrice(runnerChange);
            this.SetTotalMatched(runnerChange);
            this.ProcessBestAvailableToBack(runnerChange.BestAvailableToBack);
            this.ProcessBestAvailableToLay(runnerChange.BestAvailableToLay);
            this.UpdateTradedLadder(runnerChange);
        }

        public void OnOrderChange(OrderRunnerChange orc)
        {
            if (orc is null) return;
            if (orc.SelectionId != this.SelectionId) return;
            this.matchedBacks.Update(orc.MatchedBacks, 0);
            this.matchedLays.Update(orc.MatchedLays, 0);
            this.UnmatchedLiability = 0;
            orc.UnmatchedOrders?.ForEach(this.UpdateUnmatchedLiability);
        }

        public void SetDefinition(RunnerDefinition definition)
        {
            if (definition?.SelectionId != this.SelectionId) return;
            if (definition.AdjustmentFactor is null) return;
            this.AdjustmentFactor = (double)definition.AdjustmentFactor;
        }

        private void UpdateUnmatchedLiability(UnmatchedOrder uo)
        {
            if (uo.SizeRemaining is null) return;
            var sr = (double)uo.SizeRemaining;

            if (uo.Price is null) return;
            var price = (double)uo.Price;

            switch (uo.Side)
            {
                case "B":
                    this.UnmatchedLiability += sr;
                    break;
                case "L":
                    this.UnmatchedLiability += Math.Round((price * sr) - sr, 2);
                    break;
            }
        }

        private void UpdateTradedLadder(RunnerChange runnerChange)
        {
            this.TradedLadder.Update(
                runnerChange.Traded, this.LastPublishTime);
        }

        private void SetTotalMatched(RunnerChange runnerChange)
        {
            this.TotalMatched = runnerChange.TotalMatched ?? this.TotalMatched;
        }

        private void SetLastTradedPrice(RunnerChange runnerChange)
        {
            this.LastTradedPrice = runnerChange.LastTradedPrice ?? this.LastTradedPrice;
        }

        private void ProcessBestAvailableToBack(List<List<double?>> availableToBacks)
        {
            availableToBacks?.ForEach(this.UpdateBestAvailableToBack);
        }

        private void UpdateBestAvailableToBack(List<double?> availableToBack)
        {
            this.BestAvailableToBack.ProcessLevel(
                availableToBack.Select(d => d ?? 0).ToList());
        }

        private void ProcessBestAvailableToLay(List<List<double?>> availableToLays)
        {
            availableToLays?.ForEach(this.UpdateBestAvailableToLay);
        }

        private void UpdateBestAvailableToLay(List<double?> availableToLay)
        {
            this.BestAvailableToLay.ProcessLevel(
                availableToLay.Select(d => d ?? 0).ToList());
        }
    }
}
