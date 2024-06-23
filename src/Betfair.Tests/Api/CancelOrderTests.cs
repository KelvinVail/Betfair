using Betfair.Api;
using Betfair.Api.Requests.Orders;
using Betfair.Api.Responses.Orders;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class CancelOrderTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public CancelOrderTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new CancelExecutionReport();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.CancelOrders(new CancelOrders());

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/cancelOrders/");
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var cancelOrders = new CancelOrders
        {
            MarketId = "1.2345",
            Instructions = new List<CancelInstruction>
            {
                new ()
                {
                    BetId = "12345",
                    SizeReduction = 2,
                },
            },
        };

        await _api.CancelOrders(cancelOrders);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.CancelOrders);

        json.Should().Be("{\"marketId\":\"1.2345\",\"instructions\":[{\"betId\":\"12345\",\"sizeReduction\":2}]}");
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
