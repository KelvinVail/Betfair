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

    [JsonPropertyName("marketFilter")]
    public StreamMarketFilter MarketFilter { get; }

    [JsonPropertyName("marketDataFilter")]
    public DataFilter MarketDataFilter { get; }

    [JsonPropertyName("conflateMs")]
    public int ConflateMs { get; }
}
