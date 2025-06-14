using Betfair.Api.Requests;
using Betfair.Api.Requests.Markets;
using Betfair.Core.Enums;

namespace Betfair.Tests.Api.Requests.Markets;

public class MarketBookQueryTests
{
    [Fact]
    public void ConstructorSetsDefaultValues()
    {
        var query = new MarketBookQuery();

        query.PriceProjection.Should().BeNull();
        query.OrderProjection.Should().BeNull();
        query.MatchProjection.Should().BeNull();
        query.IncludeOverallPosition.Should().BeNull();
        query.PartitionMatchedByStrategyRef.Should().BeNull();
        query.CustomerStrategyRefs.Should().BeNull();
        query.CurrencyCode.Should().BeNull();
        query.Locale.Should().BeNull();
        query.MatchedSinceDate.Should().BeNull();
        query.BetIds.Should().BeNull();
    }

    [Fact]
    public void WithPriceProjectionSetsPriceProjection()
    {
        var query = new MarketBookQuery();
        var priceProjection = new PriceProjection();

        var result = query.WithPriceProjection(priceProjection);

        result.Should().BeSameAs(query);
        query.PriceProjection.Should().BeSameAs(priceProjection);
    }

    [Fact]
    public void WithOrderProjectionSetsOrderProjection()
    {
        var query = new MarketBookQuery();

        var result = query.WithOrderProjection(OrderStatus.Executable);

        result.Should().BeSameAs(query);
        query.OrderProjection.Should().Be(OrderStatus.Executable);
    }

    [Fact]
    public void WithMatchProjectionSetsMatchProjection()
    {
        var query = new MarketBookQuery();

        var result = query.WithMatchProjection(MatchProjection.RolledUpByPrice);

        result.Should().BeSameAs(query);
        query.MatchProjection.Should().Be(MatchProjection.RolledUpByPrice);
    }

    [Fact]
    public void IncludeOverallPositionsEnablesIncludeOverallPosition()
    {
        var query = new MarketBookQuery();

        var result = query.IncludeOverallPositions();

        result.Should().BeSameAs(query);
        query.IncludeOverallPosition.Should().BeTrue();
    }

    [Fact]
    public void PartitionByStrategyEnablesPartitionMatchedByStrategyRef()
    {
        var query = new MarketBookQuery();

        var result = query.PartitionByStrategy();

        result.Should().BeSameAs(query);
        query.PartitionMatchedByStrategyRef.Should().BeTrue();
    }

    [Fact]
    public void WithCustomerStrategiesAddsCustomerStrategyRefs()
    {
        var query = new MarketBookQuery();

        var result = query.WithCustomerStrategies("strategy1", "strategy2");

        result.Should().BeSameAs(query);
        query.CustomerStrategyRefs.Should().NotBeNull();
        query.CustomerStrategyRefs.Should().HaveCount(2);
        query.CustomerStrategyRefs.Should().Contain("strategy1");
        query.CustomerStrategyRefs.Should().Contain("strategy2");
    }

    [Fact]
    public void WithCustomerStrategiesIgnoresNullAndEmptyValues()
    {
        var query = new MarketBookQuery();

        var result = query.WithCustomerStrategies("strategy1", null!, string.Empty, "  ", "strategy2");

        result.Should().BeSameAs(query);
        query.CustomerStrategyRefs.Should().NotBeNull();
        query.CustomerStrategyRefs.Should().HaveCount(2);
        query.CustomerStrategyRefs.Should().Contain("strategy1");
        query.CustomerStrategyRefs.Should().Contain("strategy2");
    }

    [Fact]
    public void WithCurrencySetsCurrencyCode()
    {
        var query = new MarketBookQuery();

        var result = query.WithCurrency("USD");

        result.Should().BeSameAs(query);
        query.CurrencyCode.Should().Be("USD");
    }

    [Fact]
    public void WithLocaleSetsLocale()
    {
        var query = new MarketBookQuery();

        var result = query.WithLocale("en-GB");

        result.Should().BeSameAs(query);
        query.Locale.Should().Be("en-GB");
    }

