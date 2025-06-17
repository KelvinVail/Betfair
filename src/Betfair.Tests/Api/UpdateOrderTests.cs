using Betfair.Api;
using Betfair.Api.Betting.Endpoints.PlaceOrders.Responses;
using Betfair.Api.Betting.Endpoints.UpdateOrders.Requests;
using Betfair.Api.Betting.Endpoints.UpdateOrders.Responses;
using Betfair.Api.Betting.Enums;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class UpdateOrderTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public UpdateOrderTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new PlaceExecutionReport();
    }

    [Fact]
    public async Task UpdateOrdersCallsTheCorrectEndpoint()
    {
        await _api.UpdateOrders(new UpdateOrders("1.23456789"));

        _client.LastUriCalled.Should().Be(
                       "https://api.betfair.com/exchange/betting/rest/v1.0/updateOrders/");
    }

    [Fact]
    public async Task UpdateOrdersRequestBodyShouldBeSerializable()
    {
        var updateOrders = new UpdateOrders("1.23456789")
        {
            Instructions = new List<UpdateInstruction>
            {
                new ()
                {
                    BetId = "1.23456789",
                    NewPersistenceType = PersistenceType.Lapse,
                },
            },
        };

        await _api.UpdateOrders(updateOrders);

        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.UpdateOrders);

        json.Should().Be("{\"marketId\":\"1.23456789\",\"instructions\":[{\"betId\":\"1.23456789\",\"newPersistenceType\":\"LAPSE\"}]}");
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
