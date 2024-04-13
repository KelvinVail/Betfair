namespace Betfair.Api.Requests.Orders;

public class PlaceInstruction
{
    public long SelectionId { get; set; }

    public string Side { get; set; } = "BACK";

    public string OrderType { get; set; } = "LIMIT";

    public LimitOrder? LimitOrder { get; set; }

    public double? Handicap { get; set; }

    public string? CustomerOrderRef { get; set; }
}
