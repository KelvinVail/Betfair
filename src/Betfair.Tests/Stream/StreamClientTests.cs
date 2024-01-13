using Betfair.Stream;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using Betfair.Tests.TestDoubles;
using Utf8Json;
using Utf8Json.Resolvers;
using Xunit.Abstractions;

namespace Betfair.Tests.Stream;

public class StreamClientTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly MemoryStream _ms = new ();
    private readonly BetfairHttpClientStub _httpClient = new ();
    private readonly StreamClient _client;
    private readonly StreamReader _sr;
    private bool _disposedValue;

    public StreamClientTests(ITestOutputHelper output)
    {
        _output = output;
        _client = new StreamClient(_ms, _httpClient);
        _sr = new StreamReader(_ms);
    }

    [Fact]
    public async Task AuthenticateWritesMessageToStream()
    {
        await _client.Authenticate();

        var result = await ReadLastLineInStream();
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

        var result = await ReadLastLineInStream();

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

        var result = await ReadLastLineInStream();
        result.Should().ContainKey("session").WhoseValue.Should().Be(sessionToken);
    }

    [Fact]
    public async Task EachCallToAuthenticateIncrementsTheConnectionId()
    {
        await _client.Authenticate();
        var result = await ReadLastLineInStream();
        result.Should().ContainKey("id").WhoseValue.Should().Be(1);

        await _client.Authenticate();
        var result2 = await ReadLastLineInStream();
        result2.Should().ContainKey("id").WhoseValue.Should().Be(2);
    }

    [Fact]
    public async Task SubscribeWritesAMarketSubscriptionMessageToTheStream()
    {
        await _client.Subscribe(new StreamMarketFilter(), new DataFilter());

        var result = await ReadLastLineInStream();

        result.Should().ContainKey("op").WhoseValue.Should().Be("marketSubscription");
    }

    [Fact]
    public async Task EachCallToSubscribeIncrementsTheConnectionId()
    {
        await _client.Subscribe(new StreamMarketFilter(), new DataFilter());
        var result = await ReadLastLineInStream();
        result.Should().ContainKey("id").WhoseValue.Should().Be(1);

        await _client.Subscribe(new StreamMarketFilter(), new DataFilter());
        var result2 = await ReadLastLineInStream();
        result2.Should().ContainKey("id").WhoseValue.Should().Be(2);
    }

    [Theory]
    [InlineData("marketId")]
    [InlineData("1.23456789")]
    public async Task SubscribeWritesTheMarketFilterToTheStream(string marketId)
    {
        var marketFilter = new StreamMarketFilter().IncludeMarketIds(marketId);
        await _client.Subscribe(marketFilter, new DataFilter());

        var result = await ReadLastLineInStream();

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
        await _client.Subscribe(new StreamMarketFilter(), dataFilter);

        var result = await ReadLastLineInStream();

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

        var result = await ReadLastLineInStream();

        result.Should().ContainKey("op").WhoseValue.Should().Be("orderSubscription");
    }

    [Fact]
    public async Task EachCallToSubscribeToOrdersIncrementsTheConnectionId()
    {
        await _client.SubscribeToOrders();
        var result = await ReadLastLineInStream();
        result.Should().ContainKey("id").WhoseValue.Should().Be(1);

        await _client.SubscribeToOrders();
        var result2 = await ReadLastLineInStream();
        result2.Should().ContainKey("id").WhoseValue.Should().Be(2);
    }

    [Fact]
    public void DisposesTheStreamWhenDisposed()
    {
        _client.Dispose();

        _ms.Should().NotBeWritable();
    }

    [Fact]
    public void DisposesTheHttpClientWhenDisposed()
    {
        _client.Dispose();

        _httpClient.IsDisposed.Should().BeTrue();

        var act = () => _httpClient.Send(new HttpRequestMessage());

        act.Should().Throw<InvalidOperationException>()
            .And.Message.Should().StartWith("Cannot access a disposed object.");
    }

    [Fact]
    public void DisposeShouldBeIdempotent()
    {
        _client.Dispose();
#pragma warning disable S3966
        _client.Dispose();
#pragma warning restore S3966

        _httpClient.TimesDisposed.Should().Be(1);
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

    [Fact]
    public void CredentialShouldNotBeNull()
    {
        var act = () => new StreamClient(null!);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("credentials");
    }

    [Fact]
    public async Task BetfairStreamCanBeCopiedToAStream()
    {
        var message1 = new ChangeMessage { Operation = "Test1" };
        await SendChange(message1);
        var message2 = new ChangeMessage { Operation = "Test2" };
        await SendChange(message2);
        _ms.Position = 0;

        using var ms = new MemoryStream();
        await _client.CopyToStream(ms, default);

        ms.Position = 0;
        using var reader = new StreamReader(ms);
        var lines = new List<string>();
        while (!reader.EndOfStream)
            lines.Add(await reader.ReadLineAsync());

        lines.Count.Should().Be(2);
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
            _ms.Dispose();
            _sr.Dispose();
        }

        _disposedValue = true;
    }

    private async Task<Dictionary<string, object>> ReadLastLineInStream()
    {
        _ms.Position = 0;
        var line = string.Empty;
        while (!_sr.EndOfStream)
        {
            line = await _sr.ReadLineAsync();
            _output.WriteLine(line);
        }

        return JsonSerializer.Deserialize<Dictionary<string, object>>(line, StandardResolver.AllowPrivateCamelCase);
    }

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
