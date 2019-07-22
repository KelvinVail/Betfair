namespace Betfair.Services.BetfairApi
{
    using Betfair.Services.BetfairApi.Enums;
    using Betfair.Services.BetfairApi.Orders.PlaceOrders.Response;

    using Newtonsoft.Json;

    internal class ReplaceInstructionReport
    {
        [JsonProperty(PropertyName = "status")]
        internal InstructionReportStatus Status { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        internal InstructionReportErrorCode ErrorCode { get; set; }

        [JsonProperty(PropertyName = "cancelInstructionReport")]
        internal CancelInstructionReport CancelInstructionReport { get; set; }

        [JsonProperty(PropertyName = "placeInstructionReport")]
        internal PlaceInstructionReport PlaceInstructionReport { get; set; }
    }
}