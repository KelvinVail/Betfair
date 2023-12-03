namespace Betfair.Api.Requests;

internal sealed class Filter
{
    public List<int>? EventTypeIds { get; internal set; }

    public List<string>? MarketTypeCodes { get; internal set; }

    public List<string>? MarketCountries { get; internal set; }
}