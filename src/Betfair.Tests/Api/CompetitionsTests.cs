using Betfair.Api;
using Betfair.Api.Betting;
using Betfair.Api.Betting.Endpoints.ListCompetitions.Requests;
using Betfair.Api.Betting.Endpoints.ListCompetitions.Responses;
using Betfair.Core;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class CompetitionsTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public CompetitionsTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = Array.Empty<CompetitionResult>();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.Competitions();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listCompetitions/");
    }

    [Fact]
    public async Task PostsDefaultBodyIfFilterIsNull()
    {
        await _api.Competitions();

        _client.LastContentSent.Should().BeEquivalentTo(
            new CompetitionsRequest());
    }

    [Fact]
    public async Task PostsFilterIfProvided()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.Competitions(filter);

        _client.LastContentSent.Should().BeEquivalentTo(
            new CompetitionsRequest { Filter = filter });
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");

        await _api.Competitions(filter);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.CompetitionsRequest);

        json.Should().Be("{\"filter\":{\"marketIds\":[\"1.23456789\"]}}");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        _client.RespondsWithBody = new[]
        {
            new CompetitionResult
            {
                MarketCount = 2,
                CompetitionRegion = "Europe",
                Competition = new Competition
                {
                    Id = "1",
                    Name = "Test Competition",
                },
            },
        };

        var response = await _api.Competitions();

        response.Should().HaveCount(1);
        response[0].MarketCount.Should().Be(2);
        response[0].CompetitionRegion.Should().Be("Europe");
        response[0].Competition!.Id.Should().Be("1");
        response[0].Competition!.Name.Should().Be("Test Competition");
    }

    [Fact]
    public async Task ShouldPassCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        await _api.Competitions(cancellationToken: token);

        _client.LastUriCalled.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldHandleCancellationTokenCancellation()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var act = async () => await _api.Competitions(cancellationToken: cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task RequestWithComplexFilterShouldBeSerializable()
    {
        var filter = new ApiMarketFilter()
            .WithEventTypes(EventType.Of(1), EventType.Of(2))
            .WithCompetitionIds("123", "456")
            .WithCountries("GB", "US")
            .WithMarketTypes("MATCH_ODDS", "OVER_UNDER_25");

        await _api.Competitions(filter);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain("\"eventTypeIds\":[\"1\",\"2\"]");
        json.Should().Contain("\"competitionIds\":[\"123\",\"456\"]");
        json.Should().Contain("\"marketCountries\":[\"GB\",\"US\"]");
        json.Should().Contain("\"marketTypeCodes\":[\"MATCH_ODDS\",\"OVER_UNDER_25\"]");
    }

    [Fact]
    public async Task RequestWithDateRangeFilterShouldBeSerializable()
    {
        var from = new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2023, 6, 30, 23, 59, 59, TimeSpan.Zero);
        var filter = new ApiMarketFilter().FromMarketStart(from).ToMarketStart(to);

        await _api.Competitions(filter);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain("\"marketStartTime\":");
        json.Should().Contain("\"from\":\"2023-06-01T00:00:00Z\"");
        json.Should().Contain("\"to\":\"2023-06-30T23:59:59Z\"");
    }

    [Fact]
    public async Task RequestWithEmptyFilterShouldBeSerializable()
    {
        var filter = new ApiMarketFilter();

        await _api.Competitions(filter);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.CompetitionsRequest);

        json.Should().Be("{\"filter\":{}}");
    }

    [Fact]
    public async Task ResponseWithEmptyArrayShouldBeDeserializable()
    {
        _client.RespondsWithBody = Array.Empty<CompetitionResult>();

        var response = await _api.Competitions();

        response.Should().NotBeNull();
        response.Should().BeEmpty();
    }

    [Fact]
    public async Task ResponseWithMultipleCompetitionsShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new CompetitionResult
            {
                MarketCount = 150,
                CompetitionRegion = "Europe",
                Competition = new Competition
                {
                    Id = "10932509",
                    Name = "English Premier League",
                },
            },
            new CompetitionResult
            {
                MarketCount = 75,
                CompetitionRegion = "Europe",
                Competition = new Competition
                {
                    Id = "117",
                    Name = "UEFA Champions League",
                },
            },
            new CompetitionResult
            {
                MarketCount = 0,
                CompetitionRegion = "International",
                Competition = new Competition
                {
                    Id = "456",
                    Name = "FIFA World Cup",
                },
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.Competitions();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task ResponseWithNullCompetitionShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new CompetitionResult
            {
                MarketCount = 10,
                CompetitionRegion = "Unknown",
                Competition = null,
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.Competitions();

        response.Should().BeEquivalentTo(expectedResponse);
        response[0].Competition.Should().BeNull();
    }

    [Fact]
    public async Task ResponseWithNullCompetitionRegionShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new CompetitionResult
            {
                MarketCount = 5,
                CompetitionRegion = null,
                Competition = new Competition
                {
                    Id = "789",
                    Name = "Test Competition",
                },
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.Competitions();

        response.Should().BeEquivalentTo(expectedResponse);
        response[0].CompetitionRegion.Should().BeNull();
    }

    [Fact]
    public async Task ResponseWithZeroMarketCountShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new CompetitionResult
            {
                MarketCount = 0,
                CompetitionRegion = "Europe",
                Competition = new Competition
                {
                    Id = "999",
                    Name = "Inactive Competition",
                },
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.Competitions();

        response.Should().BeEquivalentTo(expectedResponse);
        response[0].MarketCount.Should().Be(0);
    }

    [Fact]
    public async Task ResponseWithLargeMarketCountShouldBeDeserializable()
    {
        var expectedResponse = new[]
        {
            new CompetitionResult
            {
                MarketCount = 999999,
                CompetitionRegion = "Global",
                Competition = new Competition
                {
                    Id = "1000",
                    Name = "Major Competition",
                },
            },
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.Competitions();

        response.Should().BeEquivalentTo(expectedResponse);
        response[0].MarketCount.Should().Be(999999);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("en-GB")]
    [InlineData("de-DE")]
    [InlineData("fr-FR")]
    [InlineData("es-ES")]
    [InlineData("it-IT")]
    [InlineData("pt-BR")]
    public void RequestWithLocaleShouldBeSerializable(string locale)
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter(),
            Locale = locale,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain($"\"locale\":\"{locale}\"");
        json.Should().Contain("\"filter\":{}");
    }

    [Fact]
    public void RequestWithNullLocaleShouldBeSerializable()
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter(),
            Locale = null,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().NotContain("\"locale\":");
        json.Should().Contain("\"filter\":{}");
    }

    [Fact]
    public void RequestWithEmptyLocaleShouldBeSerializable()
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter(),
            Locale = string.Empty,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain("\"locale\":\"\"");
        json.Should().Contain("\"filter\":{}");
    }

    [Fact]
    public void RequestWithWhitespaceLocaleShouldBeSerializable()
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter(),
            Locale = "   ",
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain("\"locale\":\"   \"");
        json.Should().Contain("\"filter\":{}");
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _client.Dispose();
                _api?.Dispose();
            }

            _disposedValue = true;
        }
    }
}
