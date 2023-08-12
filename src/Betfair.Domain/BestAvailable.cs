namespace Betfair.Domain;

public class BestAvailable
{
    private BestAvailable(List<List<double>> available)
    {
        for (int i = 0; i < available.Count; i++)
        {
            if (available[i][0] == 0)
            {
                BestPrice = Price.Of(available[i][1]);
                BestSize = available[i][2];
            }

            TotalSize += available[i][2];
        }
    }

    public Price BestPrice { get; }

    public double BestSize { get; }

    public double TotalSize { get; }

    public static Maybe<BestAvailable> Create(List<List<double>>? available)
    {
        if (available == null) return Maybe<BestAvailable>.None;
        return new BestAvailable(available);
    }
}
