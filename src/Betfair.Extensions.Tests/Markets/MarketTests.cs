﻿using Betfair.Core.Login;
using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;
using Betfair.Stream.Messages;

namespace Betfair.Extensions.Tests.Markets;

public class MarketTests
{
    private readonly Credentials _credentials = new ("username", "password", "appKey");
    private readonly SubscriptionMock _subscription = new ();

    [Fact]
    public void CredentialMustNotBeNull()
    {
        var result = Market.Create(null!, "1.1", _subscription);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Credentials must not be empty.");
    }

    [Fact]
    public void MarketIdMustNotBeNull()
    {
        var result = Market.Create(_credentials, null!, _subscription);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must not be empty.");
    }

    [Fact]
    public void MarketIdMustNotBeEmpty()
    {
        var result = Market.Create(_credentials, string.Empty, _subscription);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must not be empty.");
    }

    [Fact]
    public void MarketIdMustNotBeWhiteSpace()
    {
        var result = Market.Create(_credentials, " ", _subscription);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must not be empty.");
    }

    [Fact]
    public void MarketIdMustStartWithAOneThenADotFollowedByNumbers()
    {
        var result = Market.Create(_credentials, "2.1", _subscription);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must start with a '1.' followed by numbers.");
    }

    [Fact]
    public void MarketIdMustStartWithAOneThenADotFollowedByNumbersAndNotLetters()
    {
        var result = Market.Create(_credentials, "1.a", _subscription);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Market id must start with a '1.' followed by numbers.");
    }

    [Theory]
    [InlineData("1.12345")]
    [InlineData("1.98765")]
    public async Task SubscribeToMarketUseTheIsOfTheMarketInTheSubscriptionMarketFilter(string marketId)
    {
        var market = Market.Create(_credentials, marketId, _subscription).Value;

        await market.Subscribe();

        _subscription.SubscribedCalled.Should().BeTrue();
        _subscription.MarketFilter!.MarketIds.Should().Contain(marketId);
    }

    [Fact]
    public async Task IfDataFilterIsNullBestAvailablePricesAndMarketDefinitionIsRequested()
    {
        var market = Market.Create(_credentials, "1.1", _subscription).Value;

        await market.Subscribe();

        var dataFilter = new DataFilter().WithBestPrices().WithMarketDefinition();
        _subscription.DataFilter!.Should().BeEquivalentTo(dataFilter);
    }

    [Fact]
    public async Task MarketDefinitionIsAddedToTheDataFilterIfItIsNotPresent()
    {
        var market = Market.Create(_credentials, "1.1", _subscription).Value;

        await market.Subscribe(new DataFilter().WithFullTradedLadder());

        var dataFilter = new DataFilter().WithFullTradedLadder().WithMarketDefinition();
        _subscription.DataFilter!.Should().BeEquivalentTo(dataFilter);
    }

    [Fact]
    public async Task SubscribeToOrdersIsCalled()
    {
        var market = Market.Create(_credentials, "1.1", _subscription).Value;

        await market.Subscribe();

        _subscription.SubscribedToOrdersCalled.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, RunnerStatus.Active, 1.0)]
    [InlineData(2, RunnerStatus.Removed, 99.9)]
    public void RunnersCanBeAdded(long id, RunnerStatus status, double adj)
    {
        var market = Market.Create(_credentials, "1.1", _subscription).Value;

        market.AddOrUpdateRunnerDefinition(id, status, adj);

        market.Runners.Should().ContainSingle();
        market.Runners.Single().Id.Should().Be(id);
        market.Runners.Single().Status.Should().Be(status);
        market.Runners.Single().AdjustmentFactor.Should().Be(adj);
    }

    [Fact]
    public void RunnersCanBeUpdated()
    {
        var market = Market.Create(_credentials, "1.1", _subscription).Value;
        market.AddOrUpdateRunnerDefinition(1, RunnerStatus.Active, 1.0);
        var runner = market.Runners.Single();

        market.AddOrUpdateRunnerDefinition(1, RunnerStatus.Removed, 99.9);

        market.Runners.Single().Should().BeSameAs(runner);
        market.Runners.Should().ContainSingle();
        market.Runners.Single().Id.Should().Be(1);
        market.Runners.Single().Status.Should().Be(RunnerStatus.Removed);
        market.Runners.Single().AdjustmentFactor.Should().Be(99.9);
    }
}
