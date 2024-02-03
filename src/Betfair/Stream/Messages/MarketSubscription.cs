namespace Betfair.Stream.Messages;

internal class MarketSubscription : MessageBase
{
    public MarketSubscription(
        int id,
        StreamMarketFilter marketFiler,
        DataFilter? dataFilter = null,
        TimeSpan? conflate = null)
        : base("marketSubscription", id)
    {
        MarketFilter = marketFiler;
        MarketDataFilter = dataFilter ?? new DataFilter().WithBestPrices();
        ConflateMs = (int)(conflate?.TotalMilliseconds ?? 0);
    }

    public StreamMarketFilter MarketFilter { get; }

    public DataFilter MarketDataFilter { get; }

    public int ConflateMs { get; }
}
