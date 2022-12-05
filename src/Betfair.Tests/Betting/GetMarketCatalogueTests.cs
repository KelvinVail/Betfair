using Betfair.Betting;
using Betfair.Betting.Models;
using Betfair.Errors;
using Betfair.Tests.Errors;
using Betfair.Tests.TestDoubles;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Tests.Betting;

public class GetMarketCatalogueTests : IDisposable
{
    private readonly BetfairHttpClientFake _httpClient = new ();
    private readonly BettingClient _client;
    private bool _disposedValue;

    public GetMarketCatalogueTests() =>
        _client = new BettingClient(_httpClient);

    [Fact]
    public async Task ReturnErrorsFromClient()
    {
        var error = ErrorResult.Empty("sessionToken");
        _httpClient.SetError = error;

        var result = await _client.MarketCatalogue("token");

        result.ShouldBeFailure(error);
    }

    [Theory]
    [InlineData("token")]
    [InlineData("newSessionToken")]
    public async Task SessionTokenIsPassedToClient(string token)
    {
        await _client.MarketCatalogue(token);

        _httpClient.LastSessionTokenUsed.Should().Be(token);
    }

    [Fact]
    public async Task CallsTheCorrectUri()
    {
        var expectedUri = new Uri("https://api.betfair.com/exchange/betting/rest/v1.0/listMarketCatalogue/");

        await _client.MarketCatalogue("token");

        _httpClient.LastUriCalled.Should().Be(expectedUri);
    }

    [Fact]
    public async Task EmptyMarketFilterWithDefaultMaxResultsIsSentToClientAsDefault()
    {
        const string expected = "{\"filter\":{},\"maxResults\":1000}";

        await _client.MarketCatalogue("token");

        var actual =
            JsonSerializer.ToJsonString(_httpClient.LastBodySent, StandardResolver.AllowPrivateExcludeNullCamelCase);
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("GB")]
    [InlineData("US")]
    public async Task MarketFilterIsSentToClient(string code)
    {
        var expected = $"{{\"filter\":{{\"marketCountries\":[\"{code}\"]}},\"maxResults\":1000}}";
        var filter = new MarketFilter().WithCountryCode(code);

        await _client.MarketCatalogue("token", filter);

        var actual =
            JsonSerializer.ToJsonString(_httpClient.LastBodySent, StandardResolver.AllowPrivateExcludeNullCamelCase);
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task MarketProjectionIsSentToClient()
    {
        const string expected = "{\"filter\":{},\"marketProjection\":[\"MARKET_START_TIME\"],\"maxResults\":1000}";
        var projection = new MarketProjection().WithMarketStartTime();

        await _client.MarketCatalogue("token", marketProjection: projection);

        var actual =
            JsonSerializer.ToJsonString(_httpClient.LastBodySent, StandardResolver.AllowPrivateExcludeNullCamelCase);
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task MarketSortIsSentToClient()
    {
        const string expected = "{\"filter\":{},\"sort\":\"FIRST_TO_START\",\"maxResults\":1000}";
        var sort = MarketSort.FirstToStart;

        await _client.MarketCatalogue("token", marketSort: sort);

        var actual =
            JsonSerializer.ToJsonString(_httpClient.LastBodySent, StandardResolver.AllowPrivateExcludeNullCamelCase);
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(10)]
    [InlineData(782)]
    public async Task MaxResultsIsSentToClient(int maxResults)
    {
        var expected = $"{{\"filter\":{{}},\"maxResults\":{maxResults}}}";

        await _client.MarketCatalogue("token", maxResults: maxResults);

        var actual =
            JsonSerializer.ToJsonString(_httpClient.LastBodySent, StandardResolver.AllowPrivateExcludeNullCamelCase);
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task ResponseTypeIsListOfMarketCatalogue()
    {
        await _client.MarketCatalogue("token");

        _httpClient.ResponseType.Should().Be(typeof(IReadOnlyList<MarketCatalogue>));
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
            _httpClient.Dispose();

        _disposedValue = true;
    }
}
