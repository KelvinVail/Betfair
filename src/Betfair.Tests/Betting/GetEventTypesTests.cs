using Betfair.Betting;
using Betfair.Errors;
using Betfair.Tests.Helpers;
using Betfair.Tests.TestDoubles;
using FluentAssertions;

namespace Betfair.Tests.Betting;

public class GetEventTypesTests : IDisposable
{
    private readonly BetfairHttpClientFake _httpClient = new ();
    private readonly BettingClient _client;
    private bool _disposedValue;

    public GetEventTypesTests() =>
        _client = new BettingClient(_httpClient);

    [Fact]
    public async Task SessionTokenMustNotBeNull()
    {
        var result = await _client.EventTypes(null, default);

        result.ShouldBe(ErrorResult.Empty("sessionToken"));
    }

    [Fact]
    public async Task SessionTokenMustNotBeEmpty()
    {
        var result = await _client.EventTypes(string.Empty, default);

        result.ShouldBe(ErrorResult.Empty("sessionToken"));
    }

    [Fact]
    public async Task SessionTokenMustNotBeWhiteSpace()
    {
        var result = await _client.EventTypes(" ", default);

        result.ShouldBe(ErrorResult.Empty("sessionToken"));
    }

    [Fact]
    public async Task CallsTheCorrectUri()
    {
        var expectedUri = new Uri("https://api.betfair.com/exchange/betting/rest/v1.0/listEventTypes/");

        await _client.EventTypes("token", default);

        _httpClient.LastUriCalled.Should().Be(expectedUri);
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
