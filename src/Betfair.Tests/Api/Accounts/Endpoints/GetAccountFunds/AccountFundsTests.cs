using Betfair.Api;
using Betfair.Api.Accounts.Endpoints.GetAccountFunds.Requests;
using Betfair.Api.Accounts.Endpoints.GetAccountFunds.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountFunds;

public class AccountFundsTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public AccountFundsTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new AccountFundsResponse();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.AccountFunds();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/getAccountFunds/");
    }

    [Fact]
    public async Task PostsDefaultWallet()
    {
        await _api.AccountFunds();

        _client.LastContentSent.Should().BeEquivalentTo(
            new AccountFundsRequest());
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new AccountFundsResponse
        {
            AvailableToBetBalance = 1000.50,
            Exposure = 250.75,
            RetainedCommission = 15.25,
            ExposureLimit = 5000.00,
            DiscountRate = 0.05,
            PointsBalance = 100,
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountFunds();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ResponseWithZeroValuesShouldBeDeserializable()
    {
        var expectedResponse = new AccountFundsResponse
        {
            AvailableToBetBalance = 0.0,
            Exposure = 0.0,
            RetainedCommission = 0.0,
            ExposureLimit = 0.0,
            DiscountRate = 0.0,
            PointsBalance = 0,
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountFunds();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ResponseWithNegativeValuesShouldBeDeserializable()
    {
        var expectedResponse = new AccountFundsResponse
        {
            AvailableToBetBalance = -500.25,
            Exposure = -100.50,
            RetainedCommission = -25.75,
            ExposureLimit = -1000.00,
            DiscountRate = -0.02,
            PointsBalance = -50,
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountFunds();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ResponseWithLargeValuesShouldBeDeserializable()
    {
        var expectedResponse = new AccountFundsResponse
        {
            AvailableToBetBalance = 999999.99,
            Exposure = 888888.88,
            RetainedCommission = 777777.77,
            ExposureLimit = 1000000.00,
            DiscountRate = 0.99,
            PointsBalance = 999999,
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountFunds();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ShouldPassCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        await _api.AccountFunds(token);

        // Verify the call was made (no exception thrown)
        _client.LastUriCalled.Should().NotBeNull();
    }

    [Fact]
    public async Task MultipleCallsShouldWorkCorrectly()
    {
        var expectedResponse = new AccountFundsResponse
        {
            AvailableToBetBalance = 2500.75,
            Exposure = 150.25,
            RetainedCommission = 30.50,
            ExposureLimit = 10000.00,
            DiscountRate = 0.03,
            PointsBalance = 250,
        };
        _client.RespondsWithBody = expectedResponse;

        var response1 = await _api.AccountFunds();
        var response2 = await _api.AccountFunds();

        response1.Should().BeEquivalentTo(expectedResponse);
        response2.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public void ShouldBeVirtualForMocking()
    {
        // Verify that AccountFunds method is virtual for testing/mocking purposes
        var method = typeof(BetfairApiClient).GetMethod(nameof(BetfairApiClient.AccountFunds));

        method.Should().NotBeNull();
        method!.IsVirtual.Should().BeTrue();
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
                _api?.Dispose();
                _client?.Dispose();
            }

            _disposedValue = true;
        }
    }
}
