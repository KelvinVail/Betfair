namespace Betfair.Stream.Messages;

internal class OrderSubscription : MessageBase
{
    public OrderSubscription(
        int id,
        StreamOrderFilter? orderFilter = null,
        TimeSpan? conflate = null,
        string? initialClk = null,
        string? clk = null)
        : base("orderSubscription", id)
    {
        OrderFilter = orderFilter;
        ConflateMs = (int)(conflate?.TotalMilliseconds ?? 0);
        InitialClk = initialClk;
        Clk = clk;
    }

    [JsonPropertyName("orderFilter")]
    public StreamOrderFilter? OrderFilter { get; }

    [JsonPropertyName("conflateMs")]
    public int ConflateMs { get; }

    [JsonPropertyName("initialClk")]
    public string? InitialClk { get; }

    [JsonPropertyName("clk")]
    public string? Clk { get; }
}
