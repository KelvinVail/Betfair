namespace Betfair.Stream.Messages;

public class Authentication
{
    public string Op { get; } = "authentication";

    public int Id { get; init; }

    public string Session { get; init; } = string.Empty;

    public string AppKey { get; init; } = string.Empty;
}
