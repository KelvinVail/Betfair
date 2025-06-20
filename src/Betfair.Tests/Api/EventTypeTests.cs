﻿using Betfair.Api;
using Betfair.Api.Betting;
using Betfair.Api.Betting.Endpoints.ListEventTypes.Requests;
using Betfair.Api.Betting.Endpoints.ListEventTypes.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class EventTypeTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public EventTypeTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<EventTypeResult>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.EventTypes();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listEventTypes/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _api.EventTypes();

        _client.LastContentSent.Should().BeEquivalentTo(
            new EventTypesRequest());
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.EventTypes(filter);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.EventTypesRequest);

        json.Should().Be("{\"filter\":{\"marketIds\":[\"1.23456789\"]}}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new EventTypeResult
            {
                EventType = new EventType
                {
                    Id = "1",
                    Name = "Soccer",
                },
                MarketCount = 150,
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.EventTypes();

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
