#nullable enable
namespace Betfair.Stream.Responses;

public class StatusMessage
{
    public string Op { get; set; } = string.Empty;

    public int Id { get; set; }

    public string StatusCode { get; set; } = string.Empty;

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public int ConnectionsAvailable { get; set; }
}
