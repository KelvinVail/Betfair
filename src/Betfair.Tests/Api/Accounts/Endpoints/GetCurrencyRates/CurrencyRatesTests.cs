using Betfair.Api;
using Betfair.Api.Accounts.Endpoints.ListCurrencyRates.Requests;
using Betfair.Api.Accounts.Endpoints.ListCurrencyRates.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetCurrencyRates;

public class CurrencyRatesTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public CurrencyRatesTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<CurrencyRate>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.CurrencyRates();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/listCurrencyRates/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFromCurrencyIsNull()
    {
        await _api.CurrencyRates();

        _client.LastContentSent.Should().BeEquivalentTo(
            new CurrencyRatesRequest());
    }

    [Fact]
    public async Task PostsProvidedFromCurrency()
    {
        await _api.CurrencyRates("USD");

        _client.LastContentSent.Should().BeEquivalentTo(
            new CurrencyRatesRequest { FromCurrency = "USD" });
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        await _api.CurrencyRates("USD");
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.CurrencyRatesRequest);

        json.Should().Be("{\"fromCurrency\":\"USD\"}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new CurrencyRate
            {
                CurrencyCode = "GBP",
                Rate = 0.85,
            },
            new CurrencyRate
            {
                CurrencyCode = "EUR",
                Rate = 0.92,
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.CurrencyRates("USD");

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
