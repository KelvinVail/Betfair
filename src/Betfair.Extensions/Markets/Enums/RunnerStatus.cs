namespace Betfair.Extensions.Markets.Enums;

public enum RunnerStatus
{
    Active,

    Winner,

    Loser,

    /// <summary>
    /// The runner was placed, applies to EACH_WAY marketTypes only. 
    /// </summary>
    Placed,

    Removed,

    /// <summary>
    /// The selection is hidden from the market.  This occurs in Horse Racing markets where runners are hidden when it is does not hold an official entry following an entry stage. This could be because the horse was never entered or because they have been scratched from a race at a declaration stage. All matched customer bet prices are set to 1.0 even if there are later supplementary stages. Should it appear likely that a specific runner may actually be supplemented into the race this runner will be reinstated with all matched customer bets set back to the original prices.
    /// </summary>
    Hidden,
}