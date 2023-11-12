using Betfair.Core.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using Betfair.Stream.Tests.TestDoubles;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Stream.Tests;

public class StreamClientTests : IDisposable
{
    private readonly MemoryStream _ms = new ();
    private readonly BetfairHttpClientStub _httpClient = new (new Credentials("u", "p", "a"));
    private readonly StreamClient _client;
    private bool _disposedValue;

    public StreamClientTests() =>
        _client = new StreamClient(_ms, _httpClient);

    [Fact]
    public async Task AuthenticateWritesMessageToStream()
    {
        await _client.Authenticate();

        var result = await ReadStream();
        result.Should().ContainKey("op").WhoseValue.Should().Be("authentication");
    }

    [Theory]
    [InlineData("appKey")]
    [InlineData("newKey")]
    [InlineData("other")]
    public async Task AuthenticateWritesAppKeyToStream(string appKey)
    {
        _httpClient.ReturnsAppKey = appKey;

        await _client.Authenticate();

        var result = await ReadStream();
        result.Should().ContainKey("appKey").WhoseValue.Should().Be(appKey);
    }

    [Theory]
    [InlineData("sessionToken")]
    [InlineData("newToken")]
    [InlineData("other")]
    public async Task AuthenticateWritesSessionTokenToStream(string sessionToken)
    {
        _httpClient.ReturnsToken = sessionToken;

        await _client.Authenticate();

        var result = await ReadStream();
        result.Should().ContainKey("session").WhoseValue.Should().Be(sessionToken);
    }

    [Fact]
    public async Task EachCallToAuthenticateIncrementsTheConnectionId()
    {
        await _client.Authenticate();
        var result = await ReadStream();
        result.Should().ContainKey("id").WhoseValue.Should().Be(1);

        await _client.Authenticate();
        var result2 = await ReadStream();
        result2.Should().ContainKey("id").WhoseValue.Should().Be(2);
    }

    [Fact]
    public async Task SubscribeWritesAMarketSubscriptionMessageToTheStream()
    {
        await _client.Subscribe(new MarketFilter(), new DataFilter());

        var result = await ReadStream();

        result.Should().ContainKey("op").WhoseValue.Should().Be("marketSubscription");
    }

    [Fact]
    public async Task EachCallToSubscribeIncrementsTheConnectionId()
    {
        await _client.Subscribe(new MarketFilter(), new DataFilter());
        var result = await ReadStream();
        result.Should().ContainKey("id").WhoseValue.Should().Be(1);

        await _client.Subscribe(new MarketFilter(), new DataFilter());
        var result2 = await ReadStream();
        result2.Should().ContainKey("id").WhoseValue.Should().Be(2);
    }

    [Theory]
    [InlineData("marketId")]
    [InlineData("1.23456789")]
    public async Task SubscribeWritesTheMarketFilterToTheStream(string marketId)
    {
        var marketFilter = new MarketFilter().WithMarketId(marketId);
        await _client.Subscribe(marketFilter, new DataFilter());

        var result = await ReadStream();

        result.Should().ContainKey("marketFilter")
            .WhoseValue.Should().BeAssignableTo<Dictionary<string, object>>()
            .Which.Should().ContainKey("marketIds")
            .WhoseValue.Should().BeAssignableTo<List<object>>()
            .Which.Should().Contain(marketId);
    }

    [Fact]
    public async Task SubscribeWritesTheDataFilterToTheStream()
    {
        var dataFilter = new DataFilter().WithBestPrices();
        await _client.Subscribe(new MarketFilter(), dataFilter);

        var result = await ReadStream();

        result.Should().ContainKey("marketDataFilter")
            .WhoseValue.Should().BeAssignableTo<Dictionary<string, object>>()
            .Which.Should().ContainKey("fields")
            .WhoseValue.Should().BeAssignableTo<List<object>>()
            .Which.Should().BeEquivalentTo(dataFilter.Fields);
    }

    [Fact]
    public async Task SubscribeToOrdersWritesAnOrderSubscriptionMessageToTheStream()
    {
        await _client.SubscribeToOrders();

        var result = await ReadStream();

        result.Should().ContainKey("op").WhoseValue.Should().Be("orderSubscription");
    }

    [Fact]
    public async Task EachCallToSubscribeToOrdersIncrementsTheConnectionId()
    {
        await _client.SubscribeToOrders();
        var result = await ReadStream();
        result.Should().ContainKey("id").WhoseValue.Should().Be(1);

        await _client.SubscribeToOrders();
        var result2 = await ReadStream();
        result2.Should().ContainKey("id").WhoseValue.Should().Be(2);
    }

    [Fact]
    public void DisposesTheStreamWhenDisposed()
    {
        _client.Dispose();

        _ms.Should().NotBeWritable();
    }

    [Fact]
    public async Task ChangeMessagesAreReadFromTheStream()
    {
        var message = new ChangeMessage { Operation = "Test" };
        await SendChange(message);

        var read = await ReadMessages();

        read.Should().ContainEquivalentOf(message, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
    }

    [Fact]
    public async Task MultipleChangeMessagesAreReadFromTheStream()
    {
        var message1 = new ChangeMessage { Operation = "Test1" };
        await SendChange(message1);
        var message2 = new ChangeMessage { Operation = "Test2" };
        await SendChange(message2);

        var read = await ReadMessages();

        read.Should().ContainEquivalentOf(message1, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
        read.Should().ContainEquivalentOf(message2, o => o.Excluding(m => m.ReceivedTick).Excluding(m => m.DeserializedTick));
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            _client.Dispose();
            _httpClient.Dispose();
        }

        _disposedValue = true;
    }

    private Task<Dictionary<string, object>> ReadStream() =>
        JsonSerializer.DeserializeAsync<Dictionary<string, object>>(
            _ms,
            StandardResolver.AllowPrivateCamelCase);

    private Task SendChange(ChangeMessage message) =>
        JsonSerializer.SerializeAsync(_ms, message, StandardResolver.AllowPrivateExcludeNullCamelCase)
            .ContinueWith(_ => _ms.WriteByte((byte)'\n'));

    private async Task<List<ChangeMessage>> ReadMessages()
    {
        _ms.Position = 0;
        List<ChangeMessage> read = new ();
        await foreach (var change in _client.GetChanges())
            read.Add(change);

        return read;
    }
}
