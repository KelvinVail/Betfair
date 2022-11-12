namespace Betfair.Login;

public sealed class Response
{
    public string Token { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Error { get; init; } = string.Empty;
}
