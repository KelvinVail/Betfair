namespace Betfair.Stream.Messages;

internal class OrderSubscription : MessageBase
{
    public OrderSubscription(int id)
        : base("orderSubscription", id)
    {
    }
}
