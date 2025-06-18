using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Responses;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountStatement.Responses;

public class StatementItemTests
{
    [Fact]
    public void CanDeserializeStatementItemWithAllProperties()
    {
        var json = """
        {
            "refId": "BET123456",
            "itemDate": "2023-06-15T18:45:30.000Z",
            "amount": 25.75,
            "balance": 1250.25,
            "itemClass": "RESULT_WON",
            "itemClassData": {
                "commission": "1.29",
                "exchangeRate": "1.0",
                "winLose": "WON"
            },
            "legacyData": {
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
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementItem);

        result.Should().NotBeNull();
        result!.RefId.Should().Be("BET123456");
        result.ItemDate.Should().Be(new DateTime(2023, 6, 15, 18, 45, 30, DateTimeKind.Utc));
        result.Amount.Should().Be(25.75);
        result.Balance.Should().Be(1250.25);
        result.ItemClass.Should().Be("RESULT_WON");

        result.ItemClassData.Should().NotBeNull();
        result.ItemClassData!.Should().HaveCount(3);
        result.ItemClassData["commission"].Should().Be("1.29");
        result.ItemClassData["exchangeRate"].Should().Be("1.0");
        result.ItemClassData["winLose"].Should().Be("WON");

        result.LegacyData.Should().NotBeNull();
        result.LegacyData!.AvgPrice.Should().Be(3.75);
        result.LegacyData.BetSize.Should().Be(20.0);
        result.LegacyData.BetType.Should().Be("L");
        result.LegacyData.EventId.Should().Be(30123456);
        result.LegacyData.EventTypeId.Should().Be(2);
        result.LegacyData.FullMarketName.Should().Be("Premier League/Over/Under 2.5 Goals");
        result.LegacyData.GrossBetAmount.Should().Be(20.0);
        result.LegacyData.MarketName.Should().Be("Over/Under 2.5 Goals");
        result.LegacyData.MarketType.Should().Be("O/U");
        result.LegacyData.PlacedDate.Should().Be(new DateTime(2023, 6, 15, 15, 30, 0, DateTimeKind.Utc));
        result.LegacyData.SelectionId.Should().Be(47973);
        result.LegacyData.SelectionName.Should().Be("Under 2.5");
        result.LegacyData.StartDate.Should().Be(new DateTime(2023, 6, 15, 16, 0, 0, DateTimeKind.Utc));
        result.LegacyData.TransactionType.Should().Be("RESULT_WON");
        result.LegacyData.TransactionId.Should().Be(987654322);
        result.LegacyData.WinLose.Should().Be("WON");
    }

    [Fact]
    public void CanDeserializeStatementItemWithMinimalProperties()
    {
        var json = """
        {
            "refId": "SIMPLE123",
            "itemDate": "2023-06-15T14:30:00.000Z",
            "amount": -5.25,
            "balance": 95.50,
            "itemClass": "COMMISSION"
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementItem);

        result.Should().NotBeNull();
        result!.RefId.Should().Be("SIMPLE123");
        result.ItemDate.Should().Be(new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Utc));
        result.Amount.Should().Be(-5.25);
        result.Balance.Should().Be(95.50);
        result.ItemClass.Should().Be("COMMISSION");
        result.ItemClassData.Should().BeNull();
        result.LegacyData.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeStatementItemWithNullOptionalProperties()
    {
        var json = """
        {
            "refId": null,
            "itemDate": "2023-06-15T14:30:00.000Z",
            "amount": 10.0,
            "balance": 110.0,
            "itemClass": null,
            "itemClassData": null,
            "legacyData": null
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementItem);

        result.Should().NotBeNull();
        result!.RefId.Should().BeNull();
        result.ItemDate.Should().Be(new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Utc));
        result.Amount.Should().Be(10.0);
        result.Balance.Should().Be(110.0);
        result.ItemClass.Should().BeNull();
        result.ItemClassData.Should().BeNull();
        result.LegacyData.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeStatementItemWithEmptyItemClassData()
    {
        var json = """
        {
            "refId": "EMPTY_DATA",
            "itemDate": "2023-06-15T14:30:00.000Z",
            "amount": 15.0,
            "balance": 115.0,
            "itemClass": "DEPOSIT",
            "itemClassData": {}
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementItem);

        result.Should().NotBeNull();
        result!.RefId.Should().Be("EMPTY_DATA");
        result.ItemClassData.Should().NotBeNull();
        result.ItemClassData.Should().BeEmpty();
    }

    [Fact]
    public void CanSerializeStatementItem()
    {
        var item = new StatementItem
        {
            RefId = "SERIALIZE_TEST",
            ItemDate = new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Utc),
            Amount = 42.50,
            Balance = 142.50,
            ItemClass = "TEST_CLASS",
            ItemClassData = new Dictionary<string, string>
            {
                { "testKey", "testValue" },
            },
        };

        var json = JsonSerializer.Serialize(item, SerializerContext.Default.StatementItem);

        json.Should().Contain("\"refId\":\"SERIALIZE_TEST\"");
        json.Should().Contain("\"amount\":42.5");
        json.Should().Contain("\"balance\":142.5");
        json.Should().Contain("\"itemClass\":\"TEST_CLASS\"");
        json.Should().Contain("\"testKey\":\"testValue\"");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(10.50)]
    [InlineData(-5.25)]
    [InlineData(1000.99)]
    [InlineData(-999.99)]
    public void CanDeserializeStatementItemWithDifferentAmounts(double amount)
    {
        var json = $$"""
        {
            "refId": "AMOUNT_TEST",
            "itemDate": "2023-06-15T14:30:00.000Z",
            "amount": {{amount}},
            "balance": 100.0,
            "itemClass": "TEST"
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementItem);

        result.Should().NotBeNull();
        result!.Amount.Should().Be(amount);
    }

    [Theory]
    [InlineData("COMMISSION")]
    [InlineData("RESULT_WON")]
    [InlineData("RESULT_LOST")]
    [InlineData("DEPOSIT")]
    [InlineData("WITHDRAWAL")]
    [InlineData("UNKNOWN")]
    public void CanDeserializeStatementItemWithDifferentItemClasses(string itemClass)
    {
        var json = $$"""
        {
            "refId": "CLASS_TEST",
            "itemDate": "2023-06-15T14:30:00.000Z",
            "amount": 10.0,
            "balance": 110.0,
            "itemClass": "{{itemClass}}"
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementItem);

        result.Should().NotBeNull();
        result!.ItemClass.Should().Be(itemClass);
    }

    [Fact]
    public void SerializationRoundTripWorks()
    {
        var originalItem = new StatementItem
        {
            RefId = "ROUND_TRIP",
            ItemDate = new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Utc),
            Amount = 33.33,
            Balance = 133.33,
            ItemClass = "ROUND_TRIP_TEST",
            ItemClassData = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" },
            },
        };

        var json = JsonSerializer.Serialize(originalItem, SerializerContext.Default.StatementItem);
        var deserializedItem = JsonSerializer.Deserialize(json, SerializerContext.Default.StatementItem);

        deserializedItem.Should().BeEquivalentTo(originalItem);
    }
}
