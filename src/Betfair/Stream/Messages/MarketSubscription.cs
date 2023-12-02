namespace Betfair.Stream.Messages;

internal class MarketSubscription : MessageBase
{
    public MarketSubscription(
        int id,
        MarketFilter marketFiler,
        DataFilter dataFilter)
        : base("marketSubscription", id)
    {
        MarketFilter = marketFiler;
        MarketDataFilter = dataFilter;
    }

    public MarketFilter MarketFilter { get; }

    public DataFilter MarketDataFilter { get; }
}
