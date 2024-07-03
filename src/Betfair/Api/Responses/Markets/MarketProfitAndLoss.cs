namespace Betfair.Api.Responses.Markets;

public class MarketProfitAndLoss
{
    /// <summary>
    /// Gets the unique identifier for the market.
    /// </summary>
    public string? MarketId { get; internal set; }

    /// <summary>
    /// Gets the commission rate applied to Profit and Loss values. Only returned if Net Of Commission option is requested.
    /// </summary>
    public double? CommissionApplied { get; internal set; }

    /// <summary>
    /// Gets calculated profit and loss data.
    /// </summary>
    public IEnumerable<RunnerProfitAndLoss>? ProfitAndLosses { get; internal set; }
}
