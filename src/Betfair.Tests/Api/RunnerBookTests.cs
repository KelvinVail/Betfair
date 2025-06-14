using Betfair.Api;
using Betfair.Api.Requests.Markets;
using Betfair.Api.Responses.Markets;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class RunnerBookTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public RunnerBookTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new[]
        {
            new MarketBook
            {
                MarketId = "1.23456789",
                Status = "OPEN"
            }
        };
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.RunnerBook("1.23456789", 47972);

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listRunnerBook/");
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var query = new MarketBookQuery()
            .WithCurrency("GBP")
            .IncludeOverallPositions();

        await _api.RunnerBook("1.23456789", 47972, 0.5, query);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.RunnerBookRequest);

        json.Should().Contain("\"marketId\":\"1.23456789\"");
        json.Should().Contain("\"selectionId\":47972");
        json.Should().Contain("\"handicap\":0.5");
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
                Status = "OPEN",
                BetDelay = 0,
                BspReconciled = false,
                Complete = true,
                InPlay = false,
                NumberOfWinners = 1,
                NumberOfRunners = 1,
                NumberOfActiveRunners = 1,
                LastMatchTime = DateTimeOffset.UtcNow,
                TotalMatched = 500.0,
                TotalAvailable = 250.0,
                CrossMatching = true,
                RunnersVoidable = false,
                Version = 123456789,
                Runners = new List<Runner>
                {
                    new Runner
                    {
                        SelectionId = 47972,
                        Handicap = 0,
                        Status = "ACTIVE",
                        AdjustmentFactor = 1.0,
                        LastPriceTraded = 2.5,
                        TotalMatched = 500.0,
                        RemovalDate = null,
                        ExchangePrices = new ExchangePrices
                        {
                            AvailableToBack = new List<List<double>>
                            {
                                new List<double> { 2.5, 100.0 }
                            },
                            AvailableToLay = new List<List<double>>
                            {
                                new List<double> { 2.6, 100.0 }
                            },
                            TradedVolume = new List<List<double>>
                            {
                                new List<double> { 2.4, 50.0 }
                            }
                        }
                    }
                }
            }
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.RunnerBook("1.23456789", 47972);

        response.Should().BeEquivalentTo(expectedResponse.FirstOrDefault());
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
