using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;

/// <summary>
/// Runner status values.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<RunnerStatus>))]
public enum RunnerStatus
{
    /// <summary>
    /// Active runner.
    /// </summary>
    Active,

    /// <summary>
    /// Winner.
    /// </summary>
    Winner,

    /// <summary>
    /// Loser.
    /// </summary>
    Loser,

    /// <summary>
    /// The runner was placed, applies to EACH_WAY marketTypes only.
    /// </summary>
    Placed,

    /// <summary>
    /// Removed from market.
    /// </summary>
    Removed,

    /// <summary>
    /// The selection is hidden from the market.
    /// This occurs in Horse Racing markets where runners are hidden when they do not hold an official entry following an entry stage.
    /// This could be because the horse was never entered or because they have been scratched from a race at a declaration stage.
    /// All matched customer bet prices are set to 1.0 even if there are later supplementary stages.
    /// Should it appear likely that a specific runner may actually be supplemented into the race this runner will be reinstated with all matched customer bets set back to the original prices.
    /// </summary>
    Hidden,
}
