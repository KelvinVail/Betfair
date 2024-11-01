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

    public RunnerStatus Status { get; }

    public double AdjustmentFactor { get; }


    internal static Runner Create(long id, RunnerStatus status, double adjustmentFactor) =>
        new (id, status, adjustmentFactor);
}
