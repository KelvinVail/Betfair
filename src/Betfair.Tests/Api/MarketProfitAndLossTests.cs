using Betfair.Api;
using Betfair.Api.Betting.Endpoints.ListMarketProfitAndLoss;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class MarketProfitAndLossTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public MarketProfitAndLossTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<MarketProfitAndLoss>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.MarketProfitAndLoss(["1.2345"]);

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listMarketProfitAndLoss/");
    }

    [Theory]
    [InlineData("1.2345")]
    [InlineData("1.9876")]
    public async Task PostsMarketIsdAndDefaults(string marketId)
    {
        await _api.MarketProfitAndLoss([marketId]);

        _client.LastContentSent.Should().BeEquivalentTo(
            new
            {
                MarketIds = new[] { marketId },
                IncludeSettledBets = false,
                IncludeBspBets = false,
                NetOfCommission = false,
            });
    }

    [Fact]
    public async Task PostsIncludeSettledBets()
    {
        await _api.MarketProfitAndLoss(["1.2345"], includeSettledBets: true);

        _client.LastContentSent.Should().BeEquivalentTo(
            new
            {
                MarketIds = new List<string> { "1.2345" },
                IncludeSettledBets = true,
                IncludeBspBets = false,
                NetOfCommission = false,
            });
    }

    [Fact]
    public async Task PostsIncludeBspBets()
    {
        await _api.MarketProfitAndLoss(["1.2345"], includeBspBets: true);

        _client.LastContentSent.Should().BeEquivalentTo(
            new
            {
                MarketIds = new List<string> { "1.2345" },
                IncludeSettledBets = false,
                IncludeBspBets = true,
                NetOfCommission = false,
            });
    }

    [Fact]
    public async Task PostsNetOfCommission()
    {
        await _api.MarketProfitAndLoss(["1.2345"], netOfCommission: true);

        _client.LastContentSent.Should().BeEquivalentTo(
            new
            {
                MarketIds = new List<string> { "1.2345" },
                IncludeSettledBets = false,
                IncludeBspBets = false,
                NetOfCommission = true,
            });
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        await _api.MarketProfitAndLoss(["1.23456789"], includeSettledBets: true, includeBspBets: true, netOfCommission: true);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.MarketProfitAndLossRequest);

        json.Should().Be("{\"marketIds\":[\"1.23456789\"],\"includeSettledBets\":true,\"includeBspBets\":true,\"netOfCommission\":true}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new List<MarketProfitAndLoss>
        {
            new MarketProfitAndLoss
            {
                MarketId = "1.23456789",
                CommissionApplied = 0.05,
                ProfitAndLosses = new[]
                {
                    new RunnerProfitAndLoss
                    {
                        SelectionId = 47972,
                        IfWin = 10.50,
                        IfLose = -5.25,
                        IfPlace = 2.75
                    }
                }
            }
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.MarketProfitAndLoss(["1.23456789"]);

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
