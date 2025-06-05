﻿using Betfair.Api;
using Betfair.Api.Requests.Orders;
using Betfair.Api.Responses.Orders;
using Betfair.Core;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class CurrentOrdersTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
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
            new CurrentOrdersRequest());
    }

    [Fact]
    public async Task PostsProvidedParameters()
    {
        var betIds = new[] { "123", "456" };
        var marketIds = new[] { "1.123", "1.456" };

        var filter = new ApiOrderFilter()
            .WithBetIds(betIds)
            .WithMarketIds(marketIds)
            .WithOrderStatus(OrderStatus.Executable)
            .From(10)
            .Take(50);

        await _api.CurrentOrders(filter);

        _client.LastContentSent.Should().BeEquivalentTo(
            new CurrentOrdersRequest
            {
                BetIds = betIds.ToList(),
                MarketIds = marketIds.ToList(),
                OrderProjection = OrderStatus.Executable,
                FromRecord = 10,
                RecordCount = 50,
            });
    }

    [Fact]
    public async Task PostsFilterWithExecutableOnly()
    {
        var filter = new ApiOrderFilter()
            .ExecutableOnly()
            .MostRecentFirst()
            .Take(100);

        await _api.CurrentOrders(filter);

        _client.LastContentSent.Should().BeEquivalentTo(
            new CurrentOrdersRequest
            {
                OrderProjection = OrderStatus.Executable,
                SortDir = SortDir.LatestToEarliest,
                RecordCount = 100,
            });
    }

    [Fact]
    public async Task PostsFilterWithCustomerRefs()
    {
        var customerOrderRefs = new[] { "ref1", "ref2" };
        var customerStrategyRefs = new[] { "strategy1", "strategy2" };

        var filter = new ApiOrderFilter()
            .WithCustomerOrderRefs(customerOrderRefs)
            .WithCustomerStrategyRefs(customerStrategyRefs)
            .ExecutionCompleteOnly();

        await _api.CurrentOrders(filter);

        _client.LastContentSent.Should().BeEquivalentTo(
            new CurrentOrdersRequest
            {
                OrderProjection = OrderStatus.ExecutionComplete,
                CustomerOrderRefs = customerOrderRefs.ToList(),
                CustomerStrategyRefs = customerStrategyRefs.ToList(),
            });
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var customerOrderRefs = new[] { "ref1", "ref2" };
        var filter = new ApiOrderFilter()
            .WithCustomerOrderRefs(customerOrderRefs)
            .ExecutionCompleteOnly();

        await _api.CurrentOrders(filter);

        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.CurrentOrdersRequest);
        json.Should().Be("{\"orderProjection\":\"EXECUTION_COMPLETE\",\"customerOrderRefs\":[\"ref1\",\"ref2\"],\"fromRecord\":0,\"recordCount\":1000}");
    }

    [Fact]
    public async Task TheResponseCanBeDeserialized()
    {
        var date = DateTimeOffset.UtcNow;
        var currentOrder = new CurrentOrder
        {
            BetId = "12345",
            MarketId = "1.23456789",
            SelectionId = 123456,
            Handicap = 0.0,
            PriceSize = new PriceSize
            {
                Price = 2.0,
                Size = 100.0,
            },
            BspLiability = 10.0,
            Side = Side.Back,
            Status = OrderStatus.Executable,
            PersistenceType = PersistenceType.Lapse,
            OrderType = OrderType.Limit,
            PlacedDate = date,
            MatchedDate = date,
            AveragePriceMatched = 9.99,
            SizeMatched = 50.0,
            SizeRemaining = 50.0,
            SizeLapsed = 0.0,
            SizeCancelled = 0.0,
            SizeVoided = 0.0,
            RegulatorAuthCode = "AUTH123",
            RegulatorCode = "REG123",
            CustomerOrderRef = "order123",
            CustomerStrategyRef = "strategy123",
        };

        _client.RespondsWithBody = new CurrentOrderSummaryReport
        {
            CurrentOrders = [currentOrder],
            MoreAvailable = true,
        };

        var response = await _api.CurrentOrders();
        response.CurrentOrders.Should().HaveCount(1);

        var order = response.CurrentOrders[0];
        order.Should().BeEquivalentTo(currentOrder);
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
                _client.Dispose();
                _api?.Dispose();
            }

            _disposedValue = true;
        }
    }
}
