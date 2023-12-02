namespace Betfair.Core.Login;

#pragma warning disable CA1812
internal sealed class LoginResponse
#pragma warning restore CA1812
{
    public string Token { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Error { get; init; } = string.Empty;

    public string SessionToken { get; init; } = string.Empty;

    public string LoginStatus { get; init; } = string.Empty;
}
