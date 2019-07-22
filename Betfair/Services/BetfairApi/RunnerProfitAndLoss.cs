namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class RunnerProfitAndLoss
    {
        [JsonProperty(PropertyName = "selectionId")]
        public long SelectionId { get; set; }

        [JsonProperty(PropertyName = "ifWin")]
        public double IfWin { get; set; }

        [JsonProperty(PropertyName = "ifLose")]
        public double IfLose { get; set; }
    }
}