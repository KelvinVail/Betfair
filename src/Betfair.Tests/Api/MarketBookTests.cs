using Betfair.Api;
using Betfair.Api.Betting.Endpoints.ListMarketBook;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketBookTests : IDisposable
{
    private readonly HttpAdapterStub _client = new();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public MarketBookTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<MarketBook>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.MarketBook(new[] { "1.23456789" });

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listMarketBook/");
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var query = new MarketBookQuery()
            .WithCurrency("GBP")
            .IncludeOverallPositions();

        await _api.MarketBook(new[] { "1.23456789" }, query);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.MarketBookRequest);

        json.Should().Contain("\"marketIds\":[\"1.23456789\"]");
        json.Should().Contain("\"currencyCode\":\"GBP\"");
        json.Should().Contain("\"includeOverallPosition\":true");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new MarketBook
            {
                MarketId = "1.23456789",
                IsMarketDataDelayed = false,
                Status = MarketStatus.Open,
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
                Runners = new List<Runner>
                {
                    new Runner
                    {
                        SelectionId = 47972,
                        Handicap = 0,
                        Status = RunnerStatus.Active,
                        AdjustmentFactor = 1.0,
                        LastPriceTraded = 2.5,
                        TotalMatched = 500.0,
                        RemovalDate = null,
                        ExchangePrices = new ExchangePrices
                        {
                            AvailableToBack = new List<List<double>>
                            {
                                new List<double> { 2.5, 100.0 },
                            },
                            AvailableToLay = new List<List<double>>
                            {
                                new List<double> { 2.6, 100.0 },
                            },
                            TradedVolume = new List<List<double>>
                            {
                                new List<double> { 2.4, 50.0 },
                            },
                        },
                    },
                },
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.MarketBook(new[] { "1.23456789" });

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
