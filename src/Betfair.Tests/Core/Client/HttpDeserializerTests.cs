using System.Runtime.InteropServices;
using Betfair.Core.Client;
using Betfair.Tests.Core.Client.TestDoubles;
using Betfair.Tests.TestDoubles;

namespace Betfair.Tests.Core.Client;

public class HttpDeserializerTests : IDisposable
{
    private readonly TokenProviderStub _provider = new ();
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly Uri _uri = new ("http://test.com/");
    private readonly HttpContent _content = new StringContent("content");
    private readonly BetfairHttpClient _httpClient;
    private readonly HttpDeserializer _client;
    private bool _disposedValue;

    public HttpDeserializerTests()
    {
        _httpClient = new BetfairHttpClient(_handler);
        _client = new HttpDeserializer(_httpClient);
    }

    [Theory]
    [InlineData("http://test.com")]
    [InlineData("http://other.com")]
    public async Task PostUsesThePassedInUri(string uri)
    {
        await _client.PostAsync<object>(new Uri(uri), _content);

        _handler.MethodUsed.Should().Be(HttpMethod.Post);
        _handler.UriCalled.Should().Be(new Uri(uri));
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
            _httpClient.Dispose();
            _handler.Dispose();
            _content.Dispose();
        }

        _disposedValue = true;
    }
}
