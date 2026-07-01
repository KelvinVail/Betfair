namespace Betfair.Stream.Messages;

internal class OrderSubscription(
    int id,
    StreamOrderFilter? orderFilter = null,
    TimeSpan? conflate = null,
    string? initialClk = null,
    string? clk = null)
    : MessageBase("orderSubscription", id)
{
    [JsonPropertyName("orderFilter")]
    public StreamOrderFilter? OrderFilter { get; } = orderFilter;

    [JsonPropertyName("conflateMs")]
    public int ConflateMs { get; } = (int)(conflate?.TotalMilliseconds ?? 0);

    [JsonPropertyName("initialClk")]
    public string? InitialClk { get; } = initialClk;

    [JsonPropertyName("clk")]
    public string? Clk { get; } = clk;
}
