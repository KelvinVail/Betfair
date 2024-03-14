namespace Betfair.Core.Client;

internal class BadRequestErrorCode
{
    [JsonPropertyName("errorDetails")]
    public string? ErrorCode { get; set; }
}
