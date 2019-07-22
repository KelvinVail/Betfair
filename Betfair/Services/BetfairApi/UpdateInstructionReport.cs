namespace Betfair.Services.BetfairApi
{
    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    public class UpdateInstructionReport
    {
        [JsonProperty(PropertyName = "status")]
        public InstructionReportStatus Status { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        public InstructionReportErrorCode ErrorCode { get; set; }

        [JsonProperty(PropertyName = "instruction")]
        public UpdateInstruction Instruction { get; set; }
    }
}