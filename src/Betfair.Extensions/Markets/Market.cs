using Betfair.Core.Login;
using Betfair.Extensions.Contracts;
using Betfair.Extensions.JsonReaders;
using Betfair.Extensions.Markets.Enums;
using Betfair.Stream.Messages;

namespace Betfair.Extensions.Markets;

public sealed class Market : Entity<string>
{
    private readonly Stopwatch _stopwatch = new ();
    private readonly ISubscription _subscription;

    private Market(Credentials credentials, string id, ISubscription? subscription = null)
        : base(id) =>
        _subscription = subscription ?? new MarketSubscription(credentials);

    /// <summary>
    /// Gets or sets an action that is called when the market is updated.
    /// </summary>
    public Action<Market> OnUpdate { get; set; } = _ => { };

    /// <summary>
    /// Gets the time the market is scheduled to start.
    /// </summary>
    public DateTimeOffset StartTime { get; internal set; }

    /// <summary>
    /// Gets the current status of the market.
    /// </summary>
    public MarketStatus Status { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the market is in play.
    /// </summary>
    public bool IsInPlay { get; internal set; }

    /// <summary>
    /// Gets the total amount matched on the market.
    /// </summary>
    public double TradedVolume { get; internal set; }

    /// <summary>
    /// Gets the market version.
    /// </summary>
    public long Version { get; internal set; }

    /// <summary>
    /// Gets the time that Betfair published the last market change (in unix milliseconds).
    /// </summary>
    public long PublishTime { get; internal set; }

    /// <summary>
    /// Gets the time taken in ticks to perform the last update to the market.
    /// </summary>
    public long UpdateLatency => _stopwatch.ElapsedTicks;

    /// <summary>
    /// Gets the runners in the market.
    /// </summary>
    public IReadOnlyList<Runner> Runners => InternalRunners.Values.ToList();

    internal Dictionary<long, Runner> InternalRunners { get; } = [];

    /// <summary>
    /// Create a new market subscription.
    /// </summary>
    /// <param name="credentials">Credentials used to authenticate to Betfair.</param>
    /// <param name="marketId">The market marketId to subscribe to.</param>
    /// <param name="subscription">Optional: Use to stub out the subscription for testing.</param>
    /// <returns>A Market subscription.</returns>
    public static Result<Market> Create(Credentials credentials, string marketId, ISubscription? subscription = null)
    {
        if (credentials == null)
            return Result.Failure<Market>("Credentials must not be empty.");

        var result = MarketIdIsValid(marketId);
        return result.IsFailure
            ? Result.Failure<Market>(result.Error)
            : new Market(credentials, marketId, subscription);
    }

    /// <summary>
    /// Subscribe to the market stream.
    /// </summary>
    /// <param name="filter">Optional: Used to define what data to include in the market stream.
    /// If null only Best Available Prices are returned.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    public async Task Subscribe(DataFilter? filter = null, CancellationToken cancellationToken = default)
    {
        var marketFilter = new StreamMarketFilter().WithMarketIds(Id);
        filter ??= new DataFilter().WithBestPrices();
        
        await _subscription.Subscribe(marketFilter, filter.WithMarketDefinition(), cancellationToken: cancellationToken);
        await _subscription.SubscribeToOrders(cancellationToken: cancellationToken);

        await foreach (var message in _subscription.ReadBytes(cancellationToken))
            Update(message);
    }

    private static Result MarketIdIsValid(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Result.Failure("Market id must not be empty.");

        if (!id.StartsWith("1."))
            return Result.Failure("Market id must start with a '1.' followed by numbers.");

        if (!int.TryParse(id[2..], out _))
            return Result.Failure("Market id must start with a '1.' followed by numbers.");

        return Result.Success();
    }

    private void Update(byte[] message)
    {
        _stopwatch.Restart();
        var reader = new Utf8JsonReader(message);
        
        while (reader.Read())
            this.ReadChangeMessage(ref reader);

        _stopwatch.Stop();

        OnUpdate.Invoke(this);
    }
}
