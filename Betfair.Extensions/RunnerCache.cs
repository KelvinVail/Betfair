namespace Betfair.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Betfair.Stream.Responses;

    public sealed class RunnerCache
    {
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

        public void ProcessRunnerChange(RunnerChange runnerChange, long? lastUpdated)
        {
            if (runnerChange?.SelectionId != this.SelectionId) return;

            this.LastPublishTime = lastUpdated;
            this.SetLastTradedPrice(runnerChange);
            this.SetTotalMatched(runnerChange);
            this.ProcessBestAvailableToBack(runnerChange.BestAvailableToBack);
            this.ProcessBestAvailableToLay(runnerChange.BestAvailableToLay);
            this.UpdateTradedLadder(runnerChange);
        }

        private void UpdateTradedLadder(RunnerChange runnerChange)
        {
            this.TradedLadder.Update(
                runnerChange.Traded?.Select(t => t.Select(d => d ?? 0).ToList()).ToList(),
                this.LastPublishTime);
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
            this.BestAvailableToBack.ProcessLevel(availableToBack.Select(d => d ?? 0).ToList());
        }

        private void ProcessBestAvailableToLay(List<List<double?>> availableToLays)
        {
            availableToLays?.ForEach(this.UpdateBestAvailableToLay);
        }

        private void UpdateBestAvailableToLay(List<double?> availableToLay)
        {
            this.BestAvailableToLay.ProcessLevel(availableToLay.Select(d => d ?? 0).ToList());
        }
    }
}
