using Betfair.Api;
using Betfair.Api.Requests.Orders;
using Betfair.Api.Responses.Orders;
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
