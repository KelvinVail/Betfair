namespace Betfair.Login;

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Error { get; init; } = string.Empty;

    public string SessionToken { get; init; } = string.Empty;

    public string LoginStatus { get; init; } = string.Empty;
}
