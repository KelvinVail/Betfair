namespace Betfair.Betting
{
    using System;

    public class LimitOrder
    {
        public LimitOrder(string marketId, long selectionId)
        {
            this.MarketId = marketId ?? throw new ArgumentNullException(nameof(marketId), "MarketId should not be null or empty.");
            this.SelectionId = selectionId;
        }

        public string MarketId { get; }

        public long SelectionId { get; }

        public Side Side { get; set; }
    }
}
