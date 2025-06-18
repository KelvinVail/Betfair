using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Enums;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Requests;
using Betfair.Api.Betting;

namespace Betfair.Tests.Api.Accounts.Endpoints.GetAccountStatement.Requests;

public class AccountStatementRequestTests
{
    [Fact]
    public void CanSerializeRequestWithAllProperties()
    {
        var request = new AccountStatementRequest
        {
            Locale = "en-GB",
            FromRecord = 10,
            RecordCount = 50,
            ItemDateRange = new DateRange
            {
                From = "2023-01-01T00:00:00Z",
                To = "2023-01-31T23:59:59Z",
            },
            IncludeItem = IncludeItem.Exchange,
            Wallet = "UK",
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"locale\":\"en-GB\"");
        json.Should().Contain("\"fromRecord\":10");
        json.Should().Contain("\"recordCount\":50");
        json.Should().Contain("\"includeItem\":\"EXCHANGE\"");
        json.Should().Contain("\"wallet\":\"UK\"");
        json.Should().Contain("\"itemDateRange\":");
        json.Should().Contain("\"from\":\"2023-01-01T00:00:00Z\"");
        json.Should().Contain("\"to\":\"2023-01-31T23:59:59Z\"");
    }

    [Fact]
    public void CanSerializeRequestWithMinimalProperties()
    {
        var request = new AccountStatementRequest
        {
            FromRecord = 0,
            RecordCount = 100,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"fromRecord\":0");
        json.Should().Contain("\"recordCount\":100");
        json.Should().NotContain("\"locale\":");
        json.Should().NotContain("\"itemDateRange\":");
        json.Should().NotContain("\"includeItem\":");
        json.Should().NotContain("\"wallet\":");
    }

    [Fact]
    public void CanSerializeRequestWithNullOptionalProperties()
    {
        var request = new AccountStatementRequest
        {
            Locale = null,
            FromRecord = 25,
            RecordCount = 75,
            ItemDateRange = null,
            IncludeItem = null,
            Wallet = null,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"fromRecord\":25");
        json.Should().Contain("\"recordCount\":75");
        json.Should().NotContain("\"locale\":");
        json.Should().NotContain("\"itemDateRange\":");
        json.Should().NotContain("\"includeItem\":");
        json.Should().NotContain("\"wallet\":");
    }

    [Theory]
    [InlineData(IncludeItem.All, "ALL")]
    [InlineData(IncludeItem.DepositsWithdrawals, "DEPOSITS_WITHDRAWALS")]
    [InlineData(IncludeItem.Exchange, "EXCHANGE")]
    [InlineData(IncludeItem.PokerRoom, "POKER_ROOM")]
    public void CanSerializeRequestWithDifferentIncludeItemValues(IncludeItem includeItem, string expectedJson)
    {
        var request = new AccountStatementRequest
        {
            FromRecord = 0,
            RecordCount = 100,
            IncludeItem = includeItem,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain($"\"includeItem\":\"{expectedJson}\"");
    }

    [Fact]
    public void CanSerializeRequestWithDateRange()
    {
        var request = new AccountStatementRequest
        {
            FromRecord = 0,
            RecordCount = 100,
            ItemDateRange = new DateRange
            {
                From = "2023-06-01T00:00:00Z",
                To = "2023-06-30T23:59:59Z",
            },
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"itemDateRange\":");
        json.Should().Contain("\"from\":\"2023-06-01T00:00:00Z\"");
        json.Should().Contain("\"to\":\"2023-06-30T23:59:59Z\"");
    }

    [Fact]
    public void CanSerializeRequestWithPartialDateRange()
    {
        var request = new AccountStatementRequest
        {
            FromRecord = 0,
            RecordCount = 100,
            ItemDateRange = new DateRange
            {
                From = "2023-06-01T00:00:00Z",
                To = null,
            },
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"itemDateRange\":");
        json.Should().Contain("\"from\":\"2023-06-01T00:00:00Z\"");
        json.Should().NotContain("\"to\":");
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("en-GB")]
    [InlineData("de-DE")]
    [InlineData("fr-FR")]
    [InlineData("es-ES")]
    public void CanSerializeRequestWithDifferentLocales(string locale)
    {
        var request = new AccountStatementRequest
        {
            Locale = locale,
            FromRecord = 0,
            RecordCount = 100,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain($"\"locale\":\"{locale}\"");
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(10, 50)]
    [InlineData(100, 200)]
    [InlineData(1000, 1000)]
    public void CanSerializeRequestWithDifferentPaginationValues(int fromRecord, int recordCount)
    {
        var request = new AccountStatementRequest
        {
            FromRecord = fromRecord,
            RecordCount = recordCount,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain($"\"fromRecord\":{fromRecord}");
        json.Should().Contain($"\"recordCount\":{recordCount}");
    }

    [Theory]
    [InlineData("UK")]
    [InlineData("AUS")]
    [InlineData("EUR")]
    [InlineData("US")]
    public void CanSerializeRequestWithDifferentWallets(string wallet)
    {
        var request = new AccountStatementRequest
        {
            FromRecord = 0,
            RecordCount = 100,
            Wallet = wallet,
        };

        var json = JsonSerializer.Serialize(request, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain($"\"wallet\":\"{wallet}\"");
    }
}
