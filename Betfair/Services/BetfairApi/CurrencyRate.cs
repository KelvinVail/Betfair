namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class CurrencyRate
    {
        [JsonProperty(PropertyName = "currencyCode")]
        public string CurrencyCode { get; set; }

        [JsonProperty(PropertyName = "rate")]
        public double Rate { get; set; }
    }
}