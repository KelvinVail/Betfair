namespace Betfair.Stream.Messages;

internal class MarketSubscription(
    int id,
    StreamMarketFilter marketFiler,
    DataFilter? dataFilter = null,
    TimeSpan? conflate = null,
    string? initialClk = null,
    string? clk = null)
    : MessageBase("marketSubscription", id)
{
    [JsonPropertyName("marketFilter")]
    public StreamMarketFilter MarketFilter { get; } = marketFiler;

    [JsonPropertyName("marketDataFilter")]
    public DataFilter MarketDataFilter { get; } = dataFilter ?? new DataFilter().WithBestPrices();

    [JsonPropertyName("conflateMs")]
    public int ConflateMs { get; } = (int)(conflate?.TotalMilliseconds ?? 0);

    [JsonPropertyName("initialClk")]
    public string? InitialClk { get; } = initialClk;

    [JsonPropertyName("clk")]
    public string? Clk { get; } = clk;
}
