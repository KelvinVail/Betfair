using Betfair.Betting;

namespace Betfair.Client;

public class RequestBody
{
    internal Filter Filter { get; set; } = new MarketFilter().Filter;
}
