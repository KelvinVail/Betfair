using System.Text;
using System.Text.Json;
using Betfair.Core.Login;
using Betfair.Extensions.Contracts;
using Betfair.Stream.Messages;

namespace Betfair.Extensions.Markets;

public sealed class Market : Entity<string>
{
    private ISubscription _subscription;
    private LazyString _status = string.Empty;
    private readonly Action<Market> _onUpdate;

    private Market(Credentials credentials, string id, Action<Market> onUpdate)
        : base(id)
    {
        _subscription = new MarketSubscription(credentials);
        _onUpdate = onUpdate;
    }

    /// <summary>
    /// Gets the time the market is scheduled to start.
    /// </summary>
    public DateTimeOffset StartTime { get; private set; }

    /// <summary>
    /// Gets the current status of the market.
    /// </summary>
    public LazyString Status => _status;

    /// <summary>
    /// Gets a value indicating whether the market is in play.
    /// </summary>
    public bool IsInPlay { get; private set; }

    /// <summary>
    /// Gets the total amount matched on the market.
    /// </summary>
    public double TotalMatched { get; private set; }

    /// <summary>
    /// Gets the market version.
    /// </summary>
    public long Version { get; private set; }

    /// <summary>
    /// Create a new market subscription.
    /// </summary>
    /// <param name="credentials">Credentials used to authenticate to Betfair.</param>
    /// <param name="marketId">The market marketId to subscribe to.</param>
    /// <param name="onUpdate">An action to perform after each update.</param>
    /// <returns>A Market subscription.</returns>
    public static Result<Market> Create(Credentials credentials, string marketId, Action<Market> onUpdate)
    {
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

    internal void OverrideInternalSubscription(ISubscription subscription) =>
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

    private static bool IsAtKeyValue(ref Utf8JsonReader reader, ReadOnlySpan<byte> key, ReadOnlySpan<byte> value)
    {
        if (reader.TokenType != JsonTokenType.PropertyName) return false;
        if (!reader.ValueSpan.SequenceEqual(key)) return false;
        reader.Read();
        return reader.ValueSpan.SequenceEqual(value);
    }

    private static bool IsAtKey(ref Utf8JsonReader reader, ReadOnlySpan<byte> key) =>
        reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual(key);

    private static bool ValueIs(ref Utf8JsonReader reader, string value) =>
        reader.TokenType == JsonTokenType.String && reader.ValueSpan.SequenceEqual(Encoding.UTF8.GetBytes(value));

    private static void SetString(ref Utf8JsonReader reader, ReadOnlySpan<byte> key, ref LazyString property)
    {
        if (!IsAtKey(ref reader, key)) return;

        reader.Read();
        property = reader.ValueSpan;
        reader.Read();
    }

    private static bool EndOfObject(ref Utf8JsonReader reader, ref int objectCount)
    {
        if (reader.TokenType == JsonTokenType.StartObject) objectCount++;
        if (reader.TokenType == JsonTokenType.EndObject)
        {
            objectCount--;
            if (objectCount == 0) return true;
        }

        return false;
    }

    private void Update(byte[] message)
    {
        var reader = new Utf8JsonReader(message);

        while (reader.Read())
        {
            if (IsAtKeyValue(ref reader, "op"u8, "mcm"u8))
                HandleMarketChangeMessage(ref reader);
        }

        _onUpdate.Invoke(this);
    }

    private void HandleMarketChangeMessage(ref Utf8JsonReader reader)
    {
        while (reader.Read())
        {
            if (IsAtKey(ref reader, "mc"u8))
                HandleMarketChange(ref reader);
        }
    }

    private void HandleMarketChange(ref Utf8JsonReader reader)
    {
        while (reader.Read())
        {
            if (IsAtKey(ref reader, "id"u8))
            {
                reader.Read();
                if (!ValueIs(ref reader, Id)) break;
            }

            HandleMarketDefinition(ref reader);

            SetTotalMatched(ref reader);
        }
    }

    private void HandleMarketDefinition(ref Utf8JsonReader reader)
    {
        if (!IsAtKey(ref reader, "marketDefinition"u8)) return;

        var objectCount = 0;
        while (reader.Read())
        {
            if (EndOfObject(ref reader, ref objectCount)) break;

            SetStartTime(ref reader);
            SetInPlayStatus(ref reader);
            SetString(ref reader, "status"u8, ref _status);
            SetMarketVersion(ref reader);

            if (IsAtKey(ref reader, "runners"u8))
                HandleRunnerDefinitions(ref reader);
        }
    }

    private void SetStartTime(ref Utf8JsonReader reader)
    {
        if (!IsAtKey(ref reader, "marketTime"u8)) return;

        reader.Read();
        StartTime = reader.GetDateTimeOffset();
        reader.Read();
    }

    private void SetInPlayStatus(ref Utf8JsonReader reader)
    {
        if (!IsAtKey(ref reader, "inPlay"u8)) return;

        reader.Read();
        IsInPlay = reader.GetBoolean();
        reader.Read();
    }

    private void SetTotalMatched(ref Utf8JsonReader reader)
    {
        if (!IsAtKey(ref reader, "tv"u8)) return;

        reader.Read();
        TotalMatched = reader.GetDouble();
        reader.Read();
    }

    private void SetMarketVersion(ref Utf8JsonReader reader)
    {
        if (!IsAtKey(ref reader, "version"u8)) return;

        reader.Read();
        Version = reader.GetInt64();
        reader.Read();
    }

    private void HandleRunnerDefinitions(ref Utf8JsonReader reader)
    {
        while (reader.Read())
        {
            if (IsAtKey(ref reader, "runners"u8))
                HandleRunners(ref reader);

            if (reader.TokenType == JsonTokenType.EndArray)
                break;
        }
    }

    private void HandleRunners(ref Utf8JsonReader reader)
    {
        while (reader.Read())
        {
            if (IsAtKey(ref reader, "id"u8))
            {
                reader.Read();
                var runnerId = reader.GetInt64();
                var runner = Runner.Create(runnerId);
                // if (runner.IsSuccess)
                // {
                //     var r = runner.Value;
                //     r.IsActive = true;
                //     r.AdjustmentFactor = 1.0;
                //     r.TotalMatched = 0.0;
                // }
            }
        }
    }
}
