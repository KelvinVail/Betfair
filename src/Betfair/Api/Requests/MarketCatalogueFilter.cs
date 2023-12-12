using Betfair.Core;

namespace Betfair.Api.Requests;

public class MarketCatalogueFilter : MarketFilter<MarketCatalogueFilter>
{
    internal Filter Filter { get; } = new ();

    public List<string>? MarketProjection { get; set; }

    public string? Sort { get; set; }

    public int MaxResults { get; set; } = 1000;

    public MarketCatalogueFilter WithEventTypeId(int id)
    {
        Filter.EventTypeIds ??= new ();
        Filter.EventTypeIds.Add(id);
        return this;
    }

    public MarketCatalogueFilter WithEventType(EventType eventType)
    {
        Filter.EventTypeIds ??= new ();
        Filter.EventTypeIds.Add(eventType.Id);
        return this;
    }
}
