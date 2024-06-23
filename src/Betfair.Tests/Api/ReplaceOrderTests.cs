using Betfair.Api;
using Betfair.Api.Requests.Orders;
using Betfair.Api.Responses.Orders;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class ReplaceOrderTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public ReplaceOrderTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new ReplaceExecutionReport();
    }

    [Fact]
    public async Task ReplaceOrdersCallsTheCorrectEndpoint()
    {
        await _api.ReplaceOrders(new ReplaceOrders("1.23456789"));

        _client.LastUriCalled.Should().Be(
                       "https://api.betfair.com/exchange/betting/rest/v1.0/replaceOrders/");
    }

    [Fact]
    public async Task ReplaceOrdersRequestBodyShouldBeSerializable()
    {
        var replaceOrders = new ReplaceOrders("1.23456789");
        replaceOrders.Instructions.Add(new ReplaceInstruction { BetId = "12345", NewPrice = 1.01 });

        await _api.ReplaceOrders(replaceOrders);

        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.ReplaceOrders);

        json.Should().Be("{\"marketId\":\"1.23456789\",\"instructions\":[{\"betId\":\"12345\",\"newPrice\":1.01}]}");
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
