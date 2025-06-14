namespace Betfair.Core.Enums;

/// <summary>
/// Market projection options for listMarketCatalogue.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<MarketProjection>))]
public enum MarketProjection
{
    /// <summary>
    /// If not selected then the competition will not be returned with marketCatalogue.
    /// </summary>
    Competition,

    /// <summary>
    /// If not selected then the event will not be returned with marketCatalogue.
    /// </summary>
    Event,

    /// <summary>
    /// If not selected then the eventType will not be returned with marketCatalogue.
    /// </summary>
    EventType,

    /// <summary>
    /// If not selected then the market description will not be returned with marketCatalogue.
    /// </summary>
    MarketDescription,

    /// <summary>
    /// If not selected then the runner description will not be returned with marketCatalogue.
    /// </summary>
    RunnerDescription,

    /// <summary>
    /// If not selected then the runner metadata will not be returned with marketCatalogue.
    /// Note that the runner metadata is only available for certain markets.
    /// </summary>
    RunnerMetadata,
}
