namespace Betfair.Stream.Messages;

internal class OrderSubscription : MessageBase
{
    public OrderSubscription(
        int id,
        StreamOrderFilter? orderFilter = null,
        TimeSpan? conflate = null)
        : base("orderSubscription", id)
    {
        OrderFilter = orderFilter;
        ConflateMs = (int)(conflate?.TotalMilliseconds ?? 0);
    }

    [JsonPropertyName("orderFilter")]
    public StreamOrderFilter? OrderFilter { get; }

    [JsonPropertyName("conflateMs")]
    public int ConflateMs { get; }
}
