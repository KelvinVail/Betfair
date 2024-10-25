using Betfair.Core.Login;
using Betfair.Stream;
using Betfair.Stream.Messages;

namespace Betfair.Extensions.Markets;

public sealed class Market : Entity<string>, IDisposable
{
    private Subscription _subscription;
    private Action<Market> _onUpdate;

    private Market(Credentials credentials, string id, Action<Market> onUpdate)
        : base(id)
    {
        _subscription = new Subscription(credentials);
        _onUpdate = onUpdate;
    }

    /// <summary>
    /// Create a new market subscription.
    /// </summary>
    /// <param name="credentials">Credentials used to authenticate to Betfair.</param>
    /// <param name="marketId">The market marketId to subscribe to.</param>
    /// <param name="onUpdate">An action to perform after each update.</param>
    /// <returns>A Market subscription.</returns>
    public static Result<Market> Create(Credentials credentials, string marketId, Action<Market> onUpdate)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (credentials == null)
            return Result.Failure<Market>("Credentials must not be empty.");

        var result = MarketIdIsValid(marketId);
        return result.IsFailure
            ? Result.Failure<Market>(result.Error)
            : new Market(credentials, marketId, onUpdate);
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
        {
            Update(message);
        }
    }

    public void Dispose() => _subscription.Dispose();

    internal void OverrideInternalSubscription(Subscription subscription) =>
        _subscription = subscription;

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
        _onUpdate.Invoke(this);
    }
}
