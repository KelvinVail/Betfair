using Betfair.Api;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Enums;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Requests;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Responses;
using Betfair.Api.Betting;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountStatement;

public class AccountStatementTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public AccountStatementTests()
    {
        _api = new BetfairApiClient(_client);
        _client.RespondsWithBody = new AccountStatementReport();
    }

    [Fact]
    public async Task CallsTheCorrectEndpoint()
    {
        await _api.AccountStatement().ExecuteAsync();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/getAccountStatement/");
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        await _api.AccountStatement()
            .ExchangeOnly()
            .Take(100)
            .ExecuteAsync();

        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"includeItem\":\"EXCHANGE\"");
        json.Should().Contain("\"recordCount\":100");
    }

    [Fact]
    public async Task ResponseShouldBeDeserializable()
    {
        var expectedResponse = new AccountStatementReport
        {
            AccountStatement = new List<StatementItem>
            {
                new StatementItem
                {
                    RefId = "12345",
                    ItemDate = DateTime.UtcNow,
                    Amount = 10.50,
                    Balance = 100.75,
                    ItemClass = "UNKNOWN",
                    ItemClassData = new Dictionary<string, string>
                    {
                        { "commission", "0.05" },
                        { "exchangeRate", "1.0" },
                    },
                    LegacyData = new StatementLegacyData
                    {
                        AvgPrice = 2.5,
                        BetSize = 10.0,
                        BetType = "B",
                        EventId = 29123456,
                        EventTypeId = 1,
                        FullMarketName = "Test Match/Match Odds",
                        GrossBetAmount = 10.0,
                        MarketName = "Match Odds",
                        MarketType = "O",
                        PlacedDate = DateTime.UtcNow,
                        SelectionId = 47972,
                        SelectionName = "Team A",
                        StartDate = DateTime.UtcNow,
                        TransactionType = "RESULT_WON",
                        TransactionId = 987654321,
                        WinLose = "RESULT_WON",
                    },
                },
            },
            MoreAvailable = false,
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountStatement().ExecuteAsync();

        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task FluentBuilderCallsTheCorrectEndpoint()
    {
        await _api.AccountStatement()
            .ExchangeOnly()
            .Take(50)
            .ExecuteAsync();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/getAccountStatement/");
    }

    [Fact]
    public async Task FluentBuilderRequestBodyShouldBeSerializable()
    {
        await _api.AccountStatement()
            .ExchangeOnly()
            .Take(100)
            .From(10)
            .WithLocale("en-GB")
            .ExecuteAsync();

        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"includeItem\":\"EXCHANGE\"");
        json.Should().Contain("\"recordCount\":100");
        json.Should().Contain("\"fromRecord\":10");
        json.Should().Contain("\"locale\":\"en-GB\"");
    }

    [Fact]
    public async Task FluentBuilderImplicitConversionWorks()
    {
        Task<AccountStatementReport> task = _api.AccountStatement()
            .Today()
            .ExchangeOnly()
            .Take(25);

        var response = await task;

        response.Should().NotBeNull();
        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/getAccountStatement/");
    }

    [Fact]
    public async Task FluentBuilderWithDateRangeWorks()
    {
        var from = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2023, 1, 31, 23, 59, 59, TimeSpan.Zero);

        await _api.AccountStatement()
            .WithItemDateRange(from, to)
            .DepositsAndWithdrawalsOnly()
            .ExecuteAsync();

        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"includeItem\":\"DEPOSITS_WITHDRAWALS\"");
        json.Should().Contain("\"itemDateRange\":");
    }

    [Fact]
    public async Task RequestSerializationIncludesAllProperties()
    {
        var from = new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2023, 6, 30, 23, 59, 59, TimeSpan.Zero);

        await _api.AccountStatement()
            .WithLocale("en-US")
            .From(50)
            .Take(200)
            .WithItemDateRange(from, to)
            .PokerRoomOnly()
            .ExecuteAsync();

        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"locale\":\"en-US\"");
        json.Should().Contain("\"fromRecord\":50");
        json.Should().Contain("\"recordCount\":200");
        json.Should().Contain("\"includeItem\":\"POKER_ROOM\"");
        json.Should().Contain("\"itemDateRange\":");
    }

    [Fact]
    public async Task RequestSerializationWithNullOptionalProperties()
    {
        await _api.AccountStatement()
            .Take(100)
            .ExecuteAsync();

        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"recordCount\":100");
        json.Should().Contain("\"fromRecord\":0");
        json.Should().Contain("\"includeItem\":\"ALL\"");
        json.Should().NotContain("\"locale\":");
        json.Should().NotContain("\"itemDateRange\":");
        json.Should().NotContain("\"wallet\":");
    }

    [Fact]
    public async Task ResponseDeserializationWithMinimalData()
    {
        var expectedResponse = new AccountStatementReport
        {
            AccountStatement =
            [
                new ()
                {
                    RefId = "67890",
                    ItemDate = new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Utc),
                    Amount = -5.25,
                    Balance = 95.50,
                    ItemClass = "COMMISSION",
                },
            ],
            MoreAvailable = true,
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountStatement().ExecuteAsync();

        response.Should().BeEquivalentTo(expectedResponse);
        response.AccountStatement.Should().HaveCount(1);
        response.AccountStatement![0].RefId.Should().Be("67890");
        response.AccountStatement[0].Amount.Should().Be(-5.25);
        response.AccountStatement[0].Balance.Should().Be(95.50);
        response.AccountStatement[0].ItemClass.Should().Be("COMMISSION");
        response.AccountStatement[0].ItemClassData.Should().BeNull();
        response.AccountStatement[0].LegacyData.Should().BeNull();
        response.MoreAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task ResponseDeserializationWithComplexLegacyData()
    {
        var expectedResponse = new AccountStatementReport
        {
            AccountStatement =
            [
                new ()
                {
                    RefId = "BET123456",
                    ItemDate = new DateTime(2023, 6, 15, 18, 45, 30, DateTimeKind.Utc),
                    Amount = 25.75,
                    Balance = 1250.25,
                    ItemClass = "RESULT_WON",
                    ItemClassData = new Dictionary<string, string>
                    {
                        { "commission", "1.29" },
                        { "exchangeRate", "1.0" },
                        { "winLose", "WON" },
                    },
                    LegacyData = new StatementLegacyData
                    {
                        AvgPrice = 3.75,
                        BetSize = 20.0,
                        BetType = "L",
                        EventId = 30123456,
                        EventTypeId = 2,
                        FullMarketName = "Premier League/Over/Under 2.5 Goals",
                        GrossBetAmount = 20.0,
                        MarketName = "Over/Under 2.5 Goals",
                        MarketType = "O/U",
                        PlacedDate = new DateTime(2023, 6, 15, 15, 30, 0, DateTimeKind.Utc),
                        SelectionId = 47973,
                        SelectionName = "Under 2.5",
                        StartDate = new DateTime(2023, 6, 15, 16, 0, 0, DateTimeKind.Utc),
                        TransactionType = "RESULT_WON",
                        TransactionId = 987654322,
                        WinLose = "WON",
                    },
                },
            ],
            MoreAvailable = false,
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountStatement().ExecuteAsync();

        response.Should().BeEquivalentTo(expectedResponse);
        var item = response.AccountStatement![0];
        item.LegacyData.Should().NotBeNull();
        item.LegacyData!.AvgPrice.Should().Be(3.75);
        item.LegacyData.BetSize.Should().Be(20.0);
        item.LegacyData.BetType.Should().Be("L");
        item.LegacyData.EventId.Should().Be(30123456);
        item.LegacyData.EventTypeId.Should().Be(2);
        item.LegacyData.FullMarketName.Should().Be("Premier League/Over/Under 2.5 Goals");
        item.LegacyData.GrossBetAmount.Should().Be(20.0);
        item.LegacyData.MarketName.Should().Be("Over/Under 2.5 Goals");
        item.LegacyData.MarketType.Should().Be("O/U");
        item.LegacyData.PlacedDate.Should().Be(new DateTime(2023, 6, 15, 15, 30, 0, DateTimeKind.Utc));
        item.LegacyData.SelectionId.Should().Be(47973);
        item.LegacyData.SelectionName.Should().Be("Under 2.5");
        item.LegacyData.StartDate.Should().Be(new DateTime(2023, 6, 15, 16, 0, 0, DateTimeKind.Utc));
        item.LegacyData.TransactionType.Should().Be("RESULT_WON");
        item.LegacyData.TransactionId.Should().Be(987654322);
        item.LegacyData.WinLose.Should().Be("WON");
    }

    [Fact]
    public async Task ResponseDeserializationWithEmptyAccountStatement()
    {
        var expectedResponse = new AccountStatementReport
        {
            AccountStatement = [],
            MoreAvailable = false,
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountStatement().ExecuteAsync();

        response.Should().BeEquivalentTo(expectedResponse);
        response.AccountStatement.Should().NotBeNull();
        response.AccountStatement.Should().BeEmpty();
        response.MoreAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task ResponseDeserializationWithNullAccountStatement()
    {
        var expectedResponse = new AccountStatementReport
        {
            AccountStatement = null,
            MoreAvailable = false,
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountStatement().ExecuteAsync();

        response.Should().BeEquivalentTo(expectedResponse);
        response.AccountStatement.Should().BeNull();
        response.MoreAvailable.Should().BeFalse();
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
