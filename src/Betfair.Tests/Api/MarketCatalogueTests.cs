using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Tests.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketCatalogueTests : IDisposable
{
    private readonly BetfairHttpClientStub _httpClient = new ();

    private readonly BetfairApiClient _client;
    private bool _disposedValue;

    public MarketCatalogueTests() =>
        _client = new BetfairApiClient(_httpClient);

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _client.MarketCatalogue();

        _httpClient.LastPostedUri.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listMarketCatalogue/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _client.MarketCatalogue();

        _httpClient.LastPostedBody.Should().BeEquivalentTo(
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
