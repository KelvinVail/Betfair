namespace Betfair.Api.Responses.Markets;

public class RunnerProfitAndLoss
{
    /// <summary>
    /// Gets the unique identifier for the selection.
    /// </summary>
    public long? SelectionId { get; internal set; }

    /// <summary>
    /// Gets profit or loss for the market if this selection is the winner.
    /// </summary>
    public double? IfWin { get; internal set; }

    /// <summary>
    /// Gets profit or loss for the market if this selection is the loser. Only returned for multi-winner odds markets.
    /// </summary>
    public double? IfLose { get; internal set; }

    /// <summary>
    /// Gets profit or loss for the market if this selection is placed. Applies to marketType EACH_WAY only.
    /// </summary>
    public double? IfPlace { get; internal set; }
}
