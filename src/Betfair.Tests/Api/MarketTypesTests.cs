using Betfair.Api;
using Betfair.Api.Betting;
using Betfair.Api.Betting.Endpoints.ListMarketTypes.Requests;
using Betfair.Api.Betting.Endpoints.ListMarketTypes.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketTypesTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public MarketTypesTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<MarketTypeResult>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.MarketTypes();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listMarketTypes/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _api.MarketTypes();

        _client.LastContentSent.Should().BeEquivalentTo(
            new MarketTypesRequest());
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.MarketTypes(filter);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.MarketTypesRequest);

        json.Should().Be("{\"filter\":{\"marketIds\":[\"1.23456789\"]}}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        _client.RespondsWithBody = new[]
        {
            new MarketTypeResult
            {
                MarketType = "Soccer",
                MarketCount = 2,
            },
        };

        var response = await _api.MarketTypes();

        response.Should().HaveCount(1);
        response[0].MarketType.Should().Be("Soccer");
        response[0].MarketCount.Should().Be(2);
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
