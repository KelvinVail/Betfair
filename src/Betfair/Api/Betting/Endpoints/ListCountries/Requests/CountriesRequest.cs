namespace Betfair.Api.Betting.Endpoints.ListCountries.Requests;

internal class CountriesRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new ();

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }
}
