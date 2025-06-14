using Betfair.Api;
using Betfair.Api.Requests.Account;
using Betfair.Api.Responses.Account;
using Betfair.Core.Enums;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class TransferFundsTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public TransferFundsTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new TransferResponse();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.TransferFunds(Wallet.Uk, Wallet.Australian, 100.0);

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/transferFunds/");
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        await _api.TransferFunds(Wallet.Uk, Wallet.Australian, 100.0);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.TransferFundsRequest);

        json.Should().Be("{\"from\":\"UK\",\"to\":\"AUSTRALIAN\",\"amount\":100}");
    }

    [Fact]
    public async Task PostsCorrectWalletNames()
    {
        await _api.TransferFunds(Wallet.Uk, Wallet.Australian, 50.25);

        _client.LastContentSent.Should().BeEquivalentTo(
            new TransferFundsRequest
            {
                From = "UK",
                To = "AUSTRALIAN",
                Amount = 50.25,
            });
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new TransferResponse
        {
            TransactionId = "TXN123456789",
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.TransferFunds(Wallet.Uk, Wallet.Australian, 100.0);

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
