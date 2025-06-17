using Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;

namespace Betfair.Api.Betting.Endpoints.ListMarketBook;

public class MarketBook
{
    /// <summary>
    /// Gets the market ID.
    /// The unique identifier for the market. MarketId's are prefixed with '1.'
    /// </summary>
    [JsonPropertyName("marketId")]
    public string? MarketId { get; init; }

    /// <summary>
    /// Gets a value indicating whether the data in this response is delayed.
    /// True if the data returned by listMarketBook will be delayed.
    /// The data may be delayed because you are not logged in with a funded account,
    /// or you are using an Application Key that does not allow up-to-date data.
    /// </summary>
    [JsonPropertyName("isMarketDataDelayed")]
    public bool IsMarketDataDelayed { get; init; }

    /// <summary>
    /// Gets the status of the market.
    /// For example OPEN, SUSPENDED, CLOSED (settled), etc.
    /// </summary>
    [JsonPropertyName("status")]
    public MarketStatus? Status { get; init; }

    /// <summary>
    /// Gets the number of seconds an order is held until it is submitted into the market.
    /// Orders are usually delayed when the market is in-play.
    /// </summary>
    [JsonPropertyName("betDelay")]
    public int BetDelay { get; init; }

    /// <summary>
    /// Gets a value indicating whether the market starting price has been reconciled.
    /// </summary>
    [JsonPropertyName("bspReconciled")]
    public bool BspReconciled { get; init; }

    /// <summary>
    /// Gets a value indicating whether the market is completely formed.
    /// If false, runners may be added to the market.
    /// </summary>
    [JsonPropertyName("complete")]
    public bool Complete { get; init; }

    /// <summary>
    /// Gets a value indicating whether the market is currently in play.
    /// </summary>
    [JsonPropertyName("inplay")]
    public bool InPlay { get; init; }

    /// <summary>
    /// Gets the number of selections that could be settled as winners.
    /// </summary>
    [JsonPropertyName("numberOfWinners")]
    public int NumberOfWinners { get; init; }

    /// <summary>
    /// Gets the number of runners in the market.
    /// </summary>
    [JsonPropertyName("numberOfRunners")]
    public int NumberOfRunners { get; init; }

    /// <summary>
    /// Gets the number of runners that are currently active.
    /// An active runner is a selection available for betting.
    /// </summary>
    [JsonPropertyName("numberOfActiveRunners")]
    public int NumberOfActiveRunners { get; init; }

    /// <summary>
    /// Gets the most recent time an order was executed.
    /// </summary>
    [JsonPropertyName("lastMatchTime")]
    public DateTimeOffset LastMatchTime { get; init; }

    /// <summary>
    /// Gets the total amount matched on the market.
    /// </summary>
    [JsonPropertyName("totalMatched")]
    public double TotalMatched { get; init; }

    /// <summary>
    /// Gets the total amount of orders that remain unmatched.
    /// </summary>
    [JsonPropertyName("totalAvailable")]
    public double TotalAvailable { get; init; }

    /// <summary>
    /// Gets a value indicating whether cross-matching is enabled for this market.
    /// </summary>
    [JsonPropertyName("crossMatching")]
    public bool CrossMatching { get; init; }

    /// <summary>
    /// Gets a value indicating whether runners in the market can be voided.
    /// Please note - this doesn't include horse racing markets under which bets are voided on non-runners with any applicable reduction factor applied.
    /// </summary>
    [JsonPropertyName("runnersVoidable")]
    public bool RunnersVoidable { get; init; }

    /// <summary>
    /// Gets the version of the market.
    /// The version increments whenever the market status changes, for example, turning in-play, or suspended when a goal is scored.
    /// </summary>
    [JsonPropertyName("version")]
    public long Version { get; init; }

    /// <summary>
    /// Gets the runners in the market.
    /// </summary>
    [JsonPropertyName("runners")]
    public List<Runner>? Runners { get; init; }

    /// <summary>
    /// Gets the key line description for the market.
    /// </summary>
    [JsonPropertyName("keyLineDescription")]
    public KeyLineDescription? KeyLineDescription { get; init; }
}
