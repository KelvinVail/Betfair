namespace Betfair.Stream.Messages;

internal class MarketSubscription : MessageBase
{
    public MarketSubscription(
        int id,
        StreamMarketFilter marketFiler,
        DataFilter dataFilter)
        : base("marketSubscription", id)
    {
        MarketFilter = marketFiler;
        MarketDataFilter = dataFilter;
    }

    public StreamMarketFilter MarketFilter { get; }

    public DataFilter MarketDataFilter { get; }
}
