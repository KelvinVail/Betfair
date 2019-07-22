namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The order projection.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderProjection
    {
        /// <summary>
        /// The none.
        /// </summary>
        NONE,

        /// <summary>
        /// The all.
        /// </summary>
        ALL,

        /// <summary>
        /// The executable.
        /// </summary>
        EXECUTABLE,

        /// <summary>
        /// The execution complete.
        /// </summary>
        EXECUTION_COMPLETE
    }
}