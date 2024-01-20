using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Tests.Core.Client.TestDoubles;
using Betfair.Tests.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketCatalogueTests : IDisposable
{
    private readonly TokenProviderStub _provider = new ();
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly HttpClientStub _httpClient;

    private readonly BetfairApiClient _client;
    private bool _disposedValue;

    public MarketCatalogueTests()
    {
        _httpClient = new HttpClientStub(_handler);
        _client = new BetfairApiClient(_httpClient, _provider);
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _client.MarketCatalogue();

        _handler.UriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listMarketCatalogue/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _client.MarketCatalogue();

        _handler.ContentSent.Should().BeEquivalentTo(
            new MarketCatalogueQuery());
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _httpClient.Dispose();
        _disposedValue = true;
    }
}
