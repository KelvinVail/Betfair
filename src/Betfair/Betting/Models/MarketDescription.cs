namespace Betfair.Betting.Models;

public sealed class MarketDescription
{
    public bool PersistenceEnabled { get; init; }

    public bool BspMarket { get; init; }

    public DateTimeOffset MarketTime { get; init; }

    public DateTimeOffset SuspendTime { get; init; }

    public string BettingType { get; init; } = string.Empty;

    public bool TurnInPlayEnabled { get; init; }

    public string MarketType { get; init; } = string.Empty;

    public string Regulator { get; init; } = string.Empty;

    public decimal MarketBaseRate { get; init; }

    public bool DiscountAllowed { get; init; }

    public string Wallet { get; init; } = string.Empty;

    public string Rules { get; init; } = string.Empty;

    public bool RulesHasDate { get; init; }

    public string Clarifications { get; init; } = string.Empty;

    public string RaceType { get; init; } = string.Empty;

    public LadderDescription PriceLadderDescription { get; init; } = new ();
}
