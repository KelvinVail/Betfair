using System.Globalization;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Enums;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Responses;
using Betfair.Api.Betting;
using Betfair.Core.Client;

namespace Betfair.Api.Accounts.Endpoints.GetAccountStatement.Requests;

/// <summary>
/// Fluent builder for account statement endpoint requests.
/// </summary>
public class AccountStatementBuilder
{
    private readonly HttpAdapter _client;
    private readonly string _baseUrl;

    internal AccountStatementBuilder(HttpAdapter client, string baseUrl)
    {
        _client = client;
        _baseUrl = baseUrl;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountStatementBuilder"/> class for testing purposes.
    /// </summary>
    protected AccountStatementBuilder()
    {
        _client = null!;
        _baseUrl = string.Empty;
    }

    /// <summary>
    /// Gets the locale that will be used for the request.
    /// </summary>
    public string? Locale { get; private set; }

    /// <summary>
    /// Gets the starting record index for the request.
    /// </summary>
    public int FromRecord { get; private set; }

    /// <summary>
    /// Gets the number of records that will be returned.
    /// </summary>
    public int RecordCount { get; private set; } = 100;

    /// <summary>
    /// Gets the date range filter for the request.
    /// </summary>
    public DateRange? ItemDateRange { get; private set; }

    /// <summary>
    /// Gets the include item filter for the request.
    /// </summary>
    public IncludeItem IncludeItem { get; private set; } = IncludeItem.All;

    /// <summary>
    /// Implicitly converts the builder to a Task by executing the request.
    /// </summary>
    /// <param name="builder">The builder to convert.</param>
    /// <returns>A Task representing the account statement request.</returns>
    public static implicit operator Task<AccountStatementReport>(AccountStatementBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.ExecuteAsync();
    }

    /// <summary>
    /// The language to be used where applicable. If not specified, the customer account default is returned.
    /// </summary>
    /// <param name="locale">The locale to use.</param>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder WithLocale(string locale)
    {
        Locale = locale;
        return this;
    }

    /// <summary>
    /// Specifies the first record that will be returned. Records start at index zero, not one.
    /// </summary>
    /// <param name="fromRecord">The starting record index.</param>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder From(int fromRecord)
    {
        FromRecord = Math.Max(0, fromRecord);
        return this;
    }

    /// <summary>
    /// Specifies how many records will be returned, from the index position 'fromRecord'.
    /// </summary>
    /// <param name="recordCount">The number of records to return.</param>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder Take(int recordCount)
    {
        RecordCount = Math.Max(1, recordCount);
        return this;
    }

    /// <summary>
    /// Return items with an itemDate within this date range. Both from and to date boundaries are inclusive.
    /// </summary>
    /// <param name="from">The start date.</param>
    /// <param name="to">The end date.</param>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder WithItemDateRange(DateTimeOffset from, DateTimeOffset to)
    {
        ItemDateRange = new DateRange
        {
            From = from.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            To = to.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
        };
        return this;
    }

    /// <summary>
    /// Return items with an itemDate within this date range. Both from and to date boundaries are inclusive.
    /// </summary>
    /// <param name="dateRange">The date range.</param>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder WithItemDateRange(DateRange dateRange)
    {
        ItemDateRange = dateRange;
        return this;
    }

    /// <summary>
    /// Filters to include all items.
    /// </summary>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder IncludeAll()
    {
        IncludeItem = IncludeItem.All;
        return this;
    }

    /// <summary>
    /// Filters to include deposits and withdrawals only.
    /// </summary>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder DepositsAndWithdrawalsOnly()
    {
        IncludeItem = IncludeItem.DepositsWithdrawals;
        return this;
    }

    /// <summary>
    /// Filters to include exchange items only.
    /// </summary>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder ExchangeOnly()
    {
        IncludeItem = IncludeItem.Exchange;
        return this;
    }

    /// <summary>
    /// Filters to include poker room items only.
    /// </summary>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder PokerRoomOnly()
    {
        IncludeItem = IncludeItem.PokerRoom;
        return this;
    }

    /// <summary>
    /// Filters to items from the last 7 days.
    /// </summary>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder LastWeek()
    {
        var now = DateTimeOffset.UtcNow;
        return WithItemDateRange(now.AddDays(-7), now);
    }

    /// <summary>
    /// Filters to items from the last 30 days.
    /// </summary>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder LastMonth()
    {
        var now = DateTimeOffset.UtcNow;
        return WithItemDateRange(now.AddDays(-30), now);
    }

    /// <summary>
    /// Filters to items from the last 90 days.
    /// </summary>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder Last90Days()
    {
        var now = DateTimeOffset.UtcNow;
        return WithItemDateRange(now.AddDays(-90), now);
    }

    /// <summary>
    /// Filters to items from today.
    /// </summary>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder Today()
    {
        var today = DateTimeOffset.UtcNow.Date;
        return WithItemDateRange(today, today.AddDays(1));
    }

    /// <summary>
    /// Filters to items from yesterday.
    /// </summary>
    /// <returns>This <see cref="AccountStatementBuilder"/>.</returns>
    public AccountStatementBuilder Yesterday()
    {
        var yesterday = DateTimeOffset.UtcNow.Date.AddDays(-1);
        return WithItemDateRange(yesterday, yesterday.AddDays(1));
    }

    /// <summary>
    /// Executes the account statement request.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="AccountStatementReport"/>.</returns>
    public virtual Task<AccountStatementReport> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = new AccountStatementRequest
        {
            Locale = Locale,
            FromRecord = FromRecord,
            RecordCount = RecordCount,
            ItemDateRange = ItemDateRange,
            IncludeItem = IncludeItem,
        };

        return _client.PostAsync<AccountStatementReport>(
            new Uri($"{_baseUrl}/getAccountStatement/"),
            request,
            cancellationToken);
    }

    /// <summary>
    /// Converts the builder to a Task by executing the request.
    /// </summary>
    /// <returns>A Task representing the account statement request.</returns>
    public virtual Task<AccountStatementReport> ToTask()
    {
        return ExecuteAsync();
    }
}
