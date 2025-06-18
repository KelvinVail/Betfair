using Betfair.Api.Betting.Endpoints.ListCompetitions.Responses;

namespace Betfair.Tests.Api.Betting.Endpoints.ListCompetitions.Responses;

public class CompetitionTests
{
    [Fact]
    public void CanCreateWithDefaultValues()
    {
        var competition = new Competition();

        competition.Id.Should().Be(string.Empty);
        competition.Name.Should().Be(string.Empty);
    }

    [Fact]
    public void CanSetId()
    {
        var competition = new Competition { Id = "10932509" };

        competition.Id.Should().Be("10932509");
    }

    [Fact]
    public void CanSetName()
    {
        var competition = new Competition { Name = "English Premier League" };

        competition.Name.Should().Be("English Premier League");
    }

    [Fact]
    public void CanSetBothProperties()
    {
        var competition = new Competition
        {
            Id = "117",
            Name = "UEFA Champions League",
        };

        competition.Id.Should().Be("117");
        competition.Name.Should().Be("UEFA Champions League");
    }

    [Fact]
    public void CanSetIdToEmptyString()
    {
        var competition = new Competition { Id = string.Empty };

        competition.Id.Should().Be(string.Empty);
    }

    [Fact]
    public void CanSetNameToEmptyString()
    {
        var competition = new Competition { Name = string.Empty };

        competition.Name.Should().Be(string.Empty);
    }

    [Fact]
    public void CanSetIdToWhitespace()
    {
        var competition = new Competition { Id = "   " };

        competition.Id.Should().Be("   ");
    }

    [Fact]
    public void CanSetNameToWhitespace()
    {
        var competition = new Competition { Name = "   " };

        competition.Name.Should().Be("   ");
    }

    [Fact]
    public void CanSetLongId()
    {
        const string longId = "123456789012345678901234567890";
        var competition = new Competition { Id = longId };

        competition.Id.Should().Be(longId);
    }

    [Fact]
    public void CanSetLongName()
    {
        const string longName = "This is a very long competition name that might be used in some scenarios to test the limits of the system";
        var competition = new Competition { Name = longName };

        competition.Name.Should().Be(longName);
    }

    [Fact]
    public void CanSetSpecialCharactersInId()
    {
        const string specialId = "comp-123_456.789";
        var competition = new Competition { Id = specialId };

        competition.Id.Should().Be(specialId);
    }

    [Fact]
    public void CanSetSpecialCharactersInName()
    {
        const string specialName = "Competition with Special Characters: & < > \" ' / \\";
        var competition = new Competition { Name = specialName };

        competition.Name.Should().Be(specialName);
    }

    [Fact]
    public void CanSetUnicodeCharactersInName()
    {
        const string unicodeName = "Bundesliga 🇩🇪 Fußball-Liga";
        var competition = new Competition { Name = unicodeName };

        competition.Name.Should().Be(unicodeName);
    }

    [Fact]
    public void CanDeserializeFromJson()
    {
        const string json = """
            {
                "id": "10932509",
                "name": "English Premier League"
            }
            """;

        var competition = JsonSerializer.Deserialize<Competition>(json, SerializerContext.Default.Competition);

        competition.Should().NotBeNull();
        competition!.Id.Should().Be("10932509");
        competition.Name.Should().Be("English Premier League");
    }

    [Fact]
    public void CanDeserializeFromJsonWithEmptyValues()
    {
        const string json = """
            {
                "id": "",
                "name": ""
            }
            """;

        var competition = JsonSerializer.Deserialize<Competition>(json, SerializerContext.Default.Competition);

        competition.Should().NotBeNull();
        competition!.Id.Should().Be(string.Empty);
        competition.Name.Should().Be(string.Empty);
    }

    [Fact]
    public void CanDeserializeFromJsonWithMissingProperties()
    {
        const string json = "{}";

        var competition = JsonSerializer.Deserialize<Competition>(json, SerializerContext.Default.Competition);

        competition.Should().NotBeNull();

        // When properties are missing from JSON, they may be null instead of empty string
        competition!.Id.Should().BeNullOrEmpty();
        competition.Name.Should().BeNullOrEmpty();
    }

