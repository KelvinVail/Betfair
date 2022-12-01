namespace Betfair.Betting;

public class EventTypesResponse
{
    public EventType EventType { get; set; } = new ();

    public int MarketCount { get; set; }
}
