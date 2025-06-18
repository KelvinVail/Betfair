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

    [Fact]
    public async Task CanHandleEmptyResponse()
    {
        _client.RespondsWithBody = Array.Empty<CurrencyRate>();

        var response = await _api.CurrencyRates();

        response.Should().NotBeNull();
        response.Should().BeEmpty();
    }

    [Fact]
    public async Task CanHandleLargeResponse()
    {
        var expectedResponse = new CurrencyRate[100];
        for (int i = 0; i < 100; i++)
        {
            expectedResponse[i] = new CurrencyRate
            {
                CurrencyCode = $"CUR{i:D3}",
                Rate = i * 0.01,
            };
        }

        _client.RespondsWithBody = expectedResponse;

        var response = await _api.CurrencyRates("USD");

        response.Should().HaveCount(100);
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("GBP")]
    [InlineData("EUR")]
    [InlineData("JPY")]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CanHandleDifferentFromCurrencyValues(string fromCurrency)
    {
        await _api.CurrencyRates(fromCurrency);

        _client.LastContentSent.Should().BeEquivalentTo(
            new CurrencyRatesRequest { FromCurrency = fromCurrency });
    }

    [Fact]
    public async Task CanPassCancellationToken()
    {
        using var cts = new CancellationTokenSource();

        var response = await _api.CurrencyRates(cancellationToken: cts.Token);

        response.Should().NotBeNull();
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
