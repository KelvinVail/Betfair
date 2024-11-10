using Betfair.Extensions.Markets.Enums;

namespace Betfair.Extensions.Markets;

public class Runner : Entity<long>
{
    private Runner(long id, RunnerStatus status, double adjustmentFactor)
        : base(id)
    {
        Status = status;
        AdjustmentFactor = adjustmentFactor;
    }

    public RunnerStatus Status { get; internal set; }

    public double AdjustmentFactor { get; internal set; }

    public LevelLadder BestAvailableToBack { get; } = new ();

    public LevelLadder BestAvailableToLay { get; } = new ();

    internal static Runner Create(long id, RunnerStatus status, double adjustmentFactor) =>
        new (id, status, adjustmentFactor);
}
