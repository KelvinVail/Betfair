using Betfair.Api.Betting.Endpoints.ListCompetitions.Responses;

namespace Betfair.Tests.Api.Betting.Endpoints.ListCompetitions.Responses;

public class CompetitionResultTests
{
    [Fact]
    public void CanCreateWithDefaultValues()
    {
        var result = new CompetitionResult();

        result.Competition.Should().BeNull();
        result.MarketCount.Should().Be(0);
        result.CompetitionRegion.Should().BeNull();
    }

    [Fact]
    public void CanSetCompetition()
    {
        var competition = new Competition { Id = "123", Name = "Test Competition" };
        var result = new CompetitionResult { Competition = competition };

        result.Competition.Should().BeSameAs(competition);
    }

    [Fact]
    public void CanSetMarketCount()
    {
        var result = new CompetitionResult { MarketCount = 150 };

        result.MarketCount.Should().Be(150);
    }

    [Fact]
    public void CanSetCompetitionRegion()
    {
        var result = new CompetitionResult { CompetitionRegion = "Europe" };

        result.CompetitionRegion.Should().Be("Europe");
    }

    [Fact]
    public void CanSetAllProperties()
    {
        var competition = new Competition { Id = "456", Name = "Premier League" };
        var result = new CompetitionResult
        {
            Competition = competition,
            MarketCount = 200,
            CompetitionRegion = "Europe",
        };

        result.Competition.Should().BeSameAs(competition);
        result.MarketCount.Should().Be(200);
        result.CompetitionRegion.Should().Be("Europe");
    }

    [Fact]
    public void CanSetCompetitionToNull()
    {
        var result = new CompetitionResult { Competition = null };

        result.Competition.Should().BeNull();
    }

    [Fact]
    public void CanSetCompetitionRegionToNull()
    {
        var result = new CompetitionResult { CompetitionRegion = null };

        result.CompetitionRegion.Should().BeNull();
    }

    [Fact]
    public void CanSetMarketCountToZero()
    {
        var result = new CompetitionResult { MarketCount = 0 };

        result.MarketCount.Should().Be(0);
    }

    [Fact]
    public void CanSetMarketCountToLargeValue()
    {
        var result = new CompetitionResult { MarketCount = 999999 };

        result.MarketCount.Should().Be(999999);
    }

    [Fact]
    public void CanDeserializeFromJson()
    {
        const string json = """
            {
                "competition": {
                    "id": "10932509",
                    "name": "English Premier League"
                },
                "marketCount": 150,
                "competitionRegion": "Europe"
            }
            """;

        var result = JsonSerializer.Deserialize<CompetitionResult>(json, SerializerContext.Default.CompetitionResult);

        result.Should().NotBeNull();
        result!.Competition.Should().NotBeNull();
        result.Competition!.Id.Should().Be("10932509");
        result.Competition.Name.Should().Be("English Premier League");
        result.MarketCount.Should().Be(150);
        result.CompetitionRegion.Should().Be("Europe");
    }

    [Fact]
    public void CanDeserializeFromJsonWithNullCompetition()
    {
        const string json = """
            {
                "competition": null,
                "marketCount": 10,
                "competitionRegion": "Unknown"
            }
            """;

        var result = JsonSerializer.Deserialize<CompetitionResult>(json, SerializerContext.Default.CompetitionResult);

        result.Should().NotBeNull();
        result!.Competition.Should().BeNull();
        result.MarketCount.Should().Be(10);
        result.CompetitionRegion.Should().Be("Unknown");
    }

