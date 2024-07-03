namespace Betfair.Api.Requests.Markets;

internal class MarketProfitAndLossRequest
{
    public IEnumerable<string>? MarketIds { get; set; }

    public bool? IncludeSettledBets { get; set; }

    public bool? IncludeBspBets { get; set; }

    public bool? NetOfCommission { get; set; }
}
