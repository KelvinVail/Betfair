namespace Betfair.Api.Requests;

public class MarketCatalogueFilter
{
    public ApiMarketFilter Filter { get; } = new ();

    public List<string>? MarketProjection { get; set; }

    public string? Sort { get; set; }

    public int MaxResults { get; set; } = 1000;
}
