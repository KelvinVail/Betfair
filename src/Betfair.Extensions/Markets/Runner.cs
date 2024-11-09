using Betfair.Extensions.Markets.Enums;

namespace Betfair.Extensions.Markets;

public class Runner : Entity<long>
{
    private readonly BestAvailable _bestAvailableToBack = new ();

    private Runner(long id, RunnerStatus status, double adjustmentFactor)
        : base(id)
    {
        Status = status;
        AdjustmentFactor = adjustmentFactor;
    }

    public RunnerStatus Status { get; internal set; }

    public double AdjustmentFactor { get; internal set; }

    public Price BestAvailableToBackPrice(int level) =>
        _bestAvailableToBack.PriceAt(level);

    internal static Runner Create(long id, RunnerStatus status, double adjustmentFactor) =>
        new (id, status, adjustmentFactor);

    internal void UpdateBestAvailableToBack(int level, double price, double size) =>
        _bestAvailableToBack.Update(level, price, size);
}
