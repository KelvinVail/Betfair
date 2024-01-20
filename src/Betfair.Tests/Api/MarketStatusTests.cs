using Betfair.Api;
using Betfair.Api.Responses;
using Betfair.Tests.Core.Client.TestDoubles;
using Betfair.Tests.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketStatusTests : IDisposable
{
    private readonly TokenProviderStub _provider = new ();
    private readonly HttpMessageHandlerSpy _handler = new();
    private readonly HttpClientStub _httpClient;

    private readonly BetfairApiClient _client;
    private bool _disposedValue;

    public MarketStatusTests()
    {
        _httpClient = new HttpClientStub(_handler);
        _client = new BetfairApiClient(_httpClient, _provider);
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _client.MarketStatus("1.23456789", default);

        _handler.UriCalled.Should().Be("https://api.betfair.com/exchange/betting/rest/v1.0/listMarketBook/");
    }

    [Theory]
    [InlineData("1.23456789")]
    [InlineData("1.99999999")]
    public async Task MarketIdShouldBeAddedToTheRequestBody(string marketId)
    {
        await _client.MarketStatus(marketId, default);

        _handler.ContentSent.Should().BeEquivalentTo(new { MarketIds = new List<string> { marketId } });
    }

    [Fact]
    public async Task IfResponseIsEmptyReturnNone()
    {
        _handler.RespondsWithBody = new List<MarketStatus>();

        var response = await _client.MarketStatus("1.2345", default);

        response.Should().Be("NONE");
    }

    [Theory]
    [InlineData("INACTIVE")]
    [InlineData("OPEN")]
    [InlineData("SUSPENDED")]
    [InlineData("CLOSED")]
    public async Task ReturnStatusFromResponse(string status)
    {
        _handler.RespondsWithBody = new List<MarketStatus> { new () { Status = status } };

        var response = await _client.MarketStatus("1.2345", default);

        response.Should().Be(status);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _httpClient.Dispose();
        _disposedValue = true;
    }
}
