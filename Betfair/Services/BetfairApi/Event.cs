namespace Betfair.Services.BetfairApi
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// The event.
    /// </summary>
    public class Event
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

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; }

        /// <summary>
        /// Gets or sets the venue.
        /// </summary>
        [JsonProperty(PropertyName = "venue")]
        public string Venue { get; set; }

        /// <summary>
        /// Gets or sets the open date.
        /// </summary>
        [JsonProperty(PropertyName = "openDate")]
        public DateTime? OpenDate { get; set; }
    }
}