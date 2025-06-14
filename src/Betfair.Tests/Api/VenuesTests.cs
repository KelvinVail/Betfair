using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class VenuesTests : IDisposable
{
    private readonly HttpAdapterStub _client = new();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public VenuesTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<VenueResult>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.Venues();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listVenues/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _api.Venues();

        _client.LastContentSent.Should().BeEquivalentTo(
            new VenuesRequest());
    }

    [Fact]
    public async Task PostsFilterIfProvided()
    {
        var filter = new ApiMarketFilter().WithEventTypes(1);

        await _api.Venues(filter);

        _client.LastContentSent.Should().BeEquivalentTo(
            new VenuesRequest
            {
                Filter = filter,
            });
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var filter = new ApiMarketFilter().WithEventTypes(1);

        await _api.Venues(filter);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.VenuesRequest);

        json.Should().Be("{\"filter\":{\"eventTypeIds\":[1]}}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new VenueResult
            {
                Venue = "Cheltenham",
                MarketCount = 5,
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.Venues();

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