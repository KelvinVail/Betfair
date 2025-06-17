using Betfair.Api;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Requests;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketBookTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
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
                            AvailableToBack = new List<PriceSize>
                            {
                                new PriceSize { Price = 2.5, Size = 100.0 },
                            },
                            AvailableToLay = new List<PriceSize>
                            {
                                new PriceSize { Price = 2.6, Size = 100.0 },
                            },
                            TradedVolume = new List<PriceSize>
                            {
                                new PriceSize { Price = 2.4, Size = 50.0 },
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

    [Fact]
    public void RealWorldJsonShouldBeDeserializable()
    {
        const string json = """
            [{"marketId":"1.244846171","isMarketDataDelayed":false,"status":"OPEN","betDelay":0,"bspReconciled":false,"complete":true,"inplay":false,"numberOfWinners":1,"numberOfRunners":20,"numberOfActiveRunners":20,"lastMatchTime":"2025-06-17T15:55:19.851Z","totalMatched":974693.96,"totalAvailable":3.375962291E7,"crossMatching":true,"runnersVoidable":false,"version":6701554910,"runners":[{"selectionId":61291656,"handicap":0.0,"status":"ACTIVE","adjustmentFactor":22.772,"lastPriceTraded":4.3,"totalMatched":297427.02,"ex":{"availableToBack":[{"price":4.3,"size":768.84},{"price":4.2,"size":4371.66},{"price":4.1,"size":2651.28}],"availableToLay":[{"price":4.4,"size":2456.5},{"price":4.5,"size":1206.82},{"price":4.6,"size":1491.02}],"tradedVolume":[]}}]}]
            """;

        var response = JsonSerializer.Deserialize(json, SerializerContext.Default.MarketBookArray);

        response.Should().NotBeNull();
        response.Should().HaveCount(1);

        var marketBook = response![0];
        marketBook.MarketId.Should().Be("1.244846171");
        marketBook.Runners.Should().HaveCount(1);

        var runner = marketBook.Runners![0];
        runner.SelectionId.Should().Be(61291656);
        runner.ExchangePrices.Should().NotBeNull();
        runner.ExchangePrices!.AvailableToBack.Should().HaveCount(3);
        runner.ExchangePrices.AvailableToBack![0].Price.Should().Be(4.3);
        runner.ExchangePrices.AvailableToBack[0].Size.Should().Be(768.84);
        runner.ExchangePrices.AvailableToLay.Should().HaveCount(3);
        runner.ExchangePrices.AvailableToLay![0].Price.Should().Be(4.4);
        runner.ExchangePrices.AvailableToLay[0].Size.Should().Be(2456.5);
        runner.ExchangePrices.TradedVolume.Should().HaveCount(0);
    }

    [Fact]
    public async Task OriginalFailingJsonShouldNowWork()
    {
        const string json = @"[{""marketId"":""1.244846171"",""isMarketDataDelayed"":false,""status"":""OPEN"",""betDelay"":0,""bspReconciled"":false,""complete"":true,""inplay"":false,""numberOfWinners"":1,""numberOfRunners"":20,""numberOfActiveRunners"":20,""lastMatchTime"":""2025-06-17T15:55:19.851Z"",""totalMatched"":974693.96,""totalAvailable"":3.375962291E7,""crossMatching"":true,""runnersVoidable"":false,""version"":6701554910,""runners"":[{""selectionId"":61291656,""handicap"":0.0,""status"":""ACTIVE"",""adjustmentFactor"":22.772,""lastPriceTraded"":4.3,""totalMatched"":297427.02,""ex"":{""availableToBack"":[{""price"":4.3,""size"":768.84},{""price"":4.2,""size"":4371.66},{""price"":4.1,""size"":2651.28}],""availableToLay"":[{""price"":4.4,""size"":2456.5},{""price"":4.5,""size"":1206.82},{""price"":4.6,""size"":1491.02}],""tradedVolume"":[]}},{""selectionId"":59505697,""handicap"":0.0,""status"":""ACTIVE"",""adjustmentFactor"":20.833,""lastPriceTraded"":4.4,""totalMatched"":248166.17,""ex"":{""availableToBack"":[{""price"":4.3,""size"":3884.84},{""price"":4.2,""size"":2556.63},{""price"":4.1,""size"":1291.72}],""availableToLay"":[{""price"":4.4,""size"":595.11},{""price"":4.5,""size"":1070.35},{""price"":4.6,""size"":792.06}],""tradedVolume"":[]}},{""selectionId"":62041210,""handicap"":0.0,""status"":""ACTIVE"",""adjustmentFactor"":17.241,""lastPriceTraded"":5.6,""totalMatched"":169912.68,""ex"":{""availableToBack"":[{""price"":5.6,""size"":669.36},{""price"":5.5,""size"":1973.49},{""price"":5.4,""size"":872.55}],""availableToLay"":[{""price"":5.7,""size"":1191.87},{""price"":5.8,""size"":1835.52},{""price"":5.9,""size"":803.3}],""tradedVolume"":[]}}]}]";

        _client.RespondsWithBody = json;

        var response = await _api.MarketBook(new[] { "1.244846171" });

        response.Should().NotBeNull();
        response.Should().HaveCount(1);

        var marketBook = response[0];
        marketBook.MarketId.Should().Be("1.244846171");
        marketBook.Runners.Should().HaveCount(3);

        var firstRunner = marketBook.Runners![0];
        firstRunner.SelectionId.Should().Be(61291656);
        firstRunner.ExchangePrices.Should().NotBeNull();
        firstRunner.ExchangePrices!.AvailableToBack.Should().HaveCount(3);
        firstRunner.ExchangePrices.AvailableToBack![0].Price.Should().Be(4.3);
        firstRunner.ExchangePrices.AvailableToBack[0].Size.Should().Be(768.84);
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
