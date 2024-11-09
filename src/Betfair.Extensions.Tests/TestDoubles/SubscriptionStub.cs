using Betfair.Extensions.Contracts;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Tests.TestDoubles;

internal class SubscriptionStub : ISubscription
{
    public Task Subscribe(
        StreamMarketFilter marketFilter,
        DataFilter? dataFilter = null,
        TimeSpan? conflate = null,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task SubscribeToOrders(
        OrderFilter? orderFilter = null,
        TimeSpan? conflate = null,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public IAsyncEnumerable<ChangeMessage> ReadLines(
        CancellationToken cancellationToken) =>
        Array.Empty<ChangeMessage>().ToAsyncEnumerable();

    public IAsyncEnumerable<byte[]> ReadBytes(
        CancellationToken cancellationToken) =>
        Array.Empty<byte[]>().ToAsyncEnumerable();
}
