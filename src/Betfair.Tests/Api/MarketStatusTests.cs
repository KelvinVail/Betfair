using Betfair.Api;
using Betfair.Api.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketStatusTests
{
    private readonly BetfairClientStub _client = new ();
    private readonly BetfairApiClient _api;

    public MarketStatusTests() =>
        _api = new BetfairApiClient(_client);

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.MarketStatus("1.23456789", default);

        _client.LastUriCalled.Should().Be("https://api.betfair.com/exchange/betting/rest/v1.0/listMarketBook/");
    }

    [Theory]
    [InlineData("1.23456789")]
    [InlineData("1.99999999")]
    public async Task MarketIdShouldBeAddedToTheRequestBody(string marketId)
    {
        await _api.MarketStatus(marketId, default);

        _client.LastContentSent.Should().BeEquivalentTo(new { MarketIds = new List<string> { marketId } });
    }

    [Fact]
    public async Task IfResponseIsEmptyReturnNone()
    {
        _client.RespondsWithBody = new List<MarketStatus>();

        var response = await _api.MarketStatus("1.2345", default);

        response.Should().Be("NONE");
    }

    [Theory]
    [InlineData("INACTIVE")]
    [InlineData("OPEN")]
    [InlineData("SUSPENDED")]
    [InlineData("CLOSED")]
    public async Task ReturnStatusFromResponse(string status)
    {
        _client.RespondsWithBody = new List<MarketStatus> { new () { Status = status } };

        var response = await _api.MarketStatus("1.2345", default);

        response.Should().Be(status);
    }
}
