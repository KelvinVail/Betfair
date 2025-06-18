using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Responses;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountStatement.Responses;

public class StatementLegacyDataTests
{
    [Fact]
    public void CanDeserializeStatementLegacyDataWithAllProperties()
    {
        var json = """
        {
            "avgPrice": 3.75,
            "betSize": 20.0,
            "betType": "L",
            "eventId": 30123456,
            "eventTypeId": 2,
            "fullMarketName": "Premier League/Over/Under 2.5 Goals",
            "grossBetAmount": 20.0,
            "marketName": "Over/Under 2.5 Goals",
            "marketType": "O/U",
            "placedDate": "2023-06-15T15:30:00.000Z",
            "selectionId": 47973,
            "selectionName": "Under 2.5",
            "startDate": "2023-06-15T16:00:00.000Z",
            "transactionType": "RESULT_WON",
            "transactionId": 987654322,
            "winLose": "WON"
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementLegacyData);

        result.Should().NotBeNull();
        result!.AvgPrice.Should().Be(3.75);
        result.BetSize.Should().Be(20.0);
        result.BetType.Should().Be("L");
        result.EventId.Should().Be(30123456);
        result.EventTypeId.Should().Be(2);
        result.FullMarketName.Should().Be("Premier League/Over/Under 2.5 Goals");
        result.GrossBetAmount.Should().Be(20.0);
        result.MarketName.Should().Be("Over/Under 2.5 Goals");
        result.MarketType.Should().Be("O/U");
        result.PlacedDate.Should().Be(new DateTime(2023, 6, 15, 15, 30, 0, DateTimeKind.Utc));
        result.SelectionId.Should().Be(47973);
        result.SelectionName.Should().Be("Under 2.5");
        result.StartDate.Should().Be(new DateTime(2023, 6, 15, 16, 0, 0, DateTimeKind.Utc));
        result.TransactionType.Should().Be("RESULT_WON");
        result.TransactionId.Should().Be(987654322);
        result.WinLose.Should().Be("WON");
    }

    [Fact]
    public void CanDeserializeStatementLegacyDataWithNullProperties()
    {
        var json = """
        {
            "avgPrice": null,
            "betSize": null,
            "betType": null,
            "eventId": null,
            "eventTypeId": null,
            "fullMarketName": null,
            "grossBetAmount": null,
            "marketName": null,
            "marketType": null,
            "placedDate": null,
            "selectionId": null,
            "selectionName": null,
            "startDate": null,
            "transactionType": null,
            "transactionId": null,
            "winLose": null
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementLegacyData);

        result.Should().NotBeNull();
        result!.AvgPrice.Should().BeNull();
        result.BetSize.Should().BeNull();
        result.BetType.Should().BeNull();
        result.EventId.Should().BeNull();
        result.EventTypeId.Should().BeNull();
        result.FullMarketName.Should().BeNull();
        result.GrossBetAmount.Should().BeNull();
        result.MarketName.Should().BeNull();
        result.MarketType.Should().BeNull();
        result.PlacedDate.Should().BeNull();
        result.SelectionId.Should().BeNull();
        result.SelectionName.Should().BeNull();
        result.StartDate.Should().BeNull();
        result.TransactionType.Should().BeNull();
        result.TransactionId.Should().BeNull();
        result.WinLose.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeStatementLegacyDataWithPartialProperties()
    {
        var json = """
        {
            "avgPrice": 2.5,
            "betSize": 10.0,
            "betType": "B",
            "eventId": 29123456,
            "marketName": "Match Odds",
            "selectionName": "Team A",
            "transactionType": "RESULT_WON",
            "winLose": "WON"
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementLegacyData);

        result.Should().NotBeNull();
        result!.AvgPrice.Should().Be(2.5);
        result.BetSize.Should().Be(10.0);
        result.BetType.Should().Be("B");
        result.EventId.Should().Be(29123456);
        result.MarketName.Should().Be("Match Odds");
        result.SelectionName.Should().Be("Team A");
        result.TransactionType.Should().Be("RESULT_WON");
        result.WinLose.Should().Be("WON");

        // Properties not in JSON should be null
        result.EventTypeId.Should().BeNull();
        result.FullMarketName.Should().BeNull();
        result.GrossBetAmount.Should().BeNull();
        result.MarketType.Should().BeNull();
        result.PlacedDate.Should().BeNull();
        result.SelectionId.Should().BeNull();
        result.StartDate.Should().BeNull();
        result.TransactionId.Should().BeNull();
    }

    [Fact]
    public void CanSerializeStatementLegacyData()
    {
        var legacyData = new StatementLegacyData
        {
            AvgPrice = 4.5,
            BetSize = 25.0,
            BetType = "L",
            EventId = 12345678,
            EventTypeId = 3,
            FullMarketName = "Test Event/Test Market",
            GrossBetAmount = 25.0,
            MarketName = "Test Market",
            MarketType = "TEST",
            PlacedDate = new DateTime(2023, 6, 15, 10, 0, 0, DateTimeKind.Utc),
            SelectionId = 98765,
            SelectionName = "Test Selection",
            StartDate = new DateTime(2023, 6, 15, 12, 0, 0, DateTimeKind.Utc),
            TransactionType = "TEST_TYPE",
            TransactionId = 555666777,
            WinLose = "TEST_RESULT",
        };

        var json = JsonSerializer.Serialize(legacyData, SerializerContext.Default.StatementLegacyData);

        json.Should().Contain("\"avgPrice\":4.5");
        json.Should().Contain("\"betSize\":25");
        json.Should().Contain("\"betType\":\"L\"");
        json.Should().Contain("\"eventId\":12345678");
        json.Should().Contain("\"eventTypeId\":3");
        json.Should().Contain("\"fullMarketName\":\"Test Event/Test Market\"");
        json.Should().Contain("\"grossBetAmount\":25");
        json.Should().Contain("\"marketName\":\"Test Market\"");
        json.Should().Contain("\"marketType\":\"TEST\"");
        json.Should().Contain("\"selectionId\":98765");
        json.Should().Contain("\"selectionName\":\"Test Selection\"");
        json.Should().Contain("\"transactionType\":\"TEST_TYPE\"");
        json.Should().Contain("\"transactionId\":555666777");
        json.Should().Contain("\"winLose\":\"TEST_RESULT\"");
    }

    [Theory]
    [InlineData("B")]
    [InlineData("L")]
    [InlineData("E")]
    public void CanDeserializeStatementLegacyDataWithDifferentBetTypes(string betType)
    {
        var json = $$"""
        {
            "betType": "{{betType}}",
            "avgPrice": 2.0,
            "betSize": 10.0
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementLegacyData);

        result.Should().NotBeNull();
        result!.BetType.Should().Be(betType);
    }

    [Theory]
    [InlineData("RESULT_WON")]
    [InlineData("RESULT_LOST")]
    [InlineData("COMMISSION")]
    [InlineData("DEPOSIT")]
    [InlineData("WITHDRAWAL")]
    public void CanDeserializeStatementLegacyDataWithDifferentTransactionTypes(string transactionType)
    {
        var json = $$"""
        {
            "transactionType": "{{transactionType}}",
            "avgPrice": 2.0,
            "betSize": 10.0
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementLegacyData);

        result.Should().NotBeNull();
        result!.TransactionType.Should().Be(transactionType);
    }

    [Theory]
    [InlineData("WON")]
    [InlineData("LOST")]
    [InlineData("VOID")]
    public void CanDeserializeStatementLegacyDataWithDifferentWinLoseValues(string winLose)
    {
        var json = $$"""
        {
            "winLose": "{{winLose}}",
            "avgPrice": 2.0,
            "betSize": 10.0
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementLegacyData);

        result.Should().NotBeNull();
        result!.WinLose.Should().Be(winLose);
    }

    [Fact]
    public void SerializationRoundTripWorks()
    {
        var originalLegacyData = new StatementLegacyData
        {
            AvgPrice = 1.85,
            BetSize = 50.0,
            BetType = "B",
            EventId = 87654321,
            EventTypeId = 4,
            FullMarketName = "Round Trip Test/Test Market",
            GrossBetAmount = 50.0,
            MarketName = "Test Market",
            MarketType = "RT",
            PlacedDate = new DateTime(2023, 6, 15, 8, 0, 0, DateTimeKind.Utc),
            SelectionId = 11223,
            SelectionName = "Round Trip Selection",
            StartDate = new DateTime(2023, 6, 15, 10, 0, 0, DateTimeKind.Utc),
            TransactionType = "ROUND_TRIP_TEST",
            TransactionId = 999888777,
            WinLose = "ROUND_TRIP",
        };

        var json = JsonSerializer.Serialize(originalLegacyData, SerializerContext.Default.StatementLegacyData);
        var deserializedLegacyData = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementLegacyData);

        deserializedLegacyData.Should().BeEquivalentTo(originalLegacyData);
    }
}
