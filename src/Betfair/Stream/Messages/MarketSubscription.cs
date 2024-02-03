namespace Betfair.Stream.Messages;

internal class MarketSubscription : MessageBase
{
    public MarketSubscription(
        int id,
        StreamMarketFilter marketFiler,
        DataFilter? dataFilter = null)
        : base("marketSubscription", id)
    {
        MarketFilter = marketFiler;
        MarketDataFilter = dataFilter ?? new DataFilter().WithBestPrices();
    }

    public StreamMarketFilter MarketFilter { get; }

    public DataFilter MarketDataFilter { get; }
}
