namespace Betfair.Api.Requests;

public class MarketProjection : List<string>
{
    public MarketProjection WithMarketStartTime()
    {
        AddUnique("MARKET_START_TIME");
        return this;
    }

    public MarketProjection WithCompetition()
    {
        AddUnique("COMPETITION");
        return this;
    }

    public MarketProjection WithEvent()
    {
        AddUnique("EVENT");
        return this;
    }

    public MarketProjection WithEventType()
    {
        AddUnique("EVENT_TYPE");
        return this;
    }

    public MarketProjection WithMarketDescription()
    {
        AddUnique("MARKET_DESCRIPTION");
        return this;
    }

    public MarketProjection WithRunnerDescription()
    {
        AddUnique("RUNNER_DESCRIPTION");
        return this;
    }

    public MarketProjection WithRunnerMetadata()
    {
        AddUnique("RUNNER_METADATA");
        return this;
    }

    private void AddUnique(string value)
    {
        if (!Contains(value)) Add(value);
    }
}
