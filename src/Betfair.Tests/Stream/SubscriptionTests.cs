using Betfair.Core.Authentication;
using Betfair.Stream;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using Betfair.Tests.Stream.TestDoubles;
using Betfair.Tests.TestDoubles;

namespace Betfair.Tests.Stream;

public class SubscriptionTests
{
    private readonly TokenProviderStub _tokenProvider = new ();
    private readonly PipelineStub _pipe = new ();

    [Fact]
    public void CredentialsMustNotBeNull()
    {
        var act = () => new Subscription(null!);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().BeEquivalentTo(nameof(Credentials));
    }

    [Fact]
    public async Task AuthenticatesWhenFirstMarketSubscriptionIsMade()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        await sub.Subscribe(filter);

        var authMessage = new Authentication(1, "Token", "a");
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(authMessage);
    }

    [Fact]
    public async Task AuthenticatesWhenFirstOrderSubscriptionIsMade()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);

        await sub.SubscribeToOrders();

        var authMessage = new Authentication(1, "Token", "a");
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(authMessage);
    }

    [Fact]
    public async Task DoesNotAuthenticateIfTheSubscriptionIsAlreadyAuthenticated()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        await sub.Subscribe(filter);
        await sub.SubscribeToOrders();
        await sub.Subscribe(filter);

        _pipe.ObjectsWritten.Should().ContainSingle(x => x.GetType() == typeof(Authentication));
    }

    [Theory]
    [InlineData("appKey")]
    [InlineData("newKey")]
    [InlineData("other")]
    public async Task AuthenticateWritesAppKeyToStream(string appKey)
    {
        using var sub = new Subscription(_tokenProvider, appKey, _pipe);

        await sub.SubscribeToOrders();

        var authMessage = new Authentication(1, "Token", appKey);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(authMessage);
    }

    [Theory]
    [InlineData("sessionToken")]
    [InlineData("newToken")]
    [InlineData("other")]
    public async Task AuthenticateWritesSessionTokenToStream(string sessionToken)
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        _tokenProvider.RespondsWithToken.Add(sessionToken);

        await sub.SubscribeToOrders();

        var authMessage = new Authentication(1, sessionToken, "a");
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(authMessage);
    }

    [Theory]
    [InlineData("1.2345")]
    [InlineData("1.9876")]
    public async Task SubscribeWritesAMarketSubscriptionMessageToTheStream(string marketId)
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds(marketId);

        await sub.Subscribe(filter);

        var subMessage = new MarketSubscription(2, filter, null, null, null, null);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(subMessage);
    }

    [Fact]
    public async Task EachSubscribeCallIncrementsTheConnectionId()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        await sub.Subscribe(filter);
        await sub.Subscribe(filter);

        var first = new MarketSubscription(2, filter, null, null, null, null);
        var second = new MarketSubscription(3, filter, null, null, null, null);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(first);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(second);
    }

    [Fact]
    public async Task SubscribeWritesTheDataFilterToTheStream()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var marketFilter = new StreamMarketFilter().WithMarketIds("1.2345");
        var dataFilter = new DataFilter().WithBestPrices();

        await sub.Subscribe(marketFilter, dataFilter);

        var subMessage = new MarketSubscription(2, marketFilter, dataFilter, null, null, null);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(subMessage);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1000)]
    [InlineData(9999)]
    public async Task SubscribeWritesTheConflateRateToTheStream(int conflateMs)
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var marketFilter = new StreamMarketFilter().WithMarketIds("1.2345");

        await sub.Subscribe(marketFilter, conflate: TimeSpan.FromMilliseconds(conflateMs));

        var subMessage = new MarketSubscription(2, marketFilter, conflate: TimeSpan.FromMilliseconds(conflateMs), initialClk: null, clk: null);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(subMessage);
    }

    [Fact]
    public async Task CapturesClockTokensFromChangeMessages()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var change1 = new ChangeMessage { Operation = "mcm", InitialClock = "ic1", Clock = "c1" };
        var change2 = new ChangeMessage { Operation = "mcm", Clock = "c2" };
        _pipe.ObjectsToBeRead.Add(change1);
        _pipe.ObjectsToBeRead.Add(change2);

        var last = default(ChangeMessage);
        await foreach (var msg in sub.ReadLines(default)) last = msg;

        last.Should().NotBeNull();
        last!.Clock.Should().Be("c2");
    }

    [Fact]
    public async Task SubscribeWithResumeTokensWritesClkFields()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        await sub.Subscribe(filter, initialClk: "ic", clk: "c");

        var expected = new MarketSubscription(2, filter, null, null, "ic", "c");
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(expected);
    }

    [Fact]
    public async Task SubscribeToOrdersWritesAnOrderSubscriptionMessageToTheStream()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);

        await sub.SubscribeToOrders();

        var subMessage = new OrderSubscription(2, null, null, null, null);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(subMessage);
    }

    [Theory]
    [InlineData("myRef")]
    [InlineData("Ref001")]
    public async Task SubscribeToOrderWritesOrderFilterToTheStream(string strategyRef)
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var orderFilter = new StreamOrderFilter().WithStrategyRefs(strategyRef);

        await sub.SubscribeToOrders(orderFilter);

        var subMessage = new OrderSubscription(2, orderFilter, null, null, null);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(subMessage);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1000)]
    [InlineData(9999)]
    public async Task SubscribeToOrderWritesTheConflateRateToTheStream(int conflateMs)
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);

        await sub.SubscribeToOrders(conflate: TimeSpan.FromMilliseconds(conflateMs));

        var subMessage = new OrderSubscription(2, null, TimeSpan.FromMilliseconds(conflateMs), null, null);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(subMessage);
    }

    [Fact]
    public async Task EachCallToSubscribeToOrdersIncrementsTheConnectionId()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);

        await sub.SubscribeToOrders();
        await sub.SubscribeToOrders();

        var first = new OrderSubscription(2);
        var second = new OrderSubscription(3);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(first);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(second);
    }

    [Fact]
    public async Task ChangeMessagesAreReadFromTheStream()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var message = new ChangeMessage { Operation = "Test" };
        _pipe.ObjectsToBeRead.Add(message);

        var read = new List<object>();
        await foreach (var line in sub.ReadLines(default))
            read.Add(line);

        read.Should().ContainEquivalentOf(message, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
    }

    [Fact]
    public async Task MultipleChangeMessagesAreReadFromTheStream()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var message1 = new ChangeMessage { Operation = "Test1" };
        _pipe.ObjectsToBeRead.Add(message1);
        var message2 = new ChangeMessage { Operation = "Test2" };
        _pipe.ObjectsToBeRead.Add(message2);

        var read = new List<object>();
        await foreach (var line in sub.ReadLines(default))
            read.Add(line);

        read.Should().ContainEquivalentOf(message1, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
        read.Should().ContainEquivalentOf(message2, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
    }
}
