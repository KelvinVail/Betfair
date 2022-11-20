using Betfair.Betting;

namespace Betfair.Client;

public class RequestBody
{
    public MarketFilter Filter { get; set; } = new ();
}
