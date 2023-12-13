namespace Betfair.Api.Requests;

public class MarketProjection
{
    private MarketProjection(string value) =>
        Value = value;

    public static MarketProjection MarketStartTime =>
        new ("MARKET_START_TIME");

    public static MarketProjection Competition =>
        new ("COMPETITION");

    public static MarketProjection Event =>
        new ("EVENT");

    public static MarketProjection EventType =>
        new ("EVENT_TYPE");

    public static MarketProjection MarketDescription =>
        new ("MARKET_DESCRIPTION");

    public static MarketProjection RunnerDescription =>
        new ("RUNNER_DESCRIPTION");

    public static MarketProjection RunnerMetadata =>
        new ("RUNNER_METADATA");

    public string Value { get; }
}
