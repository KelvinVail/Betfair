using Betfair.Core;

namespace Betfair.Api.Requests;

public class ApiMarketFilter : MarketFilter<ApiMarketFilter>
{
    public DateRange? MarketStartTime { get; private set; }

    public ApiMarketFilter FromMarketStart(DateTimeOffset dateTime)
    {
        MarketStartTime ??= new ();
        MarketStartTime.From = ToUtcString(dateTime);
        return this;
    }

    public ApiMarketFilter ToMarketStart(DateTimeOffset dateTime)
    {
        MarketStartTime ??= new ();
        MarketStartTime.To = ToUtcString(dateTime);
        return this;
    }

    private static string ToUtcString(DateTimeOffset dateTime) =>
        dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", DateTimeFormatInfo.InvariantInfo);
}
