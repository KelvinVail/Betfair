using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Contracts;

public interface ISubscription
{
    Task Subscribe(StreamMarketFilter marketFilter, DataFilter? dataFilter = null, TimeSpan? conflate = null, CancellationToken cancellationToken = default);

    Task SubscribeToOrders(OrderFilter? orderFilter = null, TimeSpan? conflate = null, CancellationToken cancellationToken = default);

    IAsyncEnumerable<ChangeMessage> ReadLines(CancellationToken cancellationToken);

    IAsyncEnumerable<byte[]> ReadBytes(CancellationToken cancellationToken);
}