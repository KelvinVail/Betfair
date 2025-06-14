using Betfair.Core;

namespace Betfair.Api.Requests;

/// <summary>
/// The filter to select desired markets. All markets that match the criteria in the filter are selected.
/// </summary>
public sealed class ApiMarketFilter : MarketFilter<ApiMarketFilter>
{
    /// <summary>
    /// Gets the market start time range filter.
    /// </summary>
    [JsonPropertyName("marketStartTime")]
    public DateRange? MarketStartTime { get; private set; }

    /// <summary>
    /// Sets the start time from which to filter markets.
    /// </summary>
    /// <param name="dateTime">The start time from which to filter markets.</param>
    /// <returns>The current <see cref="ApiMarketFilter"/> instance.</returns>
    public ApiMarketFilter FromMarketStart(DateTimeOffset dateTime)
    {
        MarketStartTime ??= new DateRange();
        MarketStartTime.From = ToUtcString(dateTime);
        return this;
    }

    /// <summary>
    /// Sets the end time to which to filter markets.
    /// </summary>
    /// <param name="dateTime">The end time to which to filter markets.</param>
    /// <returns>The current <see cref="ApiMarketFilter"/> instance.</returns>
    public ApiMarketFilter ToMarketStart(DateTimeOffset dateTime)
    {
        MarketStartTime ??= new DateRange();
        MarketStartTime.To = ToUtcString(dateTime);
        return this;
    }

    /// <summary>
    /// Configures the filter for today's horse racing card in the UK and Ireland.
    /// </summary>
    /// <returns>The current <see cref="ApiMarketFilter"/> instance.</returns>
    public ApiMarketFilter TodaysCard()
    {
        WithMarketTypes(MarketType.Win);
        WithEventTypes(EventType.HorseRacing);
        WithCountries(Country.UnitedKingdom);
        WithCountries(Country.Ireland);
        FromMarketStart(DateTime.Today);
        ToMarketStart(DateTime.Today.AddDays(1));

        return this;
    }

    private static string ToUtcString(DateTimeOffset dateTime) =>
        dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", DateTimeFormatInfo.InvariantInfo);
}
