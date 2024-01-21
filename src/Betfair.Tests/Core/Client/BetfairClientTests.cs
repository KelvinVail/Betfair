using Betfair.Core.Client;
using Betfair.Tests.Core.Client.TestDoubles;
using Betfair.Tests.TestDoubles;

namespace Betfair.Tests.Core.Client;

public class BetfairClientTests : IDisposable
{
    private readonly TokenProviderStub _provider = new ();
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly Uri _uri = new ("http://test.com/");
    private readonly BetfairHttpClient _httpClient;
    private readonly BetfairClient _client;
    private bool _disposedValue;

    public BetfairClientTests()
    {
        _httpClient = new BetfairHttpClient(_handler);
        _client = new BetfairClient(_httpClient, _provider, "appKey");
    }

    [Theory]
    [InlineData("http://test.com")]
    [InlineData("http://other.com")]
    public async Task PostUsesThePassedInUri(string uri)
    {
        await _client.Post<object>(new Uri(uri), "token");

        _handler.MethodUsed.Should().Be(HttpMethod.Post);
        _handler.UriCalled.Should().Be(new Uri(uri));
    }

    [Fact]
    public async Task PostPutsContentTypeInContentHeader()
    {
        await _client.Post<dynamic>(_uri, "token");

        _handler.ContentHeadersSent.Should().ContainKey("Content-Type")
            .WhoseValue.Should().Contain("application/json");
    }

    [Theory]
    [InlineData("AppKey")]
    [InlineData("OtherKey")]
    public async Task PostPutsAppKeyIsInContentHeader(string appKey)
    { 
        var client = new BetfairClient(_httpClient, _provider, appKey);

        await client.Post<dynamic>(_uri);

        _handler.ContentHeadersSent.Should().ContainKey("X-Application")
            .WhoseValue.Should().Contain(appKey);
    }

    [Theory]
    [InlineData("SessionToken")]
    [InlineData("OtherToken")]
    public async Task PostPutsSessionTokenIsInContentHeader(string token)
    {
        _provider.RespondsWithToken = token;

        await _client.Post<dynamic>(_uri);

        _handler.ContentHeadersSent.Should().ContainKey("X-Authentication")
            .WhoseValue.Should().Contain(token);
    }

    [Theory]
    [InlineData("bodyContent")]
    [InlineData("other")]
    public async Task PostPutsBodyInTheRequest(object body)
    {
        await _client.Post<dynamic>(_uri, body);

        _handler.ContentSent.Should().Be(body);
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
        }

        _disposedValue = true;
    }
}
