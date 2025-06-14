using Betfair.Api;
using Betfair.Api.Requests.Orders.Queries;
using Betfair.Api.Responses.Orders;
using Betfair.Core.Enums;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class ClearedOrdersTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public ClearedOrdersTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new ClearedOrderSummaryReport();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        var query = new ClearedOrdersQuery();
        await _api.ClearedOrders(query);

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listClearedOrders/");
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var query = new ClearedOrdersQuery()
            .WithMarkets("1.23456789")
            .BackBetsOnly()
            .SettledOnly();

        await _api.ClearedOrders(query);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.ClearedOrdersRequest);

        json.Should().Contain("\"marketIds\":[\"1.23456789\"]");
        json.Should().Contain("\"side\":\"BACK\"");
        json.Should().Contain("\"betStatus\":\"SETTLED\"");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new ClearedOrderSummaryReport
        {
            ClearedOrders = new List<ClearedOrder>
            {
                new ClearedOrder
                {
                    BetId = "123456789",
                    MarketId = "1.23456789",
                    SelectionId = 47972,
                    Handicap = 0,
                    BetOutcome = "WON",
                    PriceRequested = 2.5,
                    SettledDate = DateTime.UtcNow,
                    Commission = 0.05,
                    Profit = 1.45,
                    SizeCancelled = 0,
                    SizeSettled = 1.0,

                    Side = "BACK",
                    ItemDescription = new ItemDescription
                    {
                        EventTypeDesc = "Soccer",
                        EventDesc = "Test Match",
                        MarketDesc = "Match Odds",
                        RunnerDesc = "Team A"
                    }
                }
            },
            MoreAvailable = false
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.ClearedOrders(new ClearedOrdersQuery());

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
