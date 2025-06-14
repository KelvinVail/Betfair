namespace Betfair.Api.Responses.Markets;

public class ExchangePrices
{
    public List<List<double>>? AvailableToBack { get; init; }

    public List<List<double>>? AvailableToLay { get; init; }

    public List<List<double>>? TradedVolume { get; init; }
}
