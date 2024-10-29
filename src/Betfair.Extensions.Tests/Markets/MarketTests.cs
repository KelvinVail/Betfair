using Betfair.Core.Login;
using Betfair.Extensions.Markets;
using Betfair.Stream.Messages;

namespace Betfair.Extensions.Tests.Markets;

public class MarketTests
{
    private readonly Credentials _credentials = new ("username", "password", "appKey");
    private readonly SubscriptionStub _subscription = new ();
    private Action<Market> _onUpdate = _ => { };

    [Fact]
    public void CredentialMustNotBeNull()
    {
        var result = Market.Create(null!, "1.1", _onUpdate);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Credentials must not be empty.");
    }

    [Fact]
    public void MarketIdMustNotBeNull()
    {
        var result = Market.Create(_credentials, null!, _onUpdate);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must not be empty.");
    }

    [Fact]
    public void MarketIdMustNotBeEmpty()
    {
        var result = Market.Create(_credentials, string.Empty, _onUpdate);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must not be empty.");
    }

    [Fact]
    public void MarketIdMustNotBeWhiteSpace()
    {
        var result = Market.Create(_credentials, " ", _onUpdate);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must not be empty.");
    }

    [Fact]
    public void MarketIdMustStartWithAOneThenADotFollowedByNumbers()
    {
        var result = Market.Create(_credentials, "1.1", _onUpdate);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("1.1");
    }

    [Fact]
    public void MarketIdMustStartWithAOneThenADotFollowedByNumbersAndNotLetters()
    {
        var result = Market.Create(_credentials, "1.a", _onUpdate);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must start with a '1.' followed by numbers.");
    }

    [Theory]
    [InlineData("1.12345")]
    [InlineData("1.98765")]
    public async Task SubscribeToMarketUseTheIsOfTheMarketInTheSubscriptionMarketFilter(string marketId)
    {
        var market = Market.Create(_credentials, marketId, _onUpdate).Value;
        market.OverrideInternalSubscription(_subscription);

        await market.Subscribe();

        _subscription.SubscribedCalled.Should().BeTrue();
        _subscription.MarketFilter!.MarketIds.Should().Contain(marketId);
    }

    [Fact]
    public async Task IfDataFilterIsNullBestAvailablePricesAndMarketDefinitionIsRequested()
    {
        var market = Market.Create(_credentials, "1.1", _onUpdate).Value;
        market.OverrideInternalSubscription(_subscription);

        await market.Subscribe();

        var dataFilter = new DataFilter().WithBestPrices().WithMarketDefinition();
        _subscription.DataFilter!.Should().BeEquivalentTo(dataFilter);
    }

    [Fact]
    public async Task MarketDefinitionIsAddedToTheDataFilterIfItIsNotPresent()
    {
        var market = Market.Create(_credentials, "1.1", _onUpdate).Value;
        market.OverrideInternalSubscription(_subscription);

        await market.Subscribe(new DataFilter().WithFullTradedLadder());

        var dataFilter = new DataFilter().WithFullTradedLadder().WithMarketDefinition();
        _subscription.DataFilter!.Should().BeEquivalentTo(dataFilter);
    }

    [Fact]
    public async Task SubscribeToOrdersIsCalled()
    {
        var market = Market.Create(_credentials, "1.1", _onUpdate).Value;
        market.OverrideInternalSubscription(_subscription);

        await market.Subscribe();

        _subscription.SubscribedToOrdersCalled.Should().BeTrue();
    }

    [Fact]
    public async Task MarketStartTimeIsSetFromTheInitialMarketDefinition()
    {
        var market = Market.Create(_credentials, "1.235123059", _onUpdate).Value;
        var sub = new SubscriptionStub(Path.Combine("Data", "MarketDefinitions", "InitialImage.json"));
        market.OverrideInternalSubscription(sub);

        await market.Subscribe();

        market.StartTime.Should().Be(new DateTime(2024, 10, 29, 12, 40, 0, DateTimeKind.Utc));
    }

    [Fact]
    public async Task DoNotSetTheStartTimeIfMarketIdsDoNotMatch()
    {
        var market = Market.Create(_credentials, "1.1", _onUpdate).Value;
        var sub = new SubscriptionStub(Path.Combine("Data", "MarketDefinitions", "InitialImage.json"));
        market.OverrideInternalSubscription(sub);

        await market.Subscribe();

        market.StartTime.Should().Be(default);
    }
}
