﻿using Betfair.Api;
using Betfair.Api.Betting;
using Betfair.Api.Betting.Endpoints.ListEvents.Requests;
using Betfair.Api.Betting.Endpoints.ListEvents.Responses;
using Betfair.Api.Betting.Endpoints.ListEventTypes.Requests;
using Betfair.Api.Betting.Endpoints.ListEventTypes.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class BetfairEventTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public BetfairEventTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<EventResult>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.Events();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listEvents/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _api.Events();

        _client.LastContentSent.Should().BeEquivalentTo(
            new EventsRequest());
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.Events(filter);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.EventsRequest);

        json.Should().Be("{\"filter\":{\"marketIds\":[\"1.23456789\"]}}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new EventResult
            {
                Event = new EventType
                {
                    Id = "29301",
                    Name = "Test BetfairEvent",
                },
                MarketCount = 5,
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.Events();

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
