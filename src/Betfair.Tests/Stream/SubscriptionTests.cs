﻿using Betfair.Core.Login;
using Betfair.Stream;
using Betfair.Stream.Messages;
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

        var subMessage = new MarketSubscription(2, filter);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(subMessage);
    }

    [Fact]
    public async Task EachSubscribeCallIncrementsTheConnectionId()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var filter = new StreamMarketFilter().WithMarketIds("1.2345");

        await sub.Subscribe(filter);
        await sub.Subscribe(filter);

        var first = new MarketSubscription(2, filter);
        var second = new MarketSubscription(3, filter);
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

        var subMessage = new MarketSubscription(2, marketFilter, dataFilter);
        _pipe.ObjectsWritten.Should().ContainEquivalentOf(subMessage);
    }

    [Fact]
    public async Task SubscribeToOrdersWritesAnOrderSubscriptionMessageToTheStream()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);

        await sub.SubscribeToOrders();

        var subMessage = new OrderSubscription(2);
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

    //    [Fact]
    //    public void DisposesTheStreamWhenDisposed()
    //    {
    //        _client.Dispose();

    //        _ms.Should().NotBeWritable();
    //    }

    //    [Fact]
    //    public void DisposesTheHttpClientWhenDisposed()
    //    {
    //        _client.Dispose();

    //        _httpClient.IsDisposed.Should().BeTrue();

    //        var act = () => _httpClient.Send(new HttpRequestMessage());

    //        act.Should().Throw<InvalidOperationException>()
    //            .And.Message.Should().StartWith("Cannot access a disposed object.");
    //    }

    //    [Fact]
    //    public void DisposeShouldBeIdempotent()
    //    {
    //        _client.Dispose();
    //#pragma warning disable S3966
    //        _client.Dispose();
    //#pragma warning restore S3966

    //        _httpClient.TimesDisposed.Should().Be(1);
    //    }

    //    [Fact]
    //    public async Task ChangeMessagesAreReadFromTheStream()
    //    {
    //        var message = new ChangeMessage { Operation = "Test" };
    //        await SendChange(message);

    //        var read = await ReadMessages();

    //        read.Should().ContainEquivalentOf(message, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
    //    }

    //    [Fact]
    //    public async Task MultipleChangeMessagesAreReadFromTheStream()
    //    {
    //        var message1 = new ChangeMessage { Operation = "Test1" };
    //        await SendChange(message1);
    //        var message2 = new ChangeMessage { Operation = "Test2" };
    //        await SendChange(message2);

    //        var read = await ReadMessages();

    //        read.Should().ContainEquivalentOf(message1, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
    //        read.Should().ContainEquivalentOf(message2, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
    //    }

    //    [Fact]
    //    public void CredentialShouldNotBeNull()
    //    {
    //        var act = () => new StreamClient(null!);

    //        act.Should().Throw<ArgumentNullException>()
    //            .And.ParamName.Should().Be("credentials");
    //    }

    //    [Fact]
    //    public async Task BetfairStreamCanBeCopiedToAStream()
    //    {
    //        var message1 = new ChangeMessage { Operation = "Test1" };
    //        await SendChange(message1);
    //        var message2 = new ChangeMessage { Operation = "Test2" };
    //        await SendChange(message2);
    //        _ms.Position = 0;

    //        using var ms = new MemoryStream();
    //        await _client.CopyToStream(ms, default);

    //        ms.Position = 0;
    //        using var reader = new StreamReader(ms);
    //        var lines = new List<string>();
    //        while (!reader.EndOfStream)
    //            lines.Add(await reader.ReadLineAsync());

    //        lines.Count.Should().Be(2);
    //    }
}
