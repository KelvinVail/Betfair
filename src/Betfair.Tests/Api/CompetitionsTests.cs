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

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.Competitions(filter);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.CompetitionsRequest);

        json.Should().Be("{\"filter\":{\"marketIds\":[\"1.23456789\"]}}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        _client.RespondsWithBody = new[]
        {
            new CompetitionResult
            {
                MarketCount = 2,
                CompetitionRegion = "Europe",
                Competition = new Competition
                {
                    Id = "1",
                    Name = "Test Competition",
                },
            },
        };

        var response = await _api.Competitions();

        response.Should().HaveCount(1);
        response[0].MarketCount.Should().Be(2);
        response[0].CompetitionRegion.Should().Be("Europe");
        response[0].Competition!.Id.Should().Be("1");
        response[0].Competition!.Name.Should().Be("Test Competition");
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
