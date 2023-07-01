using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Stream.Tests;

public class StreamClientTests : IDisposable
{
    private readonly MemoryStream _ms = new ();
    private readonly StreamClient _client;
    private bool _disposedValue;

    public StreamClientTests() =>
        _client = new StreamClient(_ms);

    [Fact]
    public async Task AuthenticateWritesMessageToStream()
    {
        await _client.Authenticate("appKey", "sessionToken");

        var result = await ReadStream();

        result.Should().ContainKey("op").WhoseValue.Should().Be("authentication");
    }

    [Theory]
    [InlineData("appKey")]
    [InlineData("newKey")]
    [InlineData("other")]
    public async Task AuthenticateWritesAppKeyToStream(string appKey)
    {
        await _client.Authenticate(appKey, "sessionToken");

        var result = await ReadStream();

        result.Should().ContainKey("appKey").WhoseValue.Should().Be(appKey);
    }

    [Theory]
    [InlineData("sessionToken")]
    [InlineData("newToken")]
    [InlineData("other")]
    public async Task AuthenticateWritesSessionTokenToStream(string sessionToken)
    {
        await _client.Authenticate("appKey", sessionToken);

        var result = await ReadStream();

        result.Should().ContainKey("session").WhoseValue.Should().Be(sessionToken);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _client.Dispose();

        _disposedValue = true;
    }

    private Task<Dictionary<string, object>> ReadStream() =>
        JsonSerializer.DeserializeAsync<Dictionary<string, object>>(
            _ms,
            StandardResolver.AllowPrivateExcludeNullCamelCase);
}
