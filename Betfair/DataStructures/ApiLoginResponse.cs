namespace Betfair.DataStructures
{
    using Newtonsoft.Json;

    internal class ApiLoginResponse
    {
        [JsonProperty]
        internal string Token { get; set; }

        [JsonProperty]
        internal string Product { get; set; }

        [JsonProperty]
        internal string Status { get; set; }

        [JsonProperty]
        internal string Error { get; set; }
    }
}
