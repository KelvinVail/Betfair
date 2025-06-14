namespace Betfair.Api.Responses;

public class DateRangeResult
{
    [JsonPropertyName("from")]
    public string? From { get; init; }

    [JsonPropertyName("to")]
    public string? To { get; init; }
}
