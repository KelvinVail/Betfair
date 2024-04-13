namespace Betfair.Api.Responses.Orders;

public class PlaceExecutionReport
{
    public string MarketId { get; set; }

    public string Status { get; set; }

    public string? ErrorCode { get; set; }

    public List<PlaceInstructionReport> InstructionReports { get; set; }
}
