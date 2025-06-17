using Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;

namespace Betfair.Tests.Api.Requests;

public class PriceProjectionBuilderTests
{
    [Fact]
    public void IncludeBestPricesAddsBestOffersToProjection()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.IncludeBestPrices();

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.PriceData.Should().Contain("EX_BEST_OFFERS");
    }

    [Fact]
    public void IncludeAllPricesAddsAllOffersToProjection()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.IncludeAllPrices();

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.PriceData.Should().Contain("EX_ALL_OFFERS");
    }

    [Fact]
    public void IncludeTradedPricesAddsTradedToProjection()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.IncludeTradedPrices();

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.PriceData.Should().Contain("EX_TRADED");
    }

    [Fact]
    public void IncludeStartingPriceBackAddsSpAvailableToProjection()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.IncludeStartingPriceBack();

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.PriceData.Should().Contain("SP_AVAILABLE");
    }

    [Fact]
    public void IncludeStartingPriceLayAddsSpTradedToProjection()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.IncludeStartingPriceLay();

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.PriceData.Should().Contain("SP_TRADED");
    }

    [Fact]
    public void WithVirtualPricesEnablesVirtualise()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.WithVirtualPrices();

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.Virtualise.Should().BeTrue();
    }

    [Fact]
    public void WithRolloverStakesEnablesRolloverStakes()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.WithRolloverStakes();

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.RolloverStakes.Should().BeTrue();
    }

    [Fact]
    public void WithBestPricesDepthSetsBestPricesDepth()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.WithBestPricesDepth(5);

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.ExBestOffersOverrides.Should().NotBeNull();
        projection.ExBestOffersOverrides!.BestPricesDepth.Should().Be(5);
    }

    [Fact]
    public void WithBestPricesDepthClampsValueBetween1And10()
    {
        var builder = new PriceProjectionBuilder();

        builder.WithBestPricesDepth(0);
        var projection1 = builder.Build();
        projection1.ExBestOffersOverrides!.BestPricesDepth.Should().Be(1);

        builder.WithBestPricesDepth(15);
        var projection2 = builder.Build();
        projection2.ExBestOffersOverrides!.BestPricesDepth.Should().Be(10);
    }

    [Fact]
    public void WithRollupModelSetsRollupModel()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.WithRollupModel("STAKE");

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.ExBestOffersOverrides.Should().NotBeNull();
        projection.ExBestOffersOverrides!.RollupModel.Should().Be("STAKE");
    }

    [Fact]
    public void WithRollupLimitSetsRollupLimit()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.WithRollupLimit(5);

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.ExBestOffersOverrides.Should().NotBeNull();
        projection.ExBestOffersOverrides!.RollupLimit.Should().Be(5);
    }

    [Fact]
    public void WithRollupLimitClampsNegativeValuesToZero()
    {
        var builder = new PriceProjectionBuilder();

        builder.WithRollupLimit(-5);
        var projection = builder.Build();
        projection.ExBestOffersOverrides!.RollupLimit.Should().Be(0);
    }

    [Fact]
    public void WithRollupLiabilityThresholdSetsThreshold()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.WithRollupLiabilityThreshold(100.0);

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.ExBestOffersOverrides.Should().NotBeNull();
        projection.ExBestOffersOverrides!.RollupLiabilityThreshold.Should().Be(100.0);
    }

    [Fact]
    public void WithRollupLiabilityThresholdClampsNegativeValuesToZero()
    {
        var builder = new PriceProjectionBuilder();

        builder.WithRollupLiabilityThreshold(-50.0);
        var projection = builder.Build();
        projection.ExBestOffersOverrides!.RollupLiabilityThreshold.Should().Be(0.0);
    }

    [Fact]
    public void BasicPricesConfiguresForBestPricesOnly()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.BasicPrices();

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.PriceData.Should().HaveCount(1);
        projection.PriceData.Should().Contain("EX_BEST_OFFERS");
    }

    [Fact]
    public void ComprehensivePricesConfiguresForAllPriceData()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.ComprehensivePrices();

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.PriceData.Should().HaveCount(3);
        projection.PriceData.Should().Contain("EX_BEST_OFFERS");
        projection.PriceData.Should().Contain("EX_ALL_OFFERS");
        projection.PriceData.Should().Contain("EX_TRADED");
    }

    [Fact]
    public void StartingPricesOnlyConfiguresForStartingPricesOnly()
    {
        var builder = new PriceProjectionBuilder();

        var result = builder.StartingPricesOnly();

        result.Should().BeSameAs(builder);
        var projection = builder.Build();
        projection.PriceData.Should().HaveCount(2);
        projection.PriceData.Should().Contain("SP_AVAILABLE");
        projection.PriceData.Should().Contain("SP_TRADED");
    }

    [Fact]
    public void BasicPricesClearsExistingPriceData()
    {
        var builder = new PriceProjectionBuilder();
        builder.IncludeAllPrices().IncludeTradedPrices();

        builder.BasicPrices();

        var projection = builder.Build();
        projection.PriceData.Should().HaveCount(1);
        projection.PriceData.Should().Contain("EX_BEST_OFFERS");
    }

    [Fact]
    public void ImplicitConversionToPriceProjectionWorks()
    {
        var builder = new PriceProjectionBuilder().IncludeBestPrices().WithVirtualPrices();

        PriceProjection projection = builder;

        projection.Should().NotBeNull();
        projection.PriceData.Should().Contain("EX_BEST_OFFERS");
        projection.Virtualise.Should().BeTrue();
    }

    [Fact]
    public void CreateReturnsNewBuilder()
    {
        var builder = PriceProjectionBuilder.Create();

        builder.Should().NotBeNull();
        builder.Should().BeOfType<PriceProjectionBuilder>();
    }

    [Fact]
    public void FluentChainingWorksCorrectly()
    {
        var projection = new PriceProjectionBuilder()
            .IncludeBestPrices()
            .IncludeAllPrices()
            .IncludeTradedPrices()
            .WithVirtualPrices()
            .WithRolloverStakes()
            .WithBestPricesDepth(3)
            .WithRollupModel("STAKE")
            .WithRollupLimit(10)
            .WithRollupLiabilityThreshold(50.0)
            .Build();

        projection.PriceData.Should().HaveCount(3);
        projection.PriceData.Should().Contain("EX_BEST_OFFERS");
        projection.PriceData.Should().Contain("EX_ALL_OFFERS");
        projection.PriceData.Should().Contain("EX_TRADED");
        projection.Virtualise.Should().BeTrue();
        projection.RolloverStakes.Should().BeTrue();
        projection.ExBestOffersOverrides.Should().NotBeNull();
        projection.ExBestOffersOverrides!.BestPricesDepth.Should().Be(3);
        projection.ExBestOffersOverrides.RollupModel.Should().Be("STAKE");
        projection.ExBestOffersOverrides.RollupLimit.Should().Be(10);
        projection.ExBestOffersOverrides.RollupLiabilityThreshold.Should().Be(50.0);
    }

    [Fact]
    public void BuildWithNoPriceDataReturnsNullPriceData()
    {
        var builder = new PriceProjectionBuilder();

        var projection = builder.Build();

        projection.PriceData.Should().BeNull();
    }
}
