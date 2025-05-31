﻿using Betfair.Api;
using Betfair.Api.Requests.Account;
using Betfair.Api.Responses.Account;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class AccountFundsTests : IDisposable
{
    private readonly HttpAdapterStub _client = new();
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
            new AccountFundsRequest { Wallet = "UK" });
    }

    [Fact]
    public async Task PostsProvidedWallet()
    {
        await _api.AccountFunds(Wallet.Australian);

        _client.LastContentSent.Should().BeEquivalentTo(
            new AccountFundsRequest { Wallet = "AUSTRALIAN" });
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
            }

            _disposedValue = true;
        }
    }
}
