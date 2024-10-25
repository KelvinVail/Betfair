namespace Betfair.Api.Responses.Markets;

public class ExchangePrices
{
    public List<List<double>>? AvailableToBack { get; internal set; }

    public List<List<double>>? AvailableToLay { get; internal set; }

    public List<List<double>>? TradedVolume { get; internal set; }
}
