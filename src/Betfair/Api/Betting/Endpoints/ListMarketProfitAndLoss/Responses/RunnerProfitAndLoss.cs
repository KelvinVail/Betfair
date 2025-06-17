namespace Betfair.Api.Betting.Endpoints.ListMarketProfitAndLoss.Responses;

public class RunnerProfitAndLoss
{
    /// <summary>
    /// Gets the unique identifier for the selection.
    /// </summary>
    public long? SelectionId { get; init; }

    /// <summary>
    /// Gets profit or loss for the market if this selection is the winner.
    /// </summary>
    public double? IfWin { get; init; }

    /// <summary>
    /// Gets profit or loss for the market if this selection is the loser. Only returned for multi-winner odds markets.
    /// </summary>
    public double? IfLose { get; init; }

    /// <summary>
    /// Gets profit or loss for the market if this selection is placed. Applies to marketType EACH_WAY only.
    /// </summary>
    public double? IfPlace { get; init; }
}
