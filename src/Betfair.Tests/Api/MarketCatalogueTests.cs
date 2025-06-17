using Betfair.Api;
using Betfair.Api.Betting;
using Betfair.Api.Betting.Endpoints.ListCompetitions.Responses;
using Betfair.Api.Betting.Endpoints.ListEventTypes.Responses;
using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Enums;
using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Requests;
using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketCatalogueTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public MarketCatalogueTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<MarketCatalogue>();
    }

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

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");
        var query = new MarketCatalogueQuery()
            .Include(MarketProjection.MarketStartTime)
            .OrderBy(MarketSort.MaximumTraded)
            .Take(10);

        await _api.MarketCatalogue(filter, query);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.MarketCatalogueRequest);

        json.Should().Be("{\"filter\":{\"marketIds\":[\"1.23456789\"]},\"marketProjection\":[\"MARKET_START_TIME\"],\"sort\":\"MAXIMUM_TRADED\",\"maxResults\":10}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new MarketCatalogue
            {
                MarketId = "1.23456789",
                MarketName = "Match Odds",
                TotalMatched = 1000.50m,
                Competition = new Competition
                {
                    Id = "10932509",
                    Name = "English Premier League",
                },
                Event = new MarketEvent
                {
                    Id = "29301",
                    Name = "Test Match",
                    CountryCode = "GB",
                    Timezone = "GMT",
                    OpenDate = DateTimeOffset.UtcNow.AddDays(1),
                },
                EventType = new EventType
                {
                    Id = "1",
                    Name = "Soccer",
                },
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.MarketCatalogue();

        response.Should().BeEquivalentTo(expectedResponse);
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
            _api.Dispose();
        }

        _disposedValue = true;
    }
}