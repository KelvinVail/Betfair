using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketCatalogueTests
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;

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
}