    [Fact]
    public void MatchedSinceSetsMatchedSinceDate()
    {
        var query = new MarketBookQuery();
        var date = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var result = query.MatchedSince(date);

        result.Should().BeSameAs(query);
        query.MatchedSinceDate.Should().Be(date);
    }

    [Fact]
    public void WithBetsAddsBetIds()
    {
        var query = new MarketBookQuery();

        var result = query.WithBets("bet1", "bet2");

        result.Should().BeSameAs(query);
        query.BetIds.Should().NotBeNull();
        query.BetIds.Should().HaveCount(2);
        query.BetIds.Should().Contain("bet1");
        query.BetIds.Should().Contain("bet2");
    }

    [Fact]
    public void IncludeAllOrdersSetsOrderProjectionToAll()
    {
        var query = new MarketBookQuery();

        var result = query.IncludeAllOrders();

        result.Should().BeSameAs(query);
        query.OrderProjection.Should().Be(OrderStatus.All);
    }

    [Fact]
    public void ExecutableOrdersOnlySetsOrderProjectionToExecutable()
    {
        var query = new MarketBookQuery();

        var result = query.ExecutableOrdersOnly();

        result.Should().BeSameAs(query);
        query.OrderProjection.Should().Be(OrderStatus.Executable);
    }

    [Fact]
    public void ExecutionCompleteOrdersOnlySetsOrderProjectionToExecutionComplete()
    {
        var query = new MarketBookQuery();

        var result = query.ExecutionCompleteOrdersOnly();

        result.Should().BeSameAs(query);
        query.OrderProjection.Should().Be(OrderStatus.ExecutionComplete);
    }

    [Fact]
    public void NoMatchRollupSetsMatchProjectionToNoRollup()
    {
        var query = new MarketBookQuery();

        var result = query.NoMatchRollup();

        result.Should().BeSameAs(query);
        query.MatchProjection.Should().Be(MatchProjection.NoRollup);
    }

    [Fact]
    public void RollupByPriceSetsMatchProjectionToRolledUpByPrice()
    {
        var query = new MarketBookQuery();

        var result = query.RollupByPrice();

        result.Should().BeSameAs(query);
        query.MatchProjection.Should().Be(MatchProjection.RolledUpByPrice);
    }

    [Fact]
    public void RollupByAveragePriceSetsMatchProjectionToRolledUpByAvgPrice()
    {
        var query = new MarketBookQuery();

        var result = query.RollupByAveragePrice();

        result.Should().BeSameAs(query);
        query.MatchProjection.Should().Be(MatchProjection.RolledUpByAvgPrice);
    }

    [Fact]
    public void FluentChainingWorksCorrectly()
    {
        var priceProjection = new PriceProjection();
        var matchedSince = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var query = new MarketBookQuery()
            .WithPriceProjection(priceProjection)
            .ExecutableOrdersOnly()
            .RollupByPrice()
            .IncludeOverallPositions()
            .PartitionByStrategy()
            .WithCustomerStrategies("strategy1", "strategy2")
            .WithCurrency("GBP")
            .WithLocale("en-GB")
            .MatchedSince(matchedSince)
            .WithBets("bet1", "bet2");

        query.PriceProjection.Should().BeSameAs(priceProjection);
        query.OrderProjection.Should().Be(OrderStatus.Executable);
        query.MatchProjection.Should().Be(MatchProjection.RolledUpByPrice);
        query.IncludeOverallPosition.Should().BeTrue();
        query.PartitionMatchedByStrategyRef.Should().BeTrue();
        query.CustomerStrategyRefs.Should().HaveCount(2);
        query.CurrencyCode.Should().Be("GBP");
        query.Locale.Should().Be("en-GB");
        query.MatchedSinceDate.Should().Be(matchedSince);
        query.BetIds.Should().HaveCount(2);
    }
}