    [Fact]
    public void CanDeserializeFromJsonWithSpecialCharacters()
    {
        const string json = """
            {
                "id": "comp-123_456.789",
                "name": "Competition with Special Characters: & < > \" ' / \\"
            }
            """;

        var competition = JsonSerializer.Deserialize<Competition>(json, SerializerContext.Default.Competition);

        competition.Should().NotBeNull();
        competition!.Id.Should().Be("comp-123_456.789");
        competition.Name.Should().Be("Competition with Special Characters: & < > \" ' / \\");
    }

    [Fact]
    public void CanDeserializeFromJsonWithUnicodeCharacters()
    {
        const string json = """
            {
                "id": "bundesliga",
                "name": "Bundesliga 🇩🇪 Fußball-Liga"
            }
            """;

        var competition = JsonSerializer.Deserialize<Competition>(json, SerializerContext.Default.Competition);

        competition.Should().NotBeNull();
        competition!.Id.Should().Be("bundesliga");
        competition.Name.Should().Be("Bundesliga 🇩🇪 Fußball-Liga");
    }

    [Fact]
    public void CanSerializeToJson()
    {
        var competition = new Competition
        {
            Id = "10932509",
            Name = "English Premier League",
        };

        var json = JsonSerializer.Serialize(competition, SerializerContext.Default.Competition);

        json.Should().Contain("\"id\":\"10932509\"");
        json.Should().Contain("\"name\":\"English Premier League\"");
    }

    [Fact]
    public void CanSerializeWithEmptyValues()
    {
        var competition = new Competition
        {
            Id = string.Empty,
            Name = string.Empty,
        };

        var json = JsonSerializer.Serialize(competition, SerializerContext.Default.Competition);

        json.Should().Contain("\"id\":\"\"");
        json.Should().Contain("\"name\":\"\"");
    }

    [Fact]
    public void CanSerializeWithSpecialCharacters()
    {
        var competition = new Competition
        {
            Id = "comp-123_456.789",
            Name = "Competition with Special Characters: & < > \" ' / \\",
        };

        var json = JsonSerializer.Serialize(competition, SerializerContext.Default.Competition);

        json.Should().Contain("\"id\":\"comp-123_456.789\"");
        json.Should().Contain("Competition with Special Characters");
    }

    [Fact]
    public void CanSerializeWithUnicodeCharacters()
    {
        var competition = new Competition
        {
            Id = "bundesliga",
            Name = "Bundesliga 🇩🇪 Fußball-Liga",
        };

        var json = JsonSerializer.Serialize(competition, SerializerContext.Default.Competition);

        json.Should().Contain("\"id\":\"bundesliga\"");
        json.Should().Contain("Bundesliga");

        // Unicode characters may be escaped in JSON
        json.Should().Contain("Fu").And.Contain("ball-Liga");
    }

    [Fact]
    public void SerializationRoundTripPreservesData()
    {
        var original = new Competition
        {
            Id = "test-123",
            Name = "Test Competition with Unicode: 🏆",
        };

        var json = JsonSerializer.Serialize(original, SerializerContext.Default.Competition);
        var deserialized = JsonSerializer.Deserialize<Competition>(json, SerializerContext.Default.Competition);

        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.Name.Should().Be(original.Name);
    }

    [Fact]
    public void CanCreateMultipleInstancesWithDifferentValues()
    {
        var competition1 = new Competition { Id = "1", Name = "First" };
        var competition2 = new Competition { Id = "2", Name = "Second" };

        competition1.Id.Should().Be("1");
        competition1.Name.Should().Be("First");
        competition2.Id.Should().Be("2");
        competition2.Name.Should().Be("Second");
    }

    [Fact]
    public void PropertiesAreIndependent()
    {
        var competition = new Competition { Id = "123" };

        competition.Id.Should().Be("123");
        competition.Name.Should().Be(string.Empty);

        var competition2 = new Competition { Id = "123", Name = "Test Name" };

        competition2.Id.Should().Be("123");
        competition2.Name.Should().Be("Test Name");
    }
}
