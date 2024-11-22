using Betfair.Extensions.Markets.Enums;

namespace Betfair.Extensions.Markets;

public class RunnerCache : Entity<long>
{
    private RunnerCache(long id, RunnerStatus status, double adjustmentFactor)
        : base(id)
    {
        Status = status;
        AdjustmentFactor = adjustmentFactor;
    }

    public RunnerStatus Status { get; internal set; }

    public double AdjustmentFactor { get; internal set; }

    public LevelLadderDictionary BestAvailableToBack { get; } = new ();

    public LevelLadderDictionary BestAvailableToLay { get; } = new ();

    internal static RunnerCache Create(long id, RunnerStatus status, double adjustmentFactor) =>
        new (id, status, adjustmentFactor);
}
