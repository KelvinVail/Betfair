namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The roll up model.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RollUpModel
    {
        /// <summary>
        /// The stake.
        /// </summary>
        STAKE,

        /// <summary>
        /// The payout.
        /// </summary>
        PAYOUT,

        /// <summary>
        /// The managed liability.
        /// </summary>
        MANAGED_LIABILITY,

        /// <summary>
        /// The none.
        /// </summary>
        NONE
    }
}