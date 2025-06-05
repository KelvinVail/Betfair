using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class CountriesTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public CountriesTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<CountryCodeResult>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.Countries();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listCountries/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _api.Countries();

        _client.LastContentSent.Should().BeEquivalentTo(
            new CountriesRequest());
    }

    [Fact]
    public async Task PostsFilterIfProvided()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.Countries(filter);

        _client.LastContentSent.Should().BeEquivalentTo(
            new CountriesRequest { Filter = filter });
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.Countries(filter);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.CountriesRequest);

        json.Should().Be("{\"filter\":{\"marketIds\":[\"1.23456789\"]}}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        _client.RespondsWithBody = new[]
        {
            new CountryCodeResult { CountryCode = "GB", MarketCount = 2 },
        };

        var response = await _api.Countries();

        response.Should().HaveCount(1);
        response[0].MarketCount.Should().Be(2);
        response[0].CountryCode.Should().Be("GB");
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _client.Dispose();
                _api.Dispose();
            }

            _disposedValue = true;
        }
    }
}
