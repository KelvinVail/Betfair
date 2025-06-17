using Betfair.Api;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Enums;
using Betfair.Api.Betting.Endpoints.ListClearedOrders;
using Betfair.Api.Betting.Endpoints.ListClearedOrders.Enums;
using Betfair.Api.Betting.Endpoints.ListMarketBook;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;
using Betfair.Api.Betting.Enums;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class FluentApiIntegrationTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly BetfairApiClient _api;
    private bool _disposedValue;

    public FluentApiIntegrationTests()
    {
        _api = new BetfairApiClient(_client);
    }

    [Fact]
    public async Task ClearedOrdersWithFluentQueryWorksCorrectly()
    {
        _client.RespondsWithBody = new ClearedOrderSummaryReport();

        var query = new ClearedOrdersQuery()
            .WithBetStatus(BetStatus.Settled)
            .WithMarkets("1.123", "1.456")
            .WithSettledDateRange(
                new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2023, 1, 31, 23, 59, 59, TimeSpan.Zero))
            .BackBetsOnly()
            .GroupBy(GroupBy.Market)
            .IncludeItemDescriptions()
            .From(10)
            .Take(100);

        await _api.ClearedOrders(query);

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listClearedOrders/");
        _client.LastContentSent.Should().NotBeNull();
    }

    [Fact]
    public async Task MarketBookWithFluentQueryWorksCorrectly()
    {
        _client.RespondsWithBody = Array.Empty<MarketBook>();

        var priceProjection = new PriceProjectionBuilder()
            .IncludeBestPrices()
            .WithVirtualPrices()
            .WithBestPricesDepth(3)
            .Build();

        var query = new MarketBookQuery()
            .WithPriceProjection(priceProjection)
            .ExecutableOrdersOnly()
            .RollupByPrice()
            .IncludeOverallPositions()
            .PartitionByStrategy()
            .WithCurrency("GBP")
            .WithLocale("en-GB");

        await _api.MarketBook(new[] { "1.123", "1.456" }, query);

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listMarketBook/");
        _client.LastContentSent.Should().NotBeNull();
    }

    [Fact]
    public async Task RunnerBookWithFluentQueryWorksCorrectly()
    {
        _client.RespondsWithBody = Array.Empty<MarketBook>();

        var query = new MarketBookQuery()
            .WithPriceProjection(new PriceProjectionBuilder().BasicPrices())
            .ExecutableOrdersOnly()
            .NoMatchRollup();

        await _api.RunnerBook("1.123", 456789L, query: query);

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/betting/rest/v1.0/listRunnerBook/");
        _client.LastContentSent.Should().NotBeNull();
    }

    [Fact]
    public async Task AccountStatementWithFluentQueryWorksCorrectly()
    {
        _client.RespondsWithBody = new AccountStatementReport();

        var query = new AccountStatementQuery()
            .LastMonth()
            .ExchangeOnly()
            .UkWallet()
            .From(0)
            .Take(50)
            .WithLocale("en-GB");

        await _api.AccountStatement(query);

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/getAccountStatement/");
        _client.LastContentSent.Should().NotBeNull();
    }

    [Fact]
    public void PriceProjectionBuilderImplicitConversionWorks()
    {
        var query = new MarketBookQuery()
            .WithPriceProjection(new PriceProjectionBuilder()
                .ComprehensivePrices()
                .WithVirtualPrices()
                .WithRolloverStakes());

        query.PriceProjection.Should().NotBeNull();
        query.PriceProjection!.PriceData.Should().HaveCount(3);
        query.PriceProjection.Virtualise.Should().BeTrue();
        query.PriceProjection.RolloverStakes.Should().BeTrue();
    }

    [Fact]
    public void FluentChainingDemonstratesReadability()
    {
        // This demonstrates how much more readable the fluent API is
        var clearedOrdersQuery = new ClearedOrdersQuery()
            .SettledOnly()
            .WithMarkets("1.123", "1.456", "1.789")
            .LastWeek()
            .BackBetsOnly()
            .GroupBy(GroupBy.Market)
            .IncludeItemDescriptions()
            .Take(500);

        var marketBookQuery = new MarketBookQuery()
            .WithPriceProjection(PriceProjectionBuilder.Create()
                .ComprehensivePrices()
                .WithVirtualPrices()
                .WithBestPricesDepth(5))
            .ExecutableOrdersOnly()
            .RollupByAveragePrice()
            .IncludeOverallPositions()
            .WithCurrency("GBP");

        var accountQuery = new AccountStatementQuery()
            .Today()
            .ExchangeOnly()
            .UkWallet()
            .Take(100);

        // All queries should be properly configured
        clearedOrdersQuery.BetStatus.Should().Be(BetStatus.Settled);
        clearedOrdersQuery.MarketIds.Should().HaveCount(3);
        clearedOrdersQuery.Side.Should().Be(Side.Back);

        marketBookQuery.OrderProjection.Should().Be(OrderProjection.Executable);
        marketBookQuery.MatchProjection.Should().Be(MatchProjection.RolledUpByAvgPrice);
        marketBookQuery.PriceProjection.Should().NotBeNull();

        accountQuery.IncludeItem.Should().Be(IncludeItem.Exchange);
        accountQuery.Wallet.Should().Be(Wallet.Uk);
        accountQuery.RecordCount.Should().Be(100);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _api.Dispose();
        _disposedValue = true;
    }
}
