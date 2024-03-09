using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Tests.Core.Client.TestDoubles;
using Betfair.Tests.TestDoubles;

namespace Betfair.Tests.Core.Client;

public class HttpAdapterTests : IDisposable
{
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly TokenProviderStub _tokenProvider = new ();
    private readonly HttpContent _content = new StringContent("content");
    private readonly Uri _uri = new ("http://test.com");
    private readonly BetfairHttpClient _httpClient;
    private readonly HttpAdapter _adapter;
    private bool _disposedValue;

    public HttpAdapterTests()
    {
        _httpClient = new BetfairHttpClient(_handler);
        _adapter = BetfairHttpFactory.Create(new Credentials("u", "p", "a"), _tokenProvider, _httpClient);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("other")]
    public async Task PostUsesThePassedInUri(string value)
    {
        var uri = new Uri($"http://{value}.com");

        await _adapter.PostAsync<object>(uri, _content);

        _handler.MethodUsed.Should().Be(HttpMethod.Post);
        _handler.UriCalled.Should().Be(uri);
    }

    [Fact]
    public async Task PostPutsContentTypeInContentHeader()
    {
        await _adapter.PostAsync<dynamic>(_uri, _content);

        _handler.ContentHeadersSent.Should().ContainKey("Content-Type")
            .WhoseValue.Should().Contain("application/json");
    }

    [Theory]
    [InlineData("bodyContent")]
    [InlineData("other")]
    public async Task PostPutsBodyInTheRequest(object body)
    {
        await _adapter.PostAsync<dynamic>(_uri, body);

        _handler.ContentSent.Should().BeEquivalentTo(body);
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
            _content.Dispose();
            _httpClient.Dispose();
            _handler.Dispose();
        }

        _disposedValue = true;
    }
}
