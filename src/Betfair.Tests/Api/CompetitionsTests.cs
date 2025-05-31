﻿using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class CompetitionsTests : IDisposable
{
    private readonly HttpAdapterStub _client = new();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public CompetitionsTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<CompetitionResult>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.Competitions();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listCompetitions/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _api.Competitions();

        _client.LastContentSent.Should().BeEquivalentTo(
            new CompetitionsRequest());
    }

    [Fact]
    public async Task PostsFilterIfProvided()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.Competitions(filter);

        _client.LastContentSent.Should().BeEquivalentTo(
            new CompetitionsRequest { Filter = filter });
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
