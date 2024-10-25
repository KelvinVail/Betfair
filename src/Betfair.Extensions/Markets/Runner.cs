namespace Betfair.Extensions.Markets;

public class Runner : Entity<long>
{
    private Runner(long id)
        : base(id)
    {
    }

    public bool IsActive { get; private set; }

    public double AdjustmentFactor { get; private set; }

    public double TotalMatched { get; private set; }

    internal static Result<Runner> Create(long id)
    {
        return id > 0
            ? Result.Success(new Runner(id))
            : Result.Failure<Runner>("Runner id must be greater than zero.");
    }
}
