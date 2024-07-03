using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Api.Responses;
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
        _client.RespondsWithBody = Array.Empty<MarketCatalogue>();
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
