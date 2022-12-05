using Betfair.Betting;

namespace Betfair.Client;

internal class RequestBody
{
    internal Filter Filter { get; set; } = new MarketFilter().Filter;

    internal IEnumerable<string>? MarketProjection { get; set; }

    internal string? Sort { get; set; }

    internal int? MaxResults { get; set; }
}
