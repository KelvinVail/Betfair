using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Responses;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountStatement.Responses;

public class AccountStatementReportTests
{
    [Fact]
    public void CanDeserializeAccountStatementReportWithFullData()
    {
        var json = """
        {
            "accountStatement": [
                {
                    "refId": "12345",
                    "itemDate": "2023-06-15T14:30:00.000Z",
                    "amount": 10.50,
                    "balance": 100.75,
                    "itemClass": "COMMISSION",
                    "itemClassData": {
                        "commission": "0.05",
                        "exchangeRate": "1.0"
                    },
                    "legacyData": {
                        "avgPrice": 2.5,
                        "betSize": 10.0,
                        "betType": "B",
                        "eventId": 29123456,
                        "eventTypeId": 1,
                        "fullMarketName": "Test Match/Match Odds",
                        "grossBetAmount": 10.0,
                        "marketName": "Match Odds",
                        "marketType": "O",
                        "placedDate": "2023-06-15T13:30:00.000Z",
                        "selectionId": 47972,
                        "selectionName": "Team A",
                        "startDate": "2023-06-15T15:00:00.000Z",
                        "transactionType": "RESULT_WON",
                        "transactionId": 987654321,
                        "winLose": "WON"
                    }
                }
            ],
            "moreAvailable": true
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountStatementReport);

        result.Should().NotBeNull();
        result!.AccountStatement.Should().NotBeNull();
        result.AccountStatement.Should().HaveCount(1);
        result.MoreAvailable.Should().BeTrue();

        var item = result.AccountStatement![0];
        item.RefId.Should().Be("12345");
        item.ItemDate.Should().Be(new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Utc));
        item.Amount.Should().Be(10.50);
        item.Balance.Should().Be(100.75);
        item.ItemClass.Should().Be("COMMISSION");
        item.ItemClassData.Should().NotBeNull();
        item.ItemClassData!["commission"].Should().Be("0.05");
        item.ItemClassData["exchangeRate"].Should().Be("1.0");
        item.LegacyData.Should().NotBeNull();
        item.LegacyData!.AvgPrice.Should().Be(2.5);
        item.LegacyData.BetSize.Should().Be(10.0);
        item.LegacyData.BetType.Should().Be("B");
        item.LegacyData.EventId.Should().Be(29123456);
        item.LegacyData.EventTypeId.Should().Be(1);
        item.LegacyData.FullMarketName.Should().Be("Test Match/Match Odds");
        item.LegacyData.GrossBetAmount.Should().Be(10.0);
        item.LegacyData.MarketName.Should().Be("Match Odds");
        item.LegacyData.MarketType.Should().Be("O");
        item.LegacyData.PlacedDate.Should().Be(new DateTime(2023, 6, 15, 13, 30, 0, DateTimeKind.Utc));
        item.LegacyData.SelectionId.Should().Be(47972);
        item.LegacyData.SelectionName.Should().Be("Team A");
        item.LegacyData.StartDate.Should().Be(new DateTime(2023, 6, 15, 15, 0, 0, DateTimeKind.Utc));
        item.LegacyData.TransactionType.Should().Be("RESULT_WON");
        item.LegacyData.TransactionId.Should().Be(987654321);
        item.LegacyData.WinLose.Should().Be("WON");
    }

    [Fact]
    public void CanDeserializeAccountStatementReportWithMinimalData()
    {
        var json = """
        {
            "accountStatement": [
                {
                    "refId": "67890",
                    "itemDate": "2023-06-15T14:30:00.000Z",
                    "amount": -5.25,
                    "balance": 95.50,
                    "itemClass": "COMMISSION"
                }
            ],
            "moreAvailable": false
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountStatementReport);

        result.Should().NotBeNull();
        result!.AccountStatement.Should().NotBeNull();
        result.AccountStatement.Should().HaveCount(1);
        result.MoreAvailable.Should().BeFalse();

        var item = result.AccountStatement![0];
        item.RefId.Should().Be("67890");
        item.ItemDate.Should().Be(new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Utc));
        item.Amount.Should().Be(-5.25);
        item.Balance.Should().Be(95.50);
        item.ItemClass.Should().Be("COMMISSION");
        item.ItemClassData.Should().BeNull();
        item.LegacyData.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeAccountStatementReportWithEmptyAccountStatement()
    {
        var json = """
        {
            "accountStatement": [],
            "moreAvailable": false
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountStatementReport);

        result.Should().NotBeNull();
        result!.AccountStatement.Should().NotBeNull();
        result.AccountStatement.Should().BeEmpty();
        result.MoreAvailable.Should().BeFalse();
    }

    [Fact]
    public void CanDeserializeAccountStatementReportWithNullAccountStatement()
    {
        var json = """
        {
            "accountStatement": null,
            "moreAvailable": false
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountStatementReport);

        result.Should().NotBeNull();
        result!.AccountStatement.Should().BeNull();
        result.MoreAvailable.Should().BeFalse();
    }

