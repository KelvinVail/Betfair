namespace Betfair.Api.Requests;

public class MarketCatalogueQuery
{
    private HashSet<string>? _projection;

    public IReadOnlyCollection<string>? MarketProjection => _projection;

    public string? Sort { get; private set; }

    public int MaxResults { get; private set; } = 1000;

    public MarketCatalogueQuery Include(MarketProjection projection)
    {
        if (projection is null) return this;

        _projection ??= new ();
        _projection.Add(projection.Value);
        return this;
    }

    public MarketCatalogueQuery OrderBy(MarketSort order)
    {
        if (order is null) return this;

        Sort = order.Value;
        return this;
    }

    public MarketCatalogueQuery Take(int value)
    {
        MaxResults = value;
        return this;
    }
}
