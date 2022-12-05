namespace Betfair.Betting.Models;

public sealed class Runner
{
    public long SelectionId { get; init; }

    public string RunnerName { get; init; } = string.Empty;

    public decimal Handicap { get; init; }

    public int SortPriority { get; init; }

    public Dictionary<string, string>? Metadata { get; init; }
}
