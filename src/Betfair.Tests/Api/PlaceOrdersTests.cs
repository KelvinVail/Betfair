using Betfair.Api;
using Betfair.Api.Betting.Endpoints.PlaceOrders.Requests;
using Betfair.Api.Betting.Endpoints.PlaceOrders.Responses;
using Betfair.Api.Betting.Enums;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class PlaceOrdersTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public PlaceOrdersTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new PlaceExecutionReport();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.PlaceOrders(new PlaceOrders("1.2345"));

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/placeOrders/");
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        await _api.PlaceOrders(new PlaceOrders("1.2345"));
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.PlaceOrders);

        json.Should().Be("{\"marketId\":\"1.2345\",\"instructions\":[]}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new PlaceExecutionReport
        {
            MarketId = "1.23456789",
            Status = "SUCCESS",
            ErrorCode = null,
            CustomerRef = "test-ref",
            InstructionReports = new List<PlaceInstructionReport>
            {
                new PlaceInstructionReport
                {
                    Status = "SUCCESS",
                    ErrorCode = null,
                    BetId = "123456789",
                    PlacedDate = DateTimeOffset.UtcNow,
                    AveragePriceMatched = 2.5,
                    SizeMatched = 10.0,
                    OrderStatus = "EXECUTION_COMPLETE",
                    Instruction = new PlaceInstruction
                    {
                        SelectionId = 47972,
                        Handicap = 0,
                        Side = Side.Back,
                        OrderType = OrderType.Limit,
                        LimitOrder = new LimitOrder
                        {
                            Size = 10.0,
                            Price = 2.5,
                            PersistenceType = PersistenceType.Lapse,
                        },
                    },
                },
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.PlaceOrders(new PlaceOrders("1.23456789"));

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
