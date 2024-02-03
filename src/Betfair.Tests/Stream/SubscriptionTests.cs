using Betfair.Core.Login;
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
        var authMessage = new Authentication(1, "Token", "a");

        await sub.Subscribe(filter);

        _pipe.ObjectsWritten.Should().ContainEquivalentOf(authMessage);
    }

    [Fact]
    public async Task AuthenticatesWhenFirstOrderSubscriptionIsMade()
    {
        using var sub = new Subscription(_tokenProvider, "a", _pipe);
        var authMessage = new Authentication(1, "Token", "a");

        await sub.SubscribeToOrders();

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

    //    [Theory]
    //    [InlineData("appKey")]
    //    [InlineData("newKey")]
    //    [InlineData("other")]
    //    public async Task AuthenticateWritesAppKeyToStream(string appKey)
    //    {
    //        _httpClient.ReturnsAppKey = appKey;

    //        await _client.Authenticate();

    //        var result = await ReadLastLineInStream();

    //        result.Should().ContainKey("appKey").WhoseValue.Should().Be(appKey);
    //    }

    //    [Theory]
    //    [InlineData("sessionToken")]
    //    [InlineData("newToken")]
    //    [InlineData("other")]
    //    public async Task AuthenticateWritesSessionTokenToStream(string sessionToken)
    //    {
    //        _httpClient.ReturnsToken = sessionToken;

    //        await _client.Authenticate();

    //        var result = await ReadLastLineInStream();
    //        result.Should().ContainKey("session").WhoseValue.Should().Be(sessionToken);
    //    }

    //    [Fact]
    //    public async Task EachCallToAuthenticateIncrementsTheConnectionId()
    //    {
    //        await _client.Authenticate();
    //        var result = await ReadLastLineInStream();
    //        result.Should().ContainKey("id").WhoseValue.Should().Be(1);

    //        await _client.Authenticate();
    //        var result2 = await ReadLastLineInStream();
    //        result2.Should().ContainKey("id").WhoseValue.Should().Be(2);
    //    }

    //    [Fact]
    //    public async Task SubscribeWritesAMarketSubscriptionMessageToTheStream()
    //    {
    //        await _client.Subscribe(new StreamMarketFilter(), new DataFilter());

    //        var result = await ReadLastLineInStream();

    //        result.Should().ContainKey("op").WhoseValue.Should().Be("marketSubscription");
    //    }

    //    [Fact]
    //    public async Task EachCallToSubscribeIncrementsTheConnectionId()
    //    {
    //        await _client.Subscribe(new StreamMarketFilter(), new DataFilter());
    //        var result = await ReadLastLineInStream();
    //        result.Should().ContainKey("id").WhoseValue.Should().Be(1);

    //        await _client.Subscribe(new StreamMarketFilter(), new DataFilter());
    //        var result2 = await ReadLastLineInStream();
    //        result2.Should().ContainKey("id").WhoseValue.Should().Be(2);
    //    }

    //    [Theory]
    //    [InlineData("marketId")]
    //    [InlineData("1.23456789")]
    //    public async Task SubscribeWritesTheMarketFilterToTheStream(string marketId)
    //    {
    //        var marketFilter = new StreamMarketFilter().WithMarketIds(marketId);
    //        await _client.Subscribe(marketFilter, new DataFilter());

    //        var result = await ReadLastLineInStream();

    //        result.Should().ContainKey("marketFilter")
    //            .WhoseValue.Should().BeAssignableTo<Dictionary<string, object>>()
    //            .Which.Should().ContainKey("marketIds")
    //            .WhoseValue.Should().BeAssignableTo<List<object>>()
    //            .Which.Should().Contain(marketId);
    //    }

    //    [Fact]
    //    public async Task SubscribeWritesTheDataFilterToTheStream()
    //    {
    //        var dataFilter = new DataFilter().WithBestPrices();
    //        await _client.Subscribe(new StreamMarketFilter(), dataFilter);

    //        var result = await ReadLastLineInStream();

    //        result.Should().ContainKey("marketDataFilter")
    //            .WhoseValue.Should().BeAssignableTo<Dictionary<string, object>>()
    //            .Which.Should().ContainKey("fields")
    //            .WhoseValue.Should().BeAssignableTo<List<object>>()
    //            .Which.Should().BeEquivalentTo(dataFilter.Fields);
    //    }

    //    [Fact]
    //    public async Task SubscribeToOrdersWritesAnOrderSubscriptionMessageToTheStream()
    //    {
    //        await _client.SubscribeToOrders();

    //        var result = await ReadLastLineInStream();

    //        result.Should().ContainKey("op").WhoseValue.Should().Be("orderSubscription");
    //    }

    //    [Fact]
    //    public async Task EachCallToSubscribeToOrdersIncrementsTheConnectionId()
    //    {
    //        await _client.SubscribeToOrders();
    //        var result = await ReadLastLineInStream();
    //        result.Should().ContainKey("id").WhoseValue.Should().Be(1);

    //        await _client.SubscribeToOrders();
    //        var result2 = await ReadLastLineInStream();
    //        result2.Should().ContainKey("id").WhoseValue.Should().Be(2);
    //    }

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
