using Betfair.Errors;
using Betfair.Login;
using Betfair.Stream;
using Betfair.Stream.Responses;
using Betfair.Tests.Errors;
using Betfair.Tests.Stream.TestDoubles;

namespace Betfair.Tests.Stream;

public class SubscribeTests
{
    private readonly PipelineSpy _pipe;
    private readonly Credentials _credentials = Credentials.Create("username", "password", "appKey").Value;
    private readonly Subscription _subscription;
    private readonly List<ChangeMessage> _messages = new ();

    public SubscribeTests()
    {
        _pipe = new PipelineSpy();
        _subscription = new Subscription(_pipe, _credentials);
    }

    [Fact]
    public async Task MarketFilterMustNotBeNull()
    {
        await _subscription.Subscribe(null, new MarketDataFilter());

        _subscription.Status.ShouldBeFailure(
            ErrorResult.Empty("MarketFilter"));
        _pipe.WrittenObjects.Should().NotContain(
            x => x.GetType() == typeof(SubscriptionMessage));
    }

    [Fact]
    public async Task DataFilterMustNotBeNull()
    {
        await _subscription.Subscribe(new StreamMarketFilter(), null);

        _subscription.Status.ShouldBeFailure(
            ErrorResult.Empty("DataFilter"));
        _pipe.WrittenObjects.Should().NotContain(
            x => x.GetType() == typeof(SubscriptionMessage));
    }

    [Fact]
    public async Task ReturnErrorIfConnectionFailed()
    {
        _pipe.Responses.Add(new ChangeMessage
        {
            Operation = "connection",
            StatusCode = "FAILURE",
            ErrorCode = "NO_APP_KEY",
        });
        await _subscription.Authenticate("token");
        await foreach (var line in _subscription.GetChanges())
            _messages.Add(line);

        await _subscription.Subscribe(new StreamMarketFilter(), new MarketDataFilter());

        _subscription.Status.ShouldBeFailure(ErrorResult.Create("NO_APP_KEY"));
        _pipe.WrittenObjects.Should().NotContain(
            x => x.GetType() == typeof(SubscriptionMessage));
    }

    [Fact]
    public async Task ShouldWriteASubscriptionMessageToStream()
    {
        await _subscription.Subscribe(new StreamMarketFilter(), new MarketDataFilter());

        var expected = new SubscriptionMessage
        {
            Op = "marketSubscription",
            Id = 1,
            MarketFilter = new StreamMarketFilter(),
            MarketDataFilter = new MarketDataFilter(),
        };
        _pipe.WrittenObjects.Should().ContainEquivalentOf(expected);
    }

    [Theory]
    [InlineData("1.2345")]
    [InlineData("1.98765")]
    public async Task ShouldWriteMarketFilterToStream(string marketId)
    {
        var marketFilter = new StreamMarketFilter().WithMarketId(marketId);
        await _subscription.Subscribe(marketFilter, new MarketDataFilter());

        var expected = new SubscriptionMessage
        {
            Op = "marketSubscription",
            Id = 1,
            MarketFilter = marketFilter,
            MarketDataFilter = new MarketDataFilter(),
        };
        _pipe.WrittenObjects.Should().ContainEquivalentOf(expected);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task ShouldWriteDateFilterToStream(int levels)
    {
        var dataFilter = new MarketDataFilter().WithLadderLevels(levels);
        await _subscription.Subscribe(new StreamMarketFilter(), dataFilter);

        var expected = new SubscriptionMessage
        {
            Op = "marketSubscription",
            Id = 1,
            MarketFilter = new StreamMarketFilter(),
            MarketDataFilter = dataFilter,
        };
        _pipe.WrittenObjects.Should().ContainEquivalentOf(expected);
    }
}
