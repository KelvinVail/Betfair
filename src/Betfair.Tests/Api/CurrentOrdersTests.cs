﻿using Betfair.Api;
using Betfair.Api.Requests.Orders;
using Betfair.Api.Responses.Orders;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class CurrentOrdersTests : IDisposable
{
    private readonly HttpAdapterStub _client = new();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public CurrentOrdersTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new CurrentOrderSummaryReport();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.CurrentOrders();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listCurrentOrders/");
    }

    [Fact]
    public async Task PostsDefaultParameters()
    {
        await _api.CurrentOrders();

        _client.LastContentSent.Should().BeEquivalentTo(
            new CurrentOrdersRequest
            {
                BetIds = null,
                MarketIds = null,
                OrderProjection = "ALL",
                CustomerOrderRefs = null,
                CustomerStrategyRefs = null,
                DateRange = null,
                OrderBy = "PLACEDDATE",
                SortDir = "EARLIESTTOLATEST",
                FromRecord = 0,
                RecordCount = 1000
            });
    }

    [Fact]
    public async Task PostsProvidedParameters()
    {
        var betIds = new[] { "123", "456" };
        var marketIds = new[] { "1.123", "1.456" };

        await _api.CurrentOrders(
            betIds: betIds,
            marketIds: marketIds,
            orderProjection: OrderProjection.Executable,
            fromRecord: 10,
            recordCount: 50);

        _client.LastContentSent.Should().BeEquivalentTo(
            new CurrentOrdersRequest
            {
                BetIds = betIds.ToList(),
                MarketIds = marketIds.ToList(),
                OrderProjection = "EXECUTABLE",
                CustomerOrderRefs = null,
                CustomerStrategyRefs = null,
                DateRange = null,
                OrderBy = "PLACEDDATE",
                SortDir = "EARLIESTTOLATEST",
                FromRecord = 10,
                RecordCount = 50
            });
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
