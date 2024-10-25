namespace Betfair.Api.Responses.Markets;

public class MarketBook
{
    /// <summary>
    /// Gets the market ID.
    /// The unique identifier for the market. MarketId's are prefixed with '1.'
    /// </summary>
    public string? MarketId { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the data in this response is delayed.
    /// True if the data returned by listMarketBook will be delayed.
    /// The data may be delayed because you are not logged in with a funded account,
    /// or you are using an Application Key that does not allow up-to-date data.
    /// </summary>
    public bool IsMarketDataDelayed { get; internal set; }

    /// <summary>
    /// Gets the status of the market.
    /// For example OPEN, SUSPENDED, CLOSED (settled), etc.
    /// </summary>
    public string? Status { get; internal set; }

    /// <summary>
    /// Gets the number of seconds an order is held until it is submitted into the market.
    /// Orders are usually delayed when the market is in-play.
    /// </summary>
    public int BetDelay { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the market starting price has been reconciled.
    /// </summary>
    public bool BspReconciled { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the market is completely formed.
    /// If false, runners may be added to the market.
    /// </summary>
    public bool Complete { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the market is currently in play.
    /// </summary>
    public bool InPlay { get; internal set; }

    /// <summary>
    /// Gets the number of selections that could be settled as winners.
    /// </summary>
    public int NumberOfWinners { get; internal set; }

    /// <summary>
    /// Gets the number of runners in the market.
    /// </summary>
    public int NumberOfRunners { get; internal set; }

    /// <summary>
    /// Gets the number of runners that are currently active.
    /// An active runner is a selection available for betting.
    /// </summary>
    public int NumberOfActiveRunners { get; internal set; }

    /// <summary>
    /// Gets the most recent time an order was executed.
    /// </summary>
    public DateTimeOffset LastMatchTime { get; internal set; }

    /// <summary>
    /// Gets the total amount matched on the market.
    /// </summary>
    public double TotalMatched { get; internal set; }

    /// <summary>
    /// Gets the total amount of orders that remain unmatched.
    /// </summary>
    public double TotalAvailable { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether cross-matching is enabled for this market.
    /// </summary>
    public bool CrossMatching { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether runners in the market can be voided.
    /// Please note - this doesn't include horse racing markets under which bets are voided on non-runners with any applicable reduction factor applied.
    /// </summary>
    public bool RunnersVoidable { get; internal set; }

    /// <summary>
    /// Gets the version of the market.
    /// The version increments whenever the market status changes, for example, turning in-play, or suspended when a goal is scored.
    /// </summary>
    public long Version { get; internal set; }
}
