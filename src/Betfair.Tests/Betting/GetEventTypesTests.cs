using Betfair.Betting;
using Betfair.Client;
using Betfair.Errors;
using Betfair.Tests.Errors;
using Betfair.Tests.TestDoubles;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Tests.Betting;

public class GetEventTypesTests : IDisposable
{
    private readonly BetfairHttpClientFake _httpClient = new ();
    private readonly BettingClient _client;
    private bool _disposedValue;

    public GetEventTypesTests() =>
        _client = new BettingClient(_httpClient);

    [Fact]
    public async Task ReturnErrorsFromClient()
    {
        var error = ErrorResult.Empty("sessionToken");
        _httpClient.SetError = error;

        var result = await _client.EventTypes("token");

        result.ShouldBeFailure(error);
    }

    [Theory]
    [InlineData("token")]
    [InlineData("newSessionToken")]
    public async Task SessionTokenIsPassedToClient(string token)
    {
        await _client.EventTypes(token);

        _httpClient.LastSessionTokenUsed.Should().Be(token);
    }

    [Fact]
    public async Task CallsTheCorrectUri()
    {
        var expectedUri = new Uri("https://api.betfair.com/exchange/betting/rest/v1.0/listEventTypes/");

        await _client.EventTypes("token");

        _httpClient.LastUriCalled.Should().Be(expectedUri);
    }

    [Fact]
    public async Task EmptyMarketFilterIsSentToClientAsDefault()
    {
        var filter = new RequestBody();
        var expected = JsonSerializer.ToJsonString(filter, StandardResolver.AllowPrivateExcludeNullCamelCase);

        await _client.EventTypes("token");

        var actual =
            JsonSerializer.ToJsonString(_httpClient.LastBodySent, StandardResolver.AllowPrivateExcludeNullCamelCase);
        actual.Should().Be(expected);
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
