using Betfair.Core.Login;
using Betfair.Extensions.ByteReaders;
using Betfair.Extensions.Contracts;
using Betfair.Extensions.Markets.Enums;
using Betfair.Stream.Messages;

namespace Betfair.Extensions.Markets;

public sealed class MarketCache : Entity<string>
{
    private readonly Stopwatch _stopwatch = new ();
    private readonly ISubscription _subscription;
    private readonly Dictionary<long, RunnerCache> _runners = [];

    private MarketCache(Credentials credentials, string id, ISubscription? subscription = null)
        : base(id)
    {
        _subscription = subscription ?? new MarketSubscription(credentials);
        MarketIdUtf8 = Encoding.UTF8.GetBytes(id);
    }

    /// <summary>
    /// Gets or sets an action that is called when the market is updated.
    /// </summary>
    public Action<MarketCache> OnUpdate { get; set; } = _ => { };

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
    public IReadOnlyCollection<RunnerCache> Runners => _runners.Values;

    internal byte[] MarketIdUtf8 { get; private set; }

    /// <summary>
    /// Create a new market subscription.
    /// </summary>
    /// <param name="credentials">Credentials used to authenticate to Betfair.</param>
    /// <param name="marketId">The market marketId to subscribe to.</param>
    /// <param name="subscription">Optional: Use to stub out the subscription for testing.</param>
    /// <returns>A Market subscription.</returns>
    public static Result<MarketCache> Create(Credentials credentials, string marketId, ISubscription? subscription = null)
    {
        if (credentials == null)
            return Result.Failure<MarketCache>("Credentials must not be empty.");

        var result = MarketIdIsValid(marketId);
        return result.IsFailure
            ? Result.Failure<MarketCache>(result.Error)
            : new MarketCache(credentials, marketId, subscription);
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

    internal void AddOrUpdateRunnerDefinition(long id, RunnerStatus status, double adjustmentFactor)
    {
        if (status == RunnerStatus.Removed)
        {
            _runners.Remove(id);
            return;
        }

        if (_runners.TryGetValue(id, out var existingRunner))
        {
            existingRunner.Status = status;
            existingRunner.AdjustmentFactor = adjustmentFactor;
            return;
        }

        var runner = RunnerCache.Create(id, status, adjustmentFactor);
        _runners[id] = runner;
    }

    internal void UpdateBestAvailableToBack(long id, int level, double price, double size) =>
        _runners[id].BestAvailableToBack.Update(level, price, size);

    internal void UpdateBestAvailableToLay(long id, int level, double price, double size) =>
        _runners[id].BestAvailableToLay.Update(level, price, size);

    internal void UpdateTradedLadder(long id, double price, double size) =>
        _runners[id].TradedLadder.Update(price, size);

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
        var reader = new BetfairJsonReader(message);

        while (reader.Read())
            this.ReadChangeMessage(ref reader);

        _stopwatch.Stop();

        OnUpdate.Invoke(this);
    }
}
