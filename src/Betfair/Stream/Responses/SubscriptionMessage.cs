#nullable enable
namespace Betfair.Stream.Responses;

public class SubscriptionMessage
{
    public string Op { get; init; } = string.Empty;

    public int Id { get; init; }

    public StreamMarketFilter? MarketFilter { get; init; }

    public MarketDataFilter? MarketDataFilter { get; init; }

    public string? InitialClk { get; init; }

    public string? Clk { get; init; }
}
