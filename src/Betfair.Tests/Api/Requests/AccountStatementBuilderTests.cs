using System.Globalization;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Enums;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Requests;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Responses;
using Betfair.Api.Betting;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api.Requests;

public class AccountStatementBuilderTests : IDisposable
{
    private readonly HttpAdapterStub _client = new ();
    private readonly AccountStatementBuilder _builder;
    private bool _disposedValue;

    public AccountStatementBuilderTests()
    {
        _client.RespondsWithBody = new AccountStatementReport();
        _builder = new AccountStatementBuilder(_client, "https://api.betfair.com/exchange/account/rest/v1.0");
    }

    [Fact]
    public void WithLocaleReturnsBuilder()
    {
        var result = _builder.WithLocale("en-GB");

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void FromReturnsBuilder()
    {
        var result = _builder.From(10);

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void FromWithNegativeValueSetsToZero()
    {
        var result = _builder.From(-5);

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void TakeReturnsBuilder()
    {
        var result = _builder.Take(100);

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void TakeWithZeroOrNegativeValueSetsToOne()
    {
        var result = _builder.Take(0);

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void WithItemDateRangeReturnsBuilder()
    {
        var from = DateTimeOffset.UtcNow.AddDays(-7);
        var to = DateTimeOffset.UtcNow;

        var result = _builder.WithItemDateRange(from, to);

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void WithItemDateRangeObjectReturnsBuilder()
    {
        var dateRange = new DateRange
        {
            From = DateTimeOffset.UtcNow.AddDays(-7).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            To = DateTimeOffset.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
        };

        var result = _builder.WithItemDateRange(dateRange);

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void IncludeAllReturnsBuilder()
    {
        var result = _builder.IncludeAll();

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void DepositsAndWithdrawalsOnlyReturnsBuilder()
    {
        var result = _builder.DepositsAndWithdrawalsOnly();

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void ExchangeOnlyReturnsBuilder()
    {
        var result = _builder.ExchangeOnly();

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void PokerRoomOnlyReturnsBuilder()
    {
        var result = _builder.PokerRoomOnly();

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void LastWeekReturnsBuilder()
    {
        var result = _builder.LastWeek();

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void LastMonthReturnsBuilder()
    {
        var result = _builder.LastMonth();

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void Last90DaysReturnsBuilder()
    {
        var result = _builder.Last90Days();

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void TodayReturnsBuilder()
    {
        var result = _builder.Today();

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void YesterdayReturnsBuilder()
    {
        var result = _builder.Yesterday();

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public async Task ExecuteAsyncCallsCorrectEndpoint()
    {
        await _builder.ExecuteAsync();

        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/getAccountStatement/");
    }

    [Fact]
    public async Task ExecuteAsyncWithParametersSerializesCorrectly()
    {
        await _builder
            .WithLocale("en-GB")
            .From(10)
            .Take(50)
            .ExchangeOnly()
            .ExecuteAsync();

        var json = JsonSerializer.Serialize(_client.LastContentSent, SerializerContext.Default.AccountStatementRequest);

        json.Should().Contain("\"locale\":\"en-GB\"");
        json.Should().Contain("\"fromRecord\":10");
        json.Should().Contain("\"recordCount\":50");
        json.Should().Contain("\"includeItem\":\"EXCHANGE\"");
    }

    [Fact]
    public async Task ImplicitConversionWorks()
    {
        Task<AccountStatementReport> task = _builder.ExchangeOnly().Take(25);

        var response = await task;

        response.Should().NotBeNull();
        _client.LastUriCalled.Should().Be(
            "https://api.betfair.com/exchange/account/rest/v1.0/getAccountStatement/");
    }

    [Fact]
    public void FluentChainingWorksCorrectly()
    {
        var from = DateTimeOffset.UtcNow.AddDays(-30);
        var to = DateTimeOffset.UtcNow;

        var result = _builder
            .WithLocale("en-US")
            .From(20)
            .Take(75)
            .WithItemDateRange(from, to)
            .DepositsAndWithdrawalsOnly();

        result.Should().BeSameAs(_builder);
    }

    [Fact]
    public void ImplicitConversionThrowsOnNullBuilder()
    {
        AccountStatementBuilder? nullBuilder = null;

        var action = () =>
        {
            Task<AccountStatementReport> task = nullBuilder!;
            return task;
        };

        action.Should().ThrowAsync<ArgumentNullException>();
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
        }

        _disposedValue = true;
    }
}
