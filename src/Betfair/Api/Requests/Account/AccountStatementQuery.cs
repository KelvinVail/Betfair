using System.Globalization;
using Betfair.Api.Requests;
using Betfair.Core.Enums;

namespace Betfair.Api.Requests.Account;

/// <summary>
/// Fluent query builder for account statement requests.
/// </summary>
public class AccountStatementQuery
{
    /// <summary>
    /// Gets the locale for the response.
    /// </summary>
    public string? Locale { get; private set; }

    /// <summary>
    /// Gets the starting record index.
    /// </summary>
    public int FromRecord { get; private set; }

    /// <summary>
    /// Gets the number of records to return.
    /// </summary>
    public int RecordCount { get; private set; } = 100;

    /// <summary>
    /// Gets the item date range filter.
    /// </summary>
    public DateRange? ItemDateRange { get; private set; }

    /// <summary>
    /// Gets the include item filter.
    /// </summary>
    public IncludeItem IncludeItem { get; private set; } = IncludeItem.All;

    /// <summary>
    /// Gets the wallet filter.
    /// </summary>
    public Wallet Wallet { get; private set; } = Wallet.Uk;

    /// <summary>
    /// Sets the locale for the response.
    /// </summary>
    /// <param name="locale">The locale to use.</param>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery WithLocale(string locale)
    {
        Locale = locale;
        return this;
    }

    /// <summary>
    /// Sets the starting record index for pagination.
    /// </summary>
    /// <param name="fromRecord">The starting record index.</param>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery From(int fromRecord)
    {
        FromRecord = Math.Max(0, fromRecord);
        return this;
    }

    /// <summary>
    /// Sets the number of records to return.
    /// </summary>
    /// <param name="recordCount">The number of records to return (max 100).</param>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery Take(int recordCount)
    {
        RecordCount = Math.Max(1, Math.Min(100, recordCount));
        return this;
    }

    /// <summary>
    /// Sets the item date range filter.
    /// </summary>
    /// <param name="from">The start date.</param>
    /// <param name="to">The end date.</param>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery WithItemDateRange(DateTimeOffset from, DateTimeOffset to)
    {
        ItemDateRange = new DateRange
        {
            From = from.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            To = to.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
        };
        return this;
    }

    /// <summary>
    /// Sets the include item filter.
    /// </summary>
    /// <param name="includeItem">The include item filter to use.</param>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery WithIncludeItem(IncludeItem includeItem)
    {
        IncludeItem = includeItem;
        return this;
    }

    /// <summary>
    /// Sets the wallet filter.
    /// </summary>
    /// <param name="wallet">The wallet to filter by.</param>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery WithWallet(Wallet wallet)
    {
        Wallet = wallet;
        return this;
    }

    /// <summary>
    /// Filters to include all items.
    /// </summary>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery IncludeAllItems()
    {
        IncludeItem = Account.IncludeItem.All;
        return this;
    }

    /// <summary>
    /// Filters to include deposits and withdrawals only.
    /// </summary>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery DepositsAndWithdrawalsOnly()
    {
        IncludeItem = Account.IncludeItem.DepositsWithdrawals;
        return this;
    }

    /// <summary>
    /// Filters to include exchange items only.
    /// </summary>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery ExchangeOnly()
    {
        IncludeItem = Account.IncludeItem.Exchange;
        return this;
    }

    /// <summary>
    /// Filters to include poker room items only.
    /// </summary>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery PokerRoomOnly()
    {
        IncludeItem = Account.IncludeItem.PokerRoom;
        return this;
    }

    /// <summary>
    /// Sets the wallet to UK.
    /// </summary>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery UkWallet()
    {
        Wallet = Account.Wallet.Uk;
        return this;
    }

    /// <summary>
    /// Sets the wallet to Australian.
    /// </summary>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery AustralianWallet()
    {
        Wallet = Account.Wallet.Australian;
        return this;
    }

    /// <summary>
    /// Filters to items from the last 7 days.
    /// </summary>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery LastWeek()
    {
        var now = DateTimeOffset.UtcNow;
        return WithItemDateRange(now.AddDays(-7), now);
    }

    /// <summary>
    /// Filters to items from the last 30 days.
    /// </summary>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery LastMonth()
    {
        var now = DateTimeOffset.UtcNow;
        return WithItemDateRange(now.AddDays(-30), now);
    }

    /// <summary>
    /// Filters to items from today.
    /// </summary>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery Today()
    {
        var today = DateTimeOffset.UtcNow.Date;
        return WithItemDateRange(today, today.AddDays(1));
    }

    /// <summary>
    /// Filters to items from yesterday.
    /// </summary>
    /// <returns>This <see cref="AccountStatementQuery"/>.</returns>
    public AccountStatementQuery Yesterday()
    {
        var yesterday = DateTimeOffset.UtcNow.Date.AddDays(-1);
        return WithItemDateRange(yesterday, yesterday.AddDays(1));
    }
}
