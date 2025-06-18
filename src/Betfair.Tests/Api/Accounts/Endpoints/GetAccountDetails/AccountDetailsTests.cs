using Betfair.Api;
using Betfair.Api.Accounts.Endpoints.GetAccountDetails.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountDetails;

public class AccountDetailsTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public AccountDetailsTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new AccountDetailsResponse();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.AccountDetails();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/getAccountDetails/");
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        await _api.AccountDetails();
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.AccountDetailsRequest);

        json.Should().Be("{}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new AccountDetailsResponse
        {
            CurrencyCode = "GBP",
            FirstName = "John",
            LastName = "Doe",
            LocaleCode = "en_GB",
            Region = "GBR",
            Timezone = "GMT",
            DiscountRate = 0.05,
            PointsBalance = 100,
            CountryCode = "GB",
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountDetails();

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
