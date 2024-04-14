namespace Betfair.Api.Requests.OrderDtos;

public class LimitOrder
{
    public double? Size { get; set; }

    public double Price { get; set; }

    public string PersistenceType { get; set; } = "LAPSE";

    public string? TimeInForce { get; set; }

    public double? MinFillSize { get; set; }

    public double? BetTargetSize { get; set; }

    public string? BetTargetType { get; set; }
}
