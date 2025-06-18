namespace Betfair.Core.Client;

internal class BetfairApiNgError
{
    [JsonPropertyName("requestUUID")]
    public string? RequestUUID { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("errorDetails")]
    public string? ErrorDetails { get; set; }
}
