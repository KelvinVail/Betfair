namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The runner catalog.
    /// </summary>
    public class RunnerCatalog
    {
        /// <summary>
        /// Gets or sets the selection id.
        /// </summary>
        [JsonProperty(PropertyName = "selectionId")]
        public long SelectionId { get; set; }

        /// <summary>
        /// Gets or sets the runner name.
        /// </summary>
        [JsonProperty(PropertyName = "runnerName")]
        public string RunnerName { get; set; }

        /// <summary>
        /// Gets or sets the handicap.
        /// </summary>
        [JsonProperty(PropertyName = "handicap")]
        public double Handicap { get; set; }

        /// <summary>
        /// Gets or sets the sort priority.
        /// </summary>
        [JsonProperty(PropertyName = "sortPriority")]
        public int SortPriority { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }
}