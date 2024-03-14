namespace Betfair.Core.Client;

[JsonSerializable(typeof(BadRequestErrorCode))]
internal class BadRequestErrorCode
{
    [JsonPropertyName("errorDetails")]
    public string? ErrorCode { get; set; }
}
