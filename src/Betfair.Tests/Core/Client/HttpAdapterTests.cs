using Betfair.Core.Client;
using Betfair.Tests.Core.Client.TestDoubles;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Tests.Core.Client;

public class HttpAdapterTests : IDisposable
{
    private readonly HttpClientStub _httpClient = new ();
    private readonly HttpContent _content = new StringContent("content");
    private readonly Uri _uri = new ("http://test.com");
    private readonly HttpAdapter _adapter;
    private bool _disposedValue;

    public HttpAdapterTests() =>
        _adapter = new HttpAdapter(_httpClient);

    [Fact]
    public async Task PostPutsContentTypeInContentHeader()
    {
        await _adapter.PostAsync<dynamic>(_uri, _content);

        _httpClient.HttpContentSent?.Headers.Should().ContainKey("Content-Type")
            .WhoseValue.Should().Contain("application/json");
    }

    [Theory]
    [InlineData("bodyContent")]
    [InlineData("other")]
    public async Task PostPutsBodyInTheRequest(object body)
    {
        await _adapter.PostAsync<dynamic>(_uri, body);

        var bytes = JsonSerializer.Serialize(body, StandardResolver.AllowPrivateExcludeNullCamelCase);
        _httpClient.ContentSent.Should().BeEquivalentTo(bytes);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _content.Dispose();

        _disposedValue = true;
    }
}
