using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Tests.TestDoubles;
using Utf8Json;
using Utf8Json.Resolvers;

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
    public void DefaultFilterIncludesEmptyFilterAndMaxResults()
    {
        var filter = new MarketCatalogueFilter();

        filter.Filter.Should().NotBeNull();
        filter.MaxResults.Should().Be(1000);
    }

    [Fact]
    public void DefaultFilterIsSerializedCorrectly()
    {
        var filter = new MarketCatalogueFilter();

        var json = JsonSerializer.ToJsonString(filter, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be("{\"filter\":{},\"maxResults\":1000}");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _client.MarketCatalogue();

        _httpClient.LastPostedBody.Should().BeEquivalentTo(
            new MarketCatalogueFilter());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    public void EventTypeIdsCanBeAddedToTheFilter(int id)
    {
        var filter = new MarketCatalogueFilter()
            .WithEventTypeId(id)
            .WithEventTypeId(10);

        var json = JsonSerializer.ToJsonString(filter, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be($"{{\"filter\":{{\"eventTypeIds\":[{id},10]}},\"maxResults\":1000}}");
    }

    [Fact]
    public void EventTypeCanBeAddedToTheFilter()
    {
        var filter = new MarketCatalogueFilter()
            .WithEventType(EventType.HorseRacing)
            .WithEventTypeId(10);

        var json = JsonSerializer.ToJsonString(filter, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be($"{{\"filter\":{{\"eventTypeIds\":[7,10]}},\"maxResults\":1000}}");
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
