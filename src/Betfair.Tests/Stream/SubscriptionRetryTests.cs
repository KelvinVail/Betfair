using System.Collections.Concurrent;
using Betfair.Stream;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using Betfair.Tests.Stream.TestDoubles;
using Betfair.Tests.TestDoubles;

namespace Betfair.Tests.Stream;

public class SubscriptionRetryTests
{
    private readonly TokenProviderStub _tokenProvider = new ();
    private readonly PipelineStubWithRetry _pipe = new ();

    [Fact]
    public async Task DoesNotRetryWhenReaderIsNotActive()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        // Setup status responses that would trigger retry if reader was active
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 2, Operation = "status", StatusCode = "FAILURE" });
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 3, Operation = "status", StatusCode = "SUCCESS" });

        await sub.Subscribe(filter);

        // Should only have auth and subscription, no retry when reader not active
        _pipe.ObjectsWritten.Should().HaveCount(2);
        _pipe.ObjectsWritten[0].Should().BeOfType<Authentication>();
        _pipe.ObjectsWritten[1].Should().BeOfType<MarketSubscription>();
    }

    [Fact]
    public async Task DoesNotThrowWhenReaderNotActiveEvenWithBadStatus()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        // Setup status responses: both failure (but reader not active so ignored)
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 2, Operation = "status", StatusCode = "FAILURE" });
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 3, Operation = "status", StatusCode = "FAILURE" });

        // Should not throw when reader is not active
        await sub.Subscribe(filter);

        _pipe.ObjectsWritten.Should().HaveCount(2);
    }

    [Fact]
    public async Task IgnoresStatusResponsesWhenReaderNotActive()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        // Setup status responses: wrong id, then correct id (but reader not active so ignored)
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 999, Operation = "status", StatusCode = "SUCCESS" });
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 3, Operation = "status", StatusCode = "SUCCESS" });

        await sub.Subscribe(filter);

        // Should only have auth and subscription, no retry when reader not active
        _pipe.ObjectsWritten.Should().HaveCount(2);
        _pipe.ObjectsWritten[0].Should().BeOfType<Authentication>();
        _pipe.ObjectsWritten[1].Should().BeOfType<MarketSubscription>();
    }

    [Fact]
    public async Task IgnoresNullStatusResponsesWhenReaderNotActive()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        // Setup status responses: null, then success (but reader not active so ignored)
        _pipe.StatusResponses.Enqueue(null!);
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 3, Operation = "status", StatusCode = "SUCCESS" });

        await sub.Subscribe(filter);

        // Should only have auth and subscription, no retry when reader not active
        _pipe.ObjectsWritten.Should().HaveCount(2);
        _pipe.ObjectsWritten[0].Should().BeOfType<Authentication>();
        _pipe.ObjectsWritten[1].Should().BeOfType<MarketSubscription>();
    }

    [Fact]
    public async Task OrderSubscriptionDoesNotRetryWhenReaderNotActive()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);

        // Setup status responses: first failure, then success (but reader not active so ignored)
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 2, Operation = "status", StatusCode = "FAILURE" });
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 3, Operation = "status", StatusCode = "SUCCESS" });

        await sub.SubscribeToOrders();

        // Should only have auth and subscription, no retry when reader not active
        _pipe.ObjectsWritten.Should().HaveCount(2);
        _pipe.ObjectsWritten[0].Should().BeOfType<Authentication>();
        _pipe.ObjectsWritten[1].Should().BeOfType<OrderSubscription>();
    }

    [Fact]
    public async Task AuthenticationDoesNotRetryWhenReaderNotActive()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        // Setup status responses: auth failure, auth success, subscription success (but reader not active so ignored)
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 1, Operation = "status", StatusCode = "FAILURE" });
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 2, Operation = "status", StatusCode = "SUCCESS" });
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 3, Operation = "status", StatusCode = "SUCCESS" });

        await sub.Subscribe(filter);

        // Should only have auth and subscription, no retry when reader not active
        _pipe.ObjectsWritten.Should().HaveCount(2);
        _pipe.ObjectsWritten[0].Should().BeOfType<Authentication>();
        _pipe.ObjectsWritten[1].Should().BeOfType<MarketSubscription>();
    }

    [Fact]
    public async Task DoesNotTimeoutWhenReaderNotActive()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        // Simulate timeout scenario but reader not active so no timeout occurs
        _pipe.SimulateTimeout = true;

        // Should not throw when reader is not active
        await sub.Subscribe(filter);

        _pipe.ObjectsWritten.Should().HaveCount(2);
    }

    [Fact]
    public async Task DoesNotTimeoutOnRetryWhenReaderNotActive()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        // Setup first status response as failure, then simulate timeout on retry (but reader not active)
        _pipe.StatusResponses.Enqueue(new ChangeMessage { Id = 2, Operation = "status", StatusCode = "FAILURE" });
        _pipe.SimulateTimeoutOnRetry = true;

        // Should not throw when reader is not active
        await sub.Subscribe(filter);

        _pipe.ObjectsWritten.Should().HaveCount(2);
    }

    [Fact]
    public async Task WritesMessagesDirectlyWhenReaderIsNotActive()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        // When reader is not active, messages are written directly without status waiting
        await sub.Subscribe(filter);

        // Should only have auth and subscription, no retry logic
        _pipe.ObjectsWritten.Should().HaveCount(2);
        _pipe.ObjectsWritten[0].Should().BeOfType<Authentication>();
        _pipe.ObjectsWritten[1].Should().BeOfType<MarketSubscription>();
    }
}
