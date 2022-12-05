namespace Betfair.Betting.Models;

public sealed class MarketCatalogue
{
    public string MarketId { get; init; } = string.Empty;

    public string MarketName { get; init; } = string.Empty;

    public decimal TotalMatched { get; init; }

    public DateTimeOffset? MarketStartTime { get; init; }

    public Competition? Competition { get; init; }

    public MarketEvent? Event { get; init; }

    public EventType? EventType { get; init; }

    public MarketDescription? Description { get; init; }

    public IEnumerable<Runner>? Runners { get; init; }
}
