using Betfair.Api;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Requests;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Responses;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class AccountStatementTests : IDisposable
{
    private readonly HttpAdapterStub _client = new();
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
        var query = new AccountStatementQuery();
        await _api.AccountStatement(query);

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/getAccountStatement/");
    }

    [Fact]
    public async Task RequestBodyShouldBeSerializable()
    {
        var query = new AccountStatementQuery()
            .ExchangeOnly()
            .UkWallet()
            .Take(100);

        await _api.AccountStatement(query);
        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"includeItem\":\"EXCHANGE\"");
        json.Should().Contain("\"wallet\":\"UK\"");
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
                        { "exchangeRate", "1.0" }
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
                        WinLose = "RESULT_WON"
                    }
                }
            },
            MoreAvailable = false
        };
        _client.RespondsWithBody = expectedResponse;

        var response = await _api.AccountStatement(new AccountStatementQuery());

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