    [Fact]
    public void CanDeserializeFromJsonWithNullCompetitionRegion()
    {
        const string json = """
            {
                "competition": {
                    "id": "123",
                    "name": "Test Competition"
                },
                "marketCount": 5,
                "competitionRegion": null
            }
            """;

        var result = JsonSerializer.Deserialize<CompetitionResult>(json, SerializerContext.Default.CompetitionResult);

        result.Should().NotBeNull();
        result!.Competition.Should().NotBeNull();
        result.Competition!.Id.Should().Be("123");
        result.Competition.Name.Should().Be("Test Competition");
        result.MarketCount.Should().Be(5);
        result.CompetitionRegion.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeFromJsonWithZeroMarketCount()
    {
        const string json = """
            {
                "competition": {
                    "id": "999",
                    "name": "Inactive Competition"
                },
                "marketCount": 0,
                "competitionRegion": "Europe"
            }
            """;

        var result = JsonSerializer.Deserialize<CompetitionResult>(json, SerializerContext.Default.CompetitionResult);

        result.Should().NotBeNull();
        result!.Competition.Should().NotBeNull();
        result.Competition!.Id.Should().Be("999");
        result.Competition.Name.Should().Be("Inactive Competition");
        result.MarketCount.Should().Be(0);
        result.CompetitionRegion.Should().Be("Europe");
    }

    [Fact]
    public void CanDeserializeFromJsonWithMissingOptionalProperties()
    {
        const string json = """
            {
                "marketCount": 25
            }
            """;

        var result = JsonSerializer.Deserialize<CompetitionResult>(json, SerializerContext.Default.CompetitionResult);

        result.Should().NotBeNull();
        result!.Competition.Should().BeNull();
        result.MarketCount.Should().Be(25);
        result.CompetitionRegion.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeArrayFromJson()
    {
        const string json = """
            [
                {
                    "competition": {
                        "id": "10932509",
                        "name": "English Premier League"
                    },
                    "marketCount": 150,
                    "competitionRegion": "Europe"
                },
                {
                    "competition": {
                        "id": "117",
                        "name": "UEFA Champions League"
                    },
                    "marketCount": 75,
                    "competitionRegion": "Europe"
                }
            ]
            """;

        var results = JsonSerializer.Deserialize<CompetitionResult[]>(json, SerializerContext.Default.CompetitionResultArray);

        results.Should().NotBeNull();
        results!.Should().HaveCount(2);

        results[0].Competition!.Id.Should().Be("10932509");
        results[0].Competition!.Name.Should().Be("English Premier League");
        results[0].MarketCount.Should().Be(150);
        results[0].CompetitionRegion.Should().Be("Europe");

        results[1].Competition!.Id.Should().Be("117");
        results[1].Competition!.Name.Should().Be("UEFA Champions League");
        results[1].MarketCount.Should().Be(75);
        results[1].CompetitionRegion.Should().Be("Europe");
    }

    [Fact]
    public void CanDeserializeEmptyArrayFromJson()
    {
        const string json = "[]";

        var results = JsonSerializer.Deserialize<CompetitionResult[]>(json, SerializerContext.Default.CompetitionResultArray);

        results.Should().NotBeNull();
        results!.Should().BeEmpty();
    }

    [Fact]
    public void CanSerializeToJson()
    {
        var result = new CompetitionResult
        {
            Competition = new Competition
            {
                Id = "10932509",
                Name = "English Premier League",
            },
            MarketCount = 150,
            CompetitionRegion = "Europe",
        };

        var json = JsonSerializer.Serialize(result, SerializerContext.Default.CompetitionResult);

        json.Should().Contain("\"competition\":");
        json.Should().Contain("\"id\":\"10932509\"");
        json.Should().Contain("\"name\":\"English Premier League\"");
        json.Should().Contain("\"marketCount\":150");
        json.Should().Contain("\"competitionRegion\":\"Europe\"");
    }

    [Fact]
    public void CanSerializeWithNullCompetition()
    {
        var result = new CompetitionResult
        {
            Competition = null,
            MarketCount = 10,
            CompetitionRegion = "Unknown",
        };

        var json = JsonSerializer.Serialize(result, SerializerContext.Default.CompetitionResult);

        json.Should().NotContain("\"competition\":");
        json.Should().Contain("\"marketCount\":10");
        json.Should().Contain("\"competitionRegion\":\"Unknown\"");
    }

    [Fact]
    public void CanSerializeWithNullCompetitionRegion()
    {
        var result = new CompetitionResult
        {
            Competition = new Competition { Id = "123", Name = "Test" },
            MarketCount = 5,
            CompetitionRegion = null,
        };

        var json = JsonSerializer.Serialize(result, SerializerContext.Default.CompetitionResult);

        json.Should().Contain("\"competition\":");
        json.Should().Contain("\"marketCount\":5");
        json.Should().NotContain("\"competitionRegion\":");
    }
}
