namespace Betfair.Core.Login;

[JsonSerializable(typeof(LoginResponse))]
internal class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;

    [JsonPropertyName("sessionToken")]
    public string SessionToken { get; set; } = string.Empty;

    [JsonPropertyName("loginStatus")]
    public string LoginStatus { get; set; } = string.Empty;
}
