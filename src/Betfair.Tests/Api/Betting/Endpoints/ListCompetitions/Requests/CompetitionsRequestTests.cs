using Betfair.Api.Betting;
using Betfair.Api.Betting.Endpoints.ListCompetitions.Requests;
using Betfair.Core;

namespace Betfair.Tests.Api.Betting.Endpoints.ListCompetitions.Requests;

public class CompetitionsRequestTests
{
    [Fact]
    public void ConstructorSetsDefaultValues()
    {
        var request = new CompetitionsRequest();

        request.Filter.Should().NotBeNull();
        request.Locale.Should().BeNull();
    }

    [Fact]
    public void CanSetFilter()
    {
        var filter = new ApiMarketFilter().WithMarketIds("1.23456789");
        var request = new CompetitionsRequest { Filter = filter };

        request.Filter.Should().BeSameAs(filter);
    }

    [Fact]
    public void CanSetLocale()
    {
        var request = new CompetitionsRequest { Locale = "en-GB" };

        request.Locale.Should().Be("en-GB");
    }

    [Fact]
    public void CanSetLocaleToNull()
    {
        var request = new CompetitionsRequest { Locale = null };

        request.Locale.Should().BeNull();
    }

    [Fact]
    public void CanSetLocaleToEmpty()
    {
        var request = new CompetitionsRequest { Locale = string.Empty };

        request.Locale.Should().Be(string.Empty);
    }

    [Fact]
    public void CanSerializeWithMinimalProperties()
    {
        var request = new CompetitionsRequest();

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Be("{\"filter\":{}}");
    }

    [Fact]
    public void CanSerializeWithFilter()
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter().WithMarketIds("1.23456789"),
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Be("{\"filter\":{\"marketIds\":[\"1.23456789\"]}}");
    }

    [Fact]
    public void CanSerializeWithLocale()
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter(),
            Locale = "en-GB",
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Be("{\"filter\":{},\"locale\":\"en-GB\"}");
    }

    [Fact]
    public void CanSerializeWithComplexFilter()
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter()
                .WithEventTypes(EventType.Of(1))
                .WithCompetitionIds("123")
                .WithCountries("GB"),
            Locale = "en-US",
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain("\"eventTypeIds\":[\"1\"]");
        json.Should().Contain("\"competitionIds\":[\"123\"]");
        json.Should().Contain("\"marketCountries\":[\"GB\"]");
        json.Should().Contain("\"locale\":\"en-US\"");
    }

    [Fact]
    public void CanSerializeWithDateRangeFilter()
    {
        var from = new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2023, 6, 30, 23, 59, 59, TimeSpan.Zero);
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter().FromMarketStart(from).ToMarketStart(to),
            Locale = "de-DE",
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain("\"marketStartTime\":");
        json.Should().Contain("\"from\":\"2023-06-01T00:00:00Z\"");
        json.Should().Contain("\"to\":\"2023-06-30T23:59:59Z\"");
        json.Should().Contain("\"locale\":\"de-DE\"");
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("en-GB")]
    [InlineData("de-DE")]
    [InlineData("fr-FR")]
    [InlineData("es-ES")]
    [InlineData("it-IT")]
    [InlineData("pt-BR")]
    [InlineData("ja-JP")]
    [InlineData("zh-CN")]
    public void CanSerializeWithDifferentLocales(string locale)
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter(),
            Locale = locale,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain($"\"locale\":\"{locale}\"");
    }

    [Fact]
    public void SerializationWithNullLocaleOmitsLocaleProperty()
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter().WithMarketIds("1.123"),
            Locale = null,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().NotContain("\"locale\":");
        json.Should().Contain("\"filter\":");
    }

    [Fact]
    public void SerializationWithEmptyLocaleIncludesLocaleProperty()
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter(),
            Locale = string.Empty,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain("\"locale\":\"\"");
    }

    [Fact]
    public void SerializationWithWhitespaceLocaleIncludesLocaleProperty()
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter(),
            Locale = "   ",
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain("\"locale\":\"   \"");
    }

    [Fact]
    public void CanSerializeWithAllFilterProperties()
    {
        var from = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2023, 12, 31, 23, 59, 59, TimeSpan.Zero);
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter()
                .WithEventTypes(EventType.Of(1), EventType.Of(2))
                .WithEventIds("123", "456")
                .WithCompetitionIds("comp1", "comp2")
                .WithMarketIds("1.123", "1.456")
                .WithCountries("GB", "US")
                .WithMarketTypes("MATCH_ODDS", "OVER_UNDER_25")
                .WithVenues("venue1", "venue2")
                .FromMarketStart(from).ToMarketStart(to)
                .WithBspOnly(true)
                .WithTurnInPlayEnabled(true)
                .WithInPlayOnly(true)
                .WithMarketBettingTypes("O", "L"),
            Locale = "en-GB",
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Contain("\"eventTypeIds\":[\"1\",\"2\"]");
        json.Should().Contain("\"eventIds\":[\"123\",\"456\"]");
        json.Should().Contain("\"competitionIds\":[\"comp1\",\"comp2\"]");
        json.Should().Contain("\"marketIds\":[\"1.123\",\"1.456\"]");
        json.Should().Contain("\"marketCountries\":[\"GB\",\"US\"]");
        json.Should().Contain("\"marketTypeCodes\":[\"MATCH_ODDS\",\"OVER_UNDER_25\"]");
        json.Should().Contain("\"venues\":[\"venue1\",\"venue2\"]");
        json.Should().Contain("\"marketStartTime\":");
        json.Should().Contain("\"bspOnly\":true");
        json.Should().Contain("\"turnInPlayEnabled\":true");
        json.Should().Contain("\"inPlayOnly\":true");
        json.Should().Contain("\"marketBettingTypes\":[\"O\",\"L\"]");
        json.Should().Contain("\"locale\":\"en-GB\"");
    }

    [Fact]
    public void CanSerializeWithEmptyFilter()
    {
        var request = new CompetitionsRequest
        {
            Filter = new ApiMarketFilter(),
            Locale = "fr-FR",
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.CompetitionsRequest);

        json.Should().Be("{\"filter\":{},\"locale\":\"fr-FR\"}");
    }

    [Fact]
    public void FilterPropertyCannotBeNull()
    {
        var request = new CompetitionsRequest();

        request.Filter.Should().NotBeNull();
    }

    [Fact]
    public void CanReplaceFilter()
    {
        var request = new CompetitionsRequest();
        var originalFilter = request.Filter;
        var newFilter = new ApiMarketFilter().WithMarketIds("1.123");

        request.Filter = newFilter;

        request.Filter.Should().BeSameAs(newFilter);
        request.Filter.Should().NotBeSameAs(originalFilter);
    }
}
