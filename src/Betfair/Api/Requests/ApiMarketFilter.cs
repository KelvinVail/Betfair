using Betfair.Core;

namespace Betfair.Api.Requests;

[JsonSerializable(typeof(ApiMarketFilter))]
public sealed class ApiMarketFilter : MarketFilter<ApiMarketFilter>
{
    [JsonPropertyName("marketStartTime")]
    public DateRange? MarketStartTime { get; private set; }

    public ApiMarketFilter FromMarketStart(DateTimeOffset dateTime)
    {
        MarketStartTime ??= new DateRange();
        MarketStartTime.From = ToUtcString(dateTime);
        return this;
    }

    public ApiMarketFilter ToMarketStart(DateTimeOffset dateTime)
    {
        MarketStartTime ??= new DateRange();
        MarketStartTime.To = ToUtcString(dateTime);
        return this;
    }

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
