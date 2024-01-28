using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketCatalogueTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public MarketCatalogueTests() =>
        _api = new BetfairApiClient(_client);

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.MarketCatalogue();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listMarketCatalogue/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _api.MarketCatalogue();

        _client.LastContentSent.Should().BeEquivalentTo(
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
        if (disposing) _api.Dispose();

        _disposedValue = true;
    }
}