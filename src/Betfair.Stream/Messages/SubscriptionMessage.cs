namespace Betfair.Stream.Messages;

public class SubscriptionMessage
{
    public string Op { get; init; } = string.Empty;

    public int Id { get; init; }

    public MarketFilter? MarketFilter { get; init; }

    public DataFilter? MarketDataFilter { get; init; }

    public string? InitialClk { get; init; }

    public string? Clk { get; init; }
}
