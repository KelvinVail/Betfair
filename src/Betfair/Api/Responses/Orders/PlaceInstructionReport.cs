using Betfair.Api.Requests.Orders;

namespace Betfair.Api.Responses.Orders;

public class PlaceInstructionReport
{
    public string Status { get; set; }

    public string? ErrorCode { get; set; }

    public string? BetId { get; set; }

    public DateTimeOffset? PlacedDate { get; set; }

    public double? AveragePriceMatched { get; set; }

    public double? SizeMatched { get; set; }

    public string? OrderStatus { get; set; }

    public PlaceInstruction Instruction { get; set; }
}
