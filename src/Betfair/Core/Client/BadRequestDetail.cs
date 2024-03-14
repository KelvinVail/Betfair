namespace Betfair.Core.Client;

[JsonSerializable(typeof(BadRequestDetail))]
internal class BadRequestDetail
{
    [JsonPropertyName("apiNgException")]
    public BadRequestErrorCode ApiNgException { get; set; } = new ();
}
