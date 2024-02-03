namespace Betfair.Stream.Messages;

internal class OrderSubscription : MessageBase
{
    public OrderSubscription(int id, TimeSpan? conflate = null)
        : base("orderSubscription", id)
    {
        ConflateMs = (int)(conflate?.TotalMilliseconds ?? 0);
    }

    public int ConflateMs { get; }
}
