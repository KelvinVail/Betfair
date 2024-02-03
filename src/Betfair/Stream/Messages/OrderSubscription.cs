namespace Betfair.Stream.Messages;

internal class OrderSubscription : MessageBase
{
    public OrderSubscription(
        int id,
        OrderFilter? orderFilter = null,
        TimeSpan? conflate = null)
        : base("orderSubscription", id)
    {
        OrderFilter = orderFilter;
        ConflateMs = (int)(conflate?.TotalMilliseconds ?? 0);
    }

    public OrderFilter? OrderFilter { get; }

    public int ConflateMs { get; }
}
