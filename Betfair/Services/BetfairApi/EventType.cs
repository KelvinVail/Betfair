namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    /// <summary>
    /// The event type.
    /// </summary>
    public class EventType
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}