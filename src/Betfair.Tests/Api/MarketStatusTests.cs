using Betfair.Api;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketStatusTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public MarketStatusTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<MarketBook>();
    }

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
    public async Task IfResponseIsEmptyReturnInactive()
    {
        _client.RespondsWithBody = Array.Empty<MarketBook>();

        var response = await _api.MarketStatus("1.2345", default);

        response.Should().Be(MarketStatus.Inactive);
    }

    [Theory]
    [InlineData(MarketStatus.Inactive)]
    [InlineData(MarketStatus.Open)]
    [InlineData(MarketStatus.Suspended)]
    [InlineData(MarketStatus.Closed)]
    public async Task ReturnStatusFromResponse(MarketStatus status)
    {
        _client.RespondsWithBody = new MarketBook[] { new () { Status = status } };

        var response = await _api.MarketStatus("1.2345", default);

        response.Should().Be(status);
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        await _api.MarketStatus("1.23456789", default);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.MarketBookRequest);

        json.Should().Be("{\"marketIds\":[\"1.23456789\"]}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new MarketBook
            {
                MarketId = "1.23456789",
                Status = MarketStatus.Open,
                IsMarketDataDelayed = false,
                BetDelay = 0,
                BspReconciled = false,
                Complete = true,
                InPlay = false,
                NumberOfWinners = 1,
                NumberOfRunners = 2,
                NumberOfActiveRunners = 2,
                LastMatchTime = DateTimeOffset.UtcNow,
                TotalMatched = 1000.0,
                TotalAvailable = 500.0,
                CrossMatching = true,
                RunnersVoidable = false,
                Version = 123456789,
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.MarketStatus("1.23456789", default);

        response.Should().Be(MarketStatus.Open);
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
            _api.Dispose();
            _client.Dispose();
        }

        _disposedValue = true;
    }
}
