namespace Betfair.Betting;

public class MarketFilter
{
    private readonly string _eventTypeId = string.Empty;

    private MarketFilter()
    {
    }

    private MarketFilter(MarketFilter filter, EventType eventType)
    {
        _eventTypeId = eventType.Id;
    }

    public static MarketFilter Create() =>
        new ();

    public MarketFilter With(EventType eventType)
    {
        if (eventType is null) return this;
        return new MarketFilter(this, eventType);
    }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(_eventTypeId))
            return "\"filter\":{}";

        return $"\"filter\":{{\"eventTypeIds\":[\"{_eventTypeId}\"]}}";
    }
}
