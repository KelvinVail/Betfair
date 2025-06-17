namespace Betfair.Api.Betting.Endpoints.ListCompetitions.Requests;

internal class CompetitionsRequest
{
    [JsonPropertyName("filter")]
    public ApiMarketFilter Filter { get; set; } = new ();

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }
}