namespace Betfair.Api.Betting;

public class DateRange
{
    [JsonPropertyName("from")]
    public string? From { get; internal set; }

    [JsonPropertyName("to")]
    public string? To { get; internal set; }
}
