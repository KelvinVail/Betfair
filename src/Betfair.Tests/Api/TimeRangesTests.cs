using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class TimeRangesTests : IDisposable
{
    private readonly HttpAdapterStub _client = new();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public TimeRangesTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<TimeRangeResult>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.TimeRanges();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listTimeRanges/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _api.TimeRanges();

        _client.LastContentSent.Should().BeEquivalentTo(
            new TimeRangesRequest
            {
                Granularity = "DAYS",
            });
    }

    [Fact]
    public async Task PostsFilterIfProvided()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.TimeRanges(filter);

        _client.LastContentSent.Should().BeEquivalentTo(
            new TimeRangesRequest
            {
                Filter = filter,
                Granularity = "DAYS",
            });
    }

    [Theory]
    [InlineData(TimeGranularity.Days)]
    [InlineData(TimeGranularity.Hours)]
    [InlineData(TimeGranularity.Minutes)]
    public async Task PostsProvidedGranularity(TimeGranularity granularity)
    {
        await _api.TimeRanges(granularity: granularity);

        _client.LastContentSent.Should().BeEquivalentTo(
            new TimeRangesRequest
            {
                Granularity = granularity.ToString().ToUpperInvariant(),
            });
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.TimeRanges(filter, TimeGranularity.Hours);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.TimeRangesRequest);

        json.Should().Be("{\"filter\":{\"marketIds\":[\"1.23456789\"]},\"granularity\":\"HOURS\"}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new TimeRangeResult
            {
                TimeRange = new TimeRange
                {
                    From = DateTimeOffset.UtcNow,
                    To = DateTimeOffset.UtcNow.AddDays(1),
                },
                MarketCount = 2,
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.TimeRanges();

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