    [Fact]
    public void CanDeserializeAccountStatementReportWithMultipleItems()
    {
        var json = """
        {
            "accountStatement": [
                {
                    "refId": "12345",
                    "itemDate": "2023-06-15T14:30:00.000Z",
                    "amount": 10.50,
                    "balance": 100.75,
                    "itemClass": "RESULT_WON"
                },
                {
                    "refId": "67890",
                    "itemDate": "2023-06-15T15:30:00.000Z",
                    "amount": -0.53,
                    "balance": 100.22,
                    "itemClass": "COMMISSION"
                },
                {
                    "refId": "11111",
                    "itemDate": "2023-06-15T16:30:00.000Z",
                    "amount": 50.00,
                    "balance": 150.22,
                    "itemClass": "DEPOSIT"
                }
            ],
            "moreAvailable": true
        }
        """;

        var result = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountStatementReport);

        result.Should().NotBeNull();
        result!.AccountStatement.Should().NotBeNull();
        result.AccountStatement.Should().HaveCount(3);
        result.MoreAvailable.Should().BeTrue();

        result.AccountStatement![0].RefId.Should().Be("12345");
        result.AccountStatement[0].Amount.Should().Be(10.50);
        result.AccountStatement[0].ItemClass.Should().Be("RESULT_WON");

        result.AccountStatement[1].RefId.Should().Be("67890");
        result.AccountStatement[1].Amount.Should().Be(-0.53);
        result.AccountStatement[1].ItemClass.Should().Be("COMMISSION");

        result.AccountStatement[2].RefId.Should().Be("11111");
        result.AccountStatement[2].Amount.Should().Be(50.00);
        result.AccountStatement[2].ItemClass.Should().Be("DEPOSIT");
    }

    [Fact]
    public void CanSerializeAccountStatementReport()
    {
        var report = new AccountStatementReport
        {
            AccountStatement =
            [
                new ()
                {
                    RefId = "TEST123",
                    ItemDate = new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Utc),
                    Amount = 25.75,
                    Balance = 125.75,
                    ItemClass = "RESULT_WON",
                },
            ],
            MoreAvailable = true,
        };

        var json = JsonSerializer.Serialize(report, SerializerContext.Default.AccountStatementReport);

        json.Should().Contain("\"refId\":\"TEST123\"");
        json.Should().Contain("\"amount\":25.75");
        json.Should().Contain("\"balance\":125.75");
        json.Should().Contain("\"itemClass\":\"RESULT_WON\"");
        json.Should().Contain("\"moreAvailable\":true");
    }

    [Fact]
    public void SerializationRoundTripWorks()
    {
        var originalReport = new AccountStatementReport
        {
            AccountStatement =
            [
                new ()
                {
                    RefId = "ROUND_TRIP_TEST",
                    ItemDate = new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Utc),
                    Amount = 15.25,
                    Balance = 115.25,
                    ItemClass = "COMMISSION",
                    ItemClassData = new Dictionary<string, string>
                    {
                        { "commission", "0.76" },
                        { "exchangeRate", "1.0" },
                    },
                },
            ],
            MoreAvailable = false,
        };

        var json = JsonSerializer.Serialize(originalReport, SerializerContext.Default.AccountStatementReport);
        var deserializedReport = JsonSerializer.Deserialize(json, SerializerContext.Default.AccountStatementReport);

        deserializedReport.Should().BeEquivalentTo(originalReport);
    }
}
