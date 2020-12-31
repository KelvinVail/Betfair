using System;
using System.Collections.Generic;
using System.Linq;
using Betfair.Stream.Responses;

namespace Betfair.Extensions
{
    public sealed class RunnerCache
    {
        private readonly PriceSizeLadder _matchedBacks = new PriceSizeLadder();
        private readonly PriceSizeLadder _matchedLays = new PriceSizeLadder();
        private readonly UnmatchedOrders _unmatchedOrders = new UnmatchedOrders();

        public RunnerCache(long selectionId)
        {
            SelectionId = selectionId;
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
                _matchedBacks.TotalReturn() - _matchedLays.TotalReturn(), 2);

        public double IfLose =>
            Math.Round(
                _matchedLays.TotalSize() - _matchedBacks.TotalSize(), 2);

        public double Profit { get; internal set; }

        public double UnmatchedLiability { get; private set; }

        public IList<UnmatchedOrder> UnmatchedOrders => _unmatchedOrders.ToList();

        public void OnRunnerChange(RunnerChange runnerChange, long? lastUpdated)
        {
            if (runnerChange?.SelectionId != SelectionId) return;

            LastPublishTime = lastUpdated;
            SetLastTradedPrice(runnerChange);
            SetTotalMatched(runnerChange);
            ProcessBestAvailableToBack(runnerChange.BestAvailableToBack);
            ProcessBestAvailableToLay(runnerChange.BestAvailableToLay);
            UpdateTradedLadder(runnerChange);
        }

        public void OnOrderChange(OrderRunnerChange orc)
        {
            if (orc is null) return;
            if (orc.SelectionId != SelectionId) return;
            _matchedBacks.Update(orc.MatchedBacks, 0);
            _matchedLays.Update(orc.MatchedLays, 0);
            UnmatchedLiability = 0;
            orc.UnmatchedOrders?.ForEach(o => _unmatchedOrders.Update(o));
            UnmatchedOrders.ToList().ForEach(UpdateUnmatchedLiability);
        }

        public void SetDefinition(RunnerDefinition definition)
        {
            if (definition?.SelectionId != SelectionId) return;
            if (definition.AdjustmentFactor is null) return;
            AdjustmentFactor = (double)definition.AdjustmentFactor;
        }

        private void UpdateUnmatchedLiability(UnmatchedOrder uo)
        {
            if (uo.SizeRemaining is null) return;
            var sr = (double)uo.SizeRemaining;

            if (uo.Price is null) return;
            var price = (double)uo.Price;

            UnmatchedLiability +=
                uo.Side == "B" ? sr : Math.Round((price * sr) - sr, 2);
        }

        private void UpdateTradedLadder(RunnerChange runnerChange)
        {
            TradedLadder.Update(
                runnerChange.Traded, LastPublishTime);
        }

        private void SetTotalMatched(RunnerChange runnerChange)
        {
            TotalMatched = runnerChange.TotalMatched ?? TotalMatched;
        }

        private void SetLastTradedPrice(RunnerChange runnerChange)
        {
            LastTradedPrice = runnerChange.LastTradedPrice ?? LastTradedPrice;
        }

        private void ProcessBestAvailableToBack(List<List<double?>> availableToBacks)
        {
            availableToBacks?.ForEach(UpdateBestAvailableToBack);
        }

        private void UpdateBestAvailableToBack(List<double?> availableToBack)
        {
            BestAvailableToBack.ProcessLevel(
                availableToBack.Select(d => d ?? 0).ToList());
        }

        private void ProcessBestAvailableToLay(List<List<double?>> availableToLays)
        {
            availableToLays?.ForEach(UpdateBestAvailableToLay);
        }

        private void UpdateBestAvailableToLay(List<double?> availableToLay)
        {
            BestAvailableToLay.ProcessLevel(
                availableToLay.Select(d => d ?? 0).ToList());
        }
    }
}
