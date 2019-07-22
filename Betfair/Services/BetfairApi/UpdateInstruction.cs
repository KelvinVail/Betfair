namespace Betfair.Services.BetfairApi
{
    using Betfair.Services.BetfairApi.Enums;

    using Newtonsoft.Json;

    public class UpdateInstruction
    {
        [JsonProperty(PropertyName = "betId")]
        public string BetId { get; set; }

        [JsonProperty(PropertyName = "newPersistenceType")]
        public PersistenceType NewPersistenceType { get; set; }
    }
